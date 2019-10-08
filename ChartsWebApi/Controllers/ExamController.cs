using ChartsWebApi.models;
using ChartsWebApi.Models;
using ChartsWebApi.ViewModel;
using GetPDMObject;
using ilabHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : Controller
    {
        private DataContext db;
        private XJwtSettings _xjwtSettings;

        private const string DICT_KEY_MAX_EXAM_COUNT = "MAXNUM";
        private const string DICT_KEY_EXAM_PART_COUNT = "PARTNUM";

        private readonly IHostingEnvironment _hostingEnvironment;

        public ExamController(DataContext _db, IHostingEnvironment hostingEnvironment, IOptions<XJwtSettings> _jwtSettingsAccesser)
        {
            db = _db;
            _xjwtSettings = _jwtSettingsAccesser.Value;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("getTestList")]
        [HttpPost]
        public ActionResult GetTestList()
        {
            List<Build> builds = db.Build.ToList();
            return Json(ResultInfo<object>.Success(builds));
        }

        [Route("getParts")]
        [HttpPost]
        public ActionResult GetParts()
        {
            List<BuildItem> buildItems = db.BuildItem.Where(bi => bi.IsPart == "Y").ToList();
            return Json(ResultInfo<object>.Success(buildItems));
        }

        [Route("handIn")]
        [HttpGet]
        public ActionResult HandIn(int examId)
        {
            // 获取exam
            Exam exam = db.Exam.Where(e => e.Id == examId).SingleOrDefault();
            if (exam == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "获取考试信息失败。"));
            }
            if (exam.Status != "进行中")
            {
                return Json(ResultInfo<string>.Fail("1004", "考试已经结束，不能交卷。"));
            }
            //判断这条记录是否超时
            int time = GetExamTimeByType(exam.Type);
            if (time <= 0)
            {
                return Json(ResultInfo<string>.Fail("2001", "获取考试时间信息失败。"));
            }
            if (DateTime.Now.AddMinutes(time * -1) > exam.TimeStart)
            {
                exam.Status = "超时";
                db.SaveChanges();
                return Json(ResultInfo<string>.Fail("2004", "考试已超时。"));
            }
            Exam01Bill bill = db.Exam01Bill.Where(eb => eb.ExamId == exam.Id && eb.Name == "构建表").SingleOrDefault();
            if (bill == null)
            {
                return Json(ResultInfo<string>.Fail("3001", "获取考试题目详细信息失败"));
            }
            Dict dictExamScorePart = db.Dict.Where(d => d.Key == "EXAM_SCORE_PART").SingleOrDefault();
            if (dictExamScorePart == null)
            {
                return Json(ResultInfo<string>.Fail("4001", "获取考试分数字典信息失败"));
            }
            // 总分
            decimal totalScore = 0;
            decimal.TryParse(dictExamScorePart.Value, out totalScore);
            // 获取考试详情，里面存放了考生本次考试的题目（当前设定为两个）
            List<ExamPart> examParts = db.ExamPart.Where(ep => ep.ExamId == exam.Id).ToList();

            // 错误列表
            List<Exam01Bom> errors = new List<Exam01Bom>();
            // 获取考生回答
            List<Exam01Bom> answers = db.Exam01Bom.Where(eb => eb.ExamId == examId).ToList();
            // 循环考试的题目
            for (int x = 0; x < examParts.Count; x++)
            {
                ExamPart examPart = examParts[x];
                // 获取此题目的标准答案
                List<BuildItem> standardAnswer = db.BuildItem.Where(bi => bi.PartId == examPart.PartId).ToList();
                // 每条数据的分值为 总分(totalScore) / 标准答案总条数
                decimal itemScore = -1 * totalScore / standardAnswer.Count();
                // 循环考生的回答
                for (int i = answers.Count - 1,j = 0; j <= i; i--)
                {
                    Exam01Bom answer = answers[i];
                    if (string.IsNullOrWhiteSpace(answer.Part) && string.IsNullOrWhiteSpace(answer.Item))
                    {
                        // 从总分中扣除此项分数，理由：未填写
                        totalScore += itemScore;
                        answer.Score = itemScore;
                        answer.Desc = "填入了空内容";
                    }
                    answer.Part = answer.Part.Trim();
                    answer.Item = answer.Item.Trim();
                    for (int a = 0, b = standardAnswer.Count; a < b; a++)
                    {
                        BuildItem item = standardAnswer[a];
                        if (answer.Part == item.PartName.Trim() || answer.Part == item.PartNameAlias1.Trim() || answer.Part == item.PartNameAlias2.Trim())
                        {
                            if (answer.Item == item.Name.Trim() || answer.Item == item.NameAlias1.Trim() || answer.Item == item.NameAlias2.Trim())
                            {
                                // 将考生的回答移除（防止再次匹配到）
                                answers.Remove(answer);
                                // 将正确答案从答案列表中移除（防止再次匹配到）
                                standardAnswer.Remove(item);
                                // 正确，跳出循环
                                break;
                            }
                        }
                    }
                }
                if (x + 1 < examParts.Count)
                {
                    // 判断循环是否结束，最后一次循环再执行下面语句
                    continue;
                }
                // 循环结束后，仍有剩余回答，将多余回答分值扣除
                for (int i = 0; i < answers.Count; i++)
                {
                    Exam01Bom answer = answers[i];
                    answer.Desc = "填写错误";
                    answer.Score = itemScore;
                }
                // 循环结束后，仍有剩余答案，将剩余答案分值扣除
                for (int i = 0; i < standardAnswer.Count; i++)
                {
                    // 加入错误列表中
                    errors.Add(new Exam01Bom
                    {
                        ExamId = exam.Id,
                        Part = standardAnswer[i].PartName,
                        Item = standardAnswer[i].Name,
                        Score = itemScore,
                        Desc = "未填写",
                        Order = 99999
                    });
                    // 从总分中扣除此项分数
                    totalScore += itemScore;
                }
            }
            
            // 不允许负分
            if (totalScore < 0)
            {
                totalScore = 0;
            }
            // 判分
            bill.Score = totalScore;
            exam.Score = bill.Score;
            exam.Status = "阅卷中";
            exam.TimeEnd = DateTime.Now;
            db.Exam01Bom.AddRange(errors);
            db.SaveChanges();
            return Json(ResultInfo<string>.Success("您已成功交卷！"));
        }

        [Route("getExamScore")]
        [HttpGet]
        public ActionResult getExamScore()
        {
            List<Dict> dictExamScore = db.Dict.Where(d => d.Key.IndexOf("EXAM_SCORE") > -1).ToList();
            return Json(ResultInfo<object>.Success(dictExamScore));
        }

        [Route("getRemark")]
        [HttpGet]
        public ActionResult GetRemark()
        {
            List<Remark> remarks = db.Remark.ToList();
            return Json(ResultInfo<object>.Success(remarks));
        }

        [Route("preMark")]
        [HttpPost]
        public ActionResult PreMark(Exam01Bill bill)
        {
            Exam exam = db.Exam.Where(e => e.Id == bill.ExamId).SingleOrDefault();
            // 判断是否在阅卷中，不是阅卷中不允许再阅卷了
            if (exam.Status != "阅卷中")
            {
                return Json(ResultInfo<object>.Fail("1000", "试卷状态不为阅卷中，不可批改！"));
            }
            // 获取关于本次考试所有bill
            List<Exam01Bill> bills = db.Exam01Bill.Where(eb => eb.ExamId == bill.ExamId).ToList();
            bool final = false;
            // 判断是否是最后一次批改
            // 如果除当前bill外，其他bill都有分数了，则返回提示
            final = bills.Where(b => b.Id != bill.Id && b.Score == null).Count() == 0;
            // 获取分数
            decimal? score = 0;
            // 获取及格分数
            int qualifyScore = 0;
            Dict dictQualifyScore = db.Dict.Where(d => d.Key == "EXAM_QUALIFY_01").FirstOrDefault();
            if (dictQualifyScore == null)
            {
                // 判断是否已经全部评分了，如果全部评完，返回一个分数
                return Json(ResultInfo<object>.Fail("2000", "没有获取到及格分数字典数据！"));
            }
            int.TryParse(dictQualifyScore.Value, out qualifyScore);
            score = bills.Sum(b => b.Score) + bill.Score;
            string result = score < qualifyScore ? "不及格" : "通过";
            return Json(ResultInfo<object>.Success(new
            {
                final,
                score,
                result
            }));
        }

        [Route("mark")]
        [HttpPost]
        public ActionResult Mark(Exam01Bill bill)
        {
            bool final = false;
            int score = 0;
            string msg = "评卷完成";
            Exam exam = db.Exam.Where(e => e.Id == bill.ExamId).SingleOrDefault();
            // 判断是否在阅卷中，不是阅卷中不允许再阅卷了
            if (exam.Status != "阅卷中")
            {
                return Json(ResultInfo<object>.Fail("1000", "试卷状态不为阅卷中，不可批改！"));
            }
            db.Exam01Bill.Attach(bill);
            db.Entry(bill).State = EntityState.Modified;
            db.SaveChanges();
            // 获取关于本次考试所有bill，查看是否都有分数了
            List<Exam01Bill> bills = db.Exam01Bill.Where(eb => eb.ExamId == bill.ExamId).ToList();
            //
            final = bills.Where(b => b.Score == null).Count() == 0;
            score = decimal.ToInt32(bills.Sum(b => b.Score ?? 0));
            exam.Score = score;
            if (!exam.TimeEnd.HasValue)
            {
                exam.TimeEnd = DateTime.Now;
            }
            if (final)
            {
                // 
                int qualifyScore = 0;
                Dict dictQualifyScore = db.Dict.Where(d => d.Key == "EXAM_QUALIFY_01").FirstOrDefault();
                if (dictQualifyScore == null)
                {
                    return Json(ResultInfo<object>.Fail("2000", "没有获取到及格分数字典数据！"));
                }
                // 判断是否已经全部评分了，如果全部评完，返回一个分数
                int.TryParse(dictQualifyScore.Value, out qualifyScore);
                if (score >= qualifyScore)
                {
                    exam.Status = "通过";
                }
                else
                {
                    exam.Status = "不及格";
                }
                int userid = exam.UserId;
                OrgUser orgUser = db.OrgUser.Where(u => u.Id == userid).FirstOrDefault();
                // 上传成绩
                if (orgUser.Login.EndsWith(_xjwtSettings.UserPad))
                {
                    TestSubmitResult r = SubmitScore(orgUser, exam);
                    switch (r.code)
                    {
                        case 0:
                            msg = msg + "。已成功提交成绩！";
                            break;
                        case 2:
                            msg = msg + "。代码2：解密失败。";
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        default:
                            msg = msg + "。代码" + r.code + "：" + r.msg;
                            break;
                    }
                    
                }
            }
            db.SaveChanges();
            // 判断是否已经全部评分了，如果全部评完，返回一个分数
            return Json(ResultInfo<object>.Success(new
            {
                final,
                score,
                msg
            }));
        }

        private string GetProjectType(string type)
        {
            switch(type)
            {
                case "测稿编绘":
                    return "建筑观察分析与测稿编绘";
                case "虚拟测量":
                    return "建筑虚拟测量";
                default:
                    return string.Empty;
            }
        }

        private TestSubmitResult SubmitScore(OrgUser orgUser, Exam exam)
        {
            // 只有实验空间的用户才上传成绩
            long startDate = Utils.GetTimestamp(exam.TimeStart);
            long endDate = Utils.GetTimestamp(exam.TimeEnd.Value);
            int timeUsed = (int)(endDate - startDate) / 1000 / 60;
            int score = decimal.ToInt32(exam.Score ?? 0);
            if (timeUsed <= 0)
            {
                timeUsed = 1;
            }
            XlibUserScore userScore = new XlibUserScore
            {
                username = orgUser.Login.Replace(_xjwtSettings.UserPad, string.Empty),
                projectTitle = _xjwtSettings.ExamName,
                childProjectTitle = GetProjectType(exam.Type),
                startDate = startDate,
                endDate = endDate,
                issuerId = _xjwtSettings.IssueId,
                score = score,
                status = exam.Status == "超时" ? 2 : 1,
                timeUsed = timeUsed
            };
            string body = JsonConvert.SerializeObject(userScore);
            XJWT xjwtObj = new XJWT(_xjwtSettings.Secret, _xjwtSettings.AesKey, _xjwtSettings.IssueId);
            string token = xjwtObj.createToken(body, XJWT.Type.SYS, Utils.GetTimestamp());
            string xjwt = HttpUtility.UrlEncode(token);
            HttpClient client = new HttpClient();
            string url = string.Format("{0}{1}?xjwt={2}", _xjwtSettings.Server, _xjwtSettings.UrlSubmitScore, xjwt);
            client.Timeout = TimeSpan.FromSeconds(10);
            var responseString = client.GetStringAsync(url);
            string str = responseString.Result;
            return JsonConvert.DeserializeObject<TestSubmitResult>(str);
        }

        [Route("getExam/{userguid}")]
        [HttpGet]
        public ActionResult GetExaming(string userguid)
        {
            if (string.IsNullOrWhiteSpace(userguid))
            {
                return Json(ResultInfo<string>.Fail("0001", "用户信息错误"));
            }
            OrgUser user = db.OrgUser.Where(ua => ua.Guid == userguid).FirstOrDefault();
            if (user == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "用户信息不存在"));
            }
            Exam exam = db.Exam.Where(e => e.Status == "进行中" && e.UserId == user.Id).FirstOrDefault();
            if (exam == null)
            {
                return Json(ResultInfo<string>.Success(string.Empty));
            }
            // 判断是否超时，超时就不再返回了，而是做超时处理
            int time = GetExamTimeByType(exam.Type);
            if (time <= 0)
            {
                return Json(ResultInfo<string>.Fail("2001", "获取考试时间信息失败。"));
            }
            if (DateTime.Now.AddMinutes(time * -1) > exam.TimeStart)
            {
                exam.Status = "超时";
                db.SaveChanges();
                return Json(ResultInfo<string>.Success(string.Empty));
            }
            return Json(ResultInfo<Exam>.Success(exam));
        }

        [Route("get/{examid}")]
        [HttpGet]
        public ActionResult GetExam(int examid)
        {
            // 根据考试ID获取试卷信息
            Exam exam = db.Exam.Where(e => e.Id == examid).SingleOrDefault();
            if (exam == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "获取考试信息失败"));
            }
            if (exam.Type == "虚拟测量")
            {
                List<Exam02Bill> bills = db.Exam02Bill.Where(eb => eb.ExamId == exam.Id).ToList();
                List<Exam02Tool> tools = db.Exam02Tool.Where(et => et.ExamId == exam.Id).ToList();
                return Json(ResultInfo<object>.Success(new
                {
                    exam,
                    bills,
                    tools
                }));
            }
            else
            {
                List<Exam01Bill> bills = db.Exam01Bill.Where(eb => eb.ExamId == exam.Id).ToList();
                // 未超时，将此条记录直接返回
                return Json(ResultInfo<object>.Success(new
                {
                    exam,
                    bills
                }));
            }
        }

        [Route("get")]
        [HttpPost]
        public ActionResult Get(GetExam data)
        {
            // 根据userguid获取用户的id
            OrgUser user = db.OrgUser.Where(u => u.Guid == data.userguid).FirstOrDefault();
            if (user == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "获取用户信息失败"));
            }
            // 查询所有考试类型
            List<BuildItem> buildItem = db.BuildItem.Where(bi => bi.IsPart == "Y").ToList();
            // 根据用户id查询已经考过的试
            IEnumerable<Exam> examed = db.Exam.Where(ea => ea.Type == data.Type && ea.UserId == user.Id && ea.Status != "作废");
            // 获取最大考试次数
            Dict dict = db.Dict.Where(d => d.Key == DICT_KEY_MAX_EXAM_COUNT).SingleOrDefault();
            int maxnum = 0;
            if (dict != null)
            {
                int.TryParse(dict.Value, out maxnum);
            }
            // 判断是否超过最大考试次数
            if (examed.Count() >= maxnum)
            {
                return Json(ResultInfo<string>.Fail("1002", "超过最大次数限制，无法继续考试。"));
            }
            // 提前建立BILL表数据
            List<Exam02Bill> bills = null;
            List<BuildSurvey> buildSurveys = db.BuildSurvey.ToList();
            // 提前建立考试部位数据
            List<ExamPart> examParts = null;
            // 判断是否有正在进行中的
            List<Exam> examing = examed.Where(ea => ea.Status == "进行中").ToList();
            int time = GetExamTimeByType(data.Type);
            if (time <= 0)
            {
                return Json(ResultInfo<string>.Fail("1010", "获取考试时长信息失败。"));
            }
            for (int i = examing.Count - 1, j = 0; i >= j; i--)
            {
                //判断这条记录是否超时
                if (DateTime.Now.AddMinutes(time * -1) > examing[i].TimeStart)
                {
                    // 超时，将其状态修改
                    examing[i].Status = "超时";
                }
                else
                {
                    examing[i].TimeEnd = examing[i].TimeStart.AddMinutes(time);
                    if (examing[i].Type == "虚拟测量")
                    {
                        bills = db.Exam02Bill.Where(eb => eb.ExamId == examing[i].Id).ToList();
                        return Json(ResultInfo<object>.Success(new
                        {
                            exam = examing[i],
                            bills,
                            buildSurveys
                        }));
                    }
                    else
                    {
                        // 未超时，将此条记录直接返回
                        examParts = db.ExamPart.Where(ep => ep.ExamId == examing[i].Id).ToList();
                        return Json(ResultInfo<object>.Success(new
                        {
                            exam = examing[i],
                            examParts
                        }));
                    }
                }
            }
            // 考试类型 examType, A: 个人, B: 组队
            string examType = "A";
            if (data.GroupUsers != null && data.GroupUsers.Count > 0)
            {
                examType = "B";
            }
            // 新建Exam数据
            Exam exam = new Exam
            {
                Type = data.Type,
                Model = "A",
                Status = "进行中",
                ExaminerType = examType,
                UserId = user.Id,
                UserName = user.Username
            };
            bills = new List<Exam02Bill>();
            examParts = new List<ExamPart>();
            if (data.Type == "测稿编绘")
            {
                // 如果是测稿编绘，必须要有题目，如果没有题目考试不进行创建
                if (buildItem.Count == 0)
                {
                    return Json(ResultInfo<string>.Fail("1002", "没有获取到考试题目，请联系管理员。"));
                }
            }
            // 可以新建出考试了
            db.Exam.Add(exam);
            db.SaveChanges();
            if (data.Type == "测稿编绘")
            {
                // 随机挑选其中DICT_KEY_EXAM_PART_NUM个，新建数据到ExamPart表
                int partCount = GetExamPartCount();
                for (int i = 0; i < partCount; i++)
                {
                    // 随机其中的任意部件
                    int index = new Random().Next(buildItem.Count);
                    BuildItem build = buildItem[index];
                    // 将选取的部件添加到examParts
                    examParts.Add(new ExamPart
                    {
                        ExamId = exam.Id,
                        PartId = build.Id,
                        PartCode = build.Code,
                        PartName = build.Name
                    });
                    // 从build_item_list中移除这个部件
                    buildItem.Remove(build);
                }
                // 添加EXAM_01_BILL表数据：01 构建表，02 虚拟照片，03 草图绘制
                List<Exam01Bill> exam01Bills = new List<Exam01Bill>();
                exam01Bills.Add(new Exam01Bill
                {
                    ExamId = exam.Id,
                    Code = "01",
                    Name = "构建表"
                });

                exam01Bills.Add(new Exam01Bill
                {
                    ExamId = exam.Id,
                    Code = "02",
                    Name = "虚拟照片"
                });

                exam01Bills.Add(new Exam01Bill
                {
                    ExamId = exam.Id,
                    Code = "03",
                    Name = "草图绘制"
                });
                db.ExamPart.AddRange(examParts);
                db.Exam01Bill.AddRange(exam01Bills);
            }
            else if (data.Type == "虚拟测量")
            {
                // 获取SURVEY表数据
                List<BuildSurvey> surveys = db.BuildSurvey.ToList();
                for (int i = 0, j = surveys.Count; i < j; i++)
                {
                    bills.Add(new Exam02Bill
                    {
                        ExamId = exam.Id,
                        SurveyId = surveys[i].Id,
                        Code = surveys[i].Code,
                        Name = surveys[i].Name
                    });
                }
                db.Exam02Bill.AddRange(bills);
            }
            if (examType == "B")
            {
                // 新增group表数据
                List<ExamGroup> examGroups = new List<ExamGroup>();
                examGroups.Add(new ExamGroup
                {
                    ExamId = exam.Id,
                    UserId = user.Id,
                    UserName = user.Name
                });
                data.GroupUsers.ForEach(gu =>
                {
                    examGroups.Add(new ExamGroup
                    {
                        ExamId = exam.Id,
                        UserId = gu.Id,
                        UserName = gu.Name
                    });
                });

                // 获取所有用户名
                IEnumerable<string> names = data.GroupUsers.Select(gu => gu.Name);
                // 获取所有用户ID
                IEnumerable<int> ids = data.GroupUsers.Select(gu => gu.Id);
                //组队试验，加上组长的名字
                exam.GroupName = string.Format("{0},{1}", user.Name, string.Join(",", names));

                db.ExamGroup.AddRange(examGroups);
            }
            db.SaveChanges();
            // 新建的Exam数据为了区分，加一个属性
            exam.Status = "Y";
            exam.TimeEnd = DateTime.Now.AddMinutes(time);
            if (data.Type == "虚拟测量")
            {
                return Json(ResultInfo<object>.Success(new
                {
                    exam,
                    bills,
                    buildSurveys
                }));
            }
            else if (data.Type == "测稿编绘")
            {
                return Json(ResultInfo<object>.Success(new
                {
                    exam,
                    examParts
                }));
            }
            else
            {
                return Json(ResultInfo<object>.Fail("9001", "未能获取到考试类型"));
            }
        }

        private int GetExamTimeByType(string type)
        {
            // 获取考试时间
            List<Dict> dicts = db.Dict.Where(d => d.Key == "EXAM_TIME_01" || d.Key == "EXAM_TIME_02").ToList();
            int time = 0;
            var dict = dicts.Where(d =>
            {
                if (type == "测稿编绘")
                {
                    return d.Key == "EXAM_TIME_01";
                }
                else if (type == "虚拟测量")
                {
                    return d.Key == "EXAM_TIME_02";
                }
                else
                {
                    return false;
                }
            }).FirstOrDefault();
            if (dict == null)
            {
                return time;
            }
            int.TryParse(dict.Value, out time);
            return time;
        }

        private int GetExamPartCount()
        {
            Dict dict = db.Dict.Where(d => d.Key == DICT_KEY_EXAM_PART_COUNT).FirstOrDefault();
            int count = 1;
            if (dict == null)
            {
                return count;
            }
            int.TryParse(dict.Value, out count);
            return count;
        }

        [Route("getGroupUser/{guid}")]
        [HttpGet]
        public ActionResult GetGroupUser(string guid)
        {
            OrgUser leader = db.OrgUser.Where(ou => ou.Guid == guid && ou.Status == "有效").FirstOrDefault();
            if (leader == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "组长信息获取失败！"));
            }
            // 根据组长的PAR_GUID获取所有可选择组员，不包含组长自己(默认已添加)
            List<OrgUser> users = db.OrgUser.Where(ou => ou.Guid != guid && ou.ParGuid == leader.ParGuid && ou.Status == "有效").ToList();
            // 返回组员信息
            return Json(ResultInfo<List<OrgUser>>.Success(users));
        }

        [Route("getTable")]
        [HttpGet]
        public ActionResult GetTable(int examId)
        {
            List<Exam01Bom> examBoms = db.Exam01Bom.Where(eb => eb.ExamId == examId).OrderBy(eb => eb.Order).ToList();
            // 获取所有BOMID，
            List<int> bomIds = examBoms.Select(eb => eb.Id).ToList();
            // 根据BOMID取得所有图片
            List<Exam01BomImg> bomImages = db.Exam01BomImg.Where(ebi => bomIds.Exists(a => a == ebi.BomId)).ToList();

            List<PartInfo> partInfo = new List<PartInfo>();
            for (int i = 0, j = examBoms.Count; i < j; i++)
            {
                // 获取bomid相对应的picturesid
                int[] pictureIds = bomImages.Where(bi => bi.BomId == examBoms[i].Id).Select(bi => bi.ImgId).ToArray();

                partInfo.Add(new PartInfo
                {
                    part = examBoms[i].Part,
                    item = examBoms[i].Item,
                    desc = examBoms[i].Desc,
                    desc1 = examBoms[i].Desc1,
                    desc2 = examBoms[i].Desc2,
                    picture = pictureIds,
                    score = examBoms[i].Score,
                    order = examBoms[i].Order
                });
            }
            return Json(ResultInfo<List<PartInfo>>.Success(partInfo));
        }

        [Route("saveTable")]
        [HttpPost]
        public ActionResult SaveTable(ExamBomTable data)
        {
            // 根据ExamId判断是否已经超时
            Exam exam = db.Exam.Where(e => e.Id == data.ExamId && e.Status == "进行中").FirstOrDefault();
            if (exam == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "已经超过考试时间限制，无法对本次考试进行任何修改。"));
            }
            int time = GetExamTimeByType(exam.Type);
            if (time <= 0)
            {
                return Json(ResultInfo<string>.Fail("1010", "获取考试时长信息失败。"));
            }
            //判断这条记录是否超时
            if (DateTime.Now.AddMinutes(time * -1) > exam.TimeStart)
            {
                // 超时，将其状态修改
                exam.Status = "超时";
                db.SaveChanges();
                return Json(ResultInfo<string>.Fail("1001", "已经超过考试时间限制，无法对本次考试进行任何修改。"));
            }
            List<Exam01Bom> exam01Boms = new List<Exam01Bom>();
            List<Exam01BomImg> bomImages = new List<Exam01BomImg>();
            for (int i = 0, j = data.boms.Count; i < j; i++)
            {
                var bom = data.boms[i];
                exam01Boms.Add(new Exam01Bom
                {
                    ExamId = data.ExamId,
                    Item = bom.item,
                    Part = bom.part,
                    Desc1 = bom.desc1,
                    Desc2 = bom.desc2,
                    Order = i,
                    PictureIds = bom.picture
                });
            }
            // 获取此考试所有填写的 bom（对应关系）
            var boms = db.Exam01Bom.Where(e => e.ExamId == data.ExamId);
            // 获取所有 bomid 列表
            var bomids = boms.Select(b => b.Id).ToList();
            // 根据bomid 删除原来的关联图片
            db.Exam01BomImg.RemoveRange(db.Exam01BomImg.Where(bi => bomids.Exists(bomId => bomId == bi.BomId)));
            // 删除原有的
            db.Exam01Bom.RemoveRange(boms);

            // 保存新的bom数据
            db.Exam01Bom.AddRange(exam01Boms);
            // 为了取得ID，必须保存一次
            db.SaveChanges();
            for (int i = 0, j = exam01Boms.Count; i < j; i++)
            {
                var bom = exam01Boms[i];
                // 判断是否有照片，将照片信息存储
                if (bom.PictureIds != null)
                {
                    for (int x = 0, y = bom.PictureIds.Length; x < y; x++)
                    {
                        bomImages.Add(new Exam01BomImg
                        {
                            BomId = bom.Id,
                            ImgId = bom.PictureIds[x]
                        });
                    }
                }
            }
            // 保存新的image数据
            db.Exam01BomImg.AddRange(bomImages);
            db.SaveChanges();
            return Json(ResultInfo<List<Exam01Bom>>.Success(exam01Boms));
        }

        [AllowAnonymous]
        [Route("picture/{id}")]
        [HttpGet]
        public ActionResult Picture(int id)
        {
            Exam01Img img = db.Exam01Img.Where(ei => ei.Id == id).FirstOrDefault();
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string imgPath = Path.Combine(webRootPath, img.FullName);
            string contentType = "application/octet-stream";
            return File(new FileStream(imgPath, FileMode.Open), contentType);
        }

        [Route("getPictures")]
        [HttpPost]
        public ActionResult GetPicture(PictureModel picture)
        {
            var query = db.Exam01Img.Where(ei => ei.ExamId == picture.ExamId);
            if (!string.IsNullOrWhiteSpace(picture.Type))
            {
                query = query.Where(ei => ei.Type == picture.Type);
            }
            List<Exam01Img> imgs = query.ToList();
            return Json(ResultInfo<List<Exam01Img>>.Success(imgs));
        }

        [Route("removePicture/{id}")]
        [HttpGet]
        public ActionResult RemovePicture(int id)
        {
            Exam01Img img = db.Exam01Img.Where(ei => ei.Id == id).FirstOrDefault();
            if (img == null)
            {
                return Json(ResultInfo<string>.Success("文件已经被删除"));
            }
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string imgPath = Path.Combine(webRootPath, img.FullName);
            FileInfo file = new FileInfo(imgPath);
            if (file.Exists)
            {
                file.Delete();
            }
            db.Exam01Img.Remove(img);
            db.SaveChanges();
            return Json(ResultInfo<string>.Success("删除成功"));
        }

        // POST api/file
        [Route("upload")]
        [HttpPost]
        [DisableRequestSizeLimit]
        public ActionResult Upload([FromForm]PictureModel picture)
        {
            //
            if (picture.ExamId == 0)
            {
                return Json(ResultInfo<string>.Fail("0001","获取考试信息失败，可能考试已经结束！"));
            }
            Exam exam = db.Exam.Where(e => e.Id == picture.ExamId && e.Status == "进行中").FirstOrDefault();
            if (exam == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "已经超过考试时间限制，无法对本次考试进行任何修改。"));
            }
            int time = GetExamTimeByType(exam.Type);
            if (time <= 0)
            {
                return Json(ResultInfo<string>.Fail("1010", "获取考试时长信息失败。"));
            }
            //判断这条记录是否超时
            if (DateTime.Now.AddMinutes(time * -1) > exam.TimeStart)
            {
                // 超时，将其状态修改
                exam.Status = "超时";
                db.SaveChanges();
                return Json(ResultInfo<string>.Fail("1001", "已经超过考试时间限制，无法对本次考试进行任何修改。"));
            }
            // 如果有文件，这里上传
            var file = Request.Form.Files[0];
            string folderName = string.Format("Upload{0}Test{0}{1}", Path.DirectorySeparatorChar, picture.ExamId);
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            var img = db.Exam01Img.Where(ei => ei.ExamId == picture.ExamId).OrderByDescending(ei => ei.Code).FirstOrDefault();
            int imgIndex = 1;
            if (img != null)
            {
                int.TryParse(img.Code, out imgIndex);
                imgIndex = imgIndex + 1;
            }
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string prefix = picture.Type == "测稿" ? "CG" : "ZP";
                string suffix = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                string newFileName = string.Format("{0}{2}{1}", prefix, suffix, imgIndex);
                string fullPath = Path.Combine(newPath, newFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                string path = Path.Combine(folderName, newFileName);
                // 判断图片数量

                // 添加图片数据
                Exam01Img examImg = new Exam01Img
                {
                    ExamId = picture.ExamId,
                    Type = picture.Type,
                    Name = string.Format("{0}{1}", prefix, imgIndex),
                    Code = imgIndex.ToString(),
                    FullName = path
                };
                db.Exam01Img.Add(examImg);
                db.SaveChanges();
                return Json(ResultInfo<Exam01Img>.Success(examImg));
            }
            return Json(ResultInfo<string>.Fail("1001", "没有文件需要上传"));
        }

        [Route("getSurvey")]
        [HttpGet]
        public ActionResult GetSurvey()
        {
            List<BuildSurvey> buildSurveys = db.BuildSurvey.ToList();
            return Json(ResultInfo<List<BuildSurvey>>.Success(buildSurveys));
        }

        [Route("getTools")]
        [HttpGet]
        public ActionResult GetTools()
        {
            List<BuildTool> tools = db.BuildTool.OrderBy(bt => bt.Code).ToList();
            return Json(ResultInfo<List<BuildTool>>.Success(tools));
        }

        [Route("getSelectedTools/{examid}")]
        [HttpGet]
        public ActionResult GetSelectedTools(int examid)
        {
            List<Exam02Tool> selectedTools = db.Exam02Tool.Where(et => et.ExamId == examid).ToList();
            return Json(ResultInfo<List<Exam02Tool>>.Success(selectedTools));
        }

        [Route("getSurveyGroup/{groupid}")]
        [HttpGet]
        public ActionResult getSurveyGroup(int groupid)
        {
            BuildSurveyGroup surveygroup = db.BuildSurveyGroup.Where(sg => groupid == sg.Id).SingleOrDefault();
            return Json(ResultInfo<object>.Success(surveygroup));
        }

        [Route("getSurveyGroupTools/{groupid}")]
        [HttpGet]
        public ActionResult getSurveyGroupTools(int groupid)
        {
            List<BuildSurveyGroupTool> surveygrouptools = db.BuildSurveyGroupTool.Where(gt => groupid == gt.GroupId).ToList();
            return Json(ResultInfo<object>.Success(surveygrouptools));
        }

        [Route("saveTools")]
        [HttpPost]
        public ActionResult SaveTools(ToolsInfo data)
        {
            // 判断当前BILL是否已经提交过，不允许重复提交
            Exam02Bill bill = db.Exam02Bill.Where(b => b.Id == data.bill.Id).SingleOrDefault();
            if (bill == null)
            {
                return Json(ResultInfo<string>.Fail("1040", "数据错误，请咨询管理员！"));
            }
            else if (bill.Score != null)
            {
                return Json(ResultInfo<string>.Fail("1020", "不允许重复提交！"));
            }
            // 根据SURVEYID 查询正确工具组
            List<BuildSurveyGroup> groups = db.BuildSurveyGroup.Where(sg => sg.SurveyId == bill.SurveyId).ToList();
            // 循环工具组
            int groupid = 0;
            for (int i = 0, j = groups.Count; i < j; i++)
            {
                BuildSurveyGroup group = groups[i];
                // 获取工具组里面的工具列表
                if (group.GroupTools == null)
                {
                    group.GroupTools = db.BuildSurveyGroupTool.Where(gt => gt.GroupId == group.Id).ToList();
                }
                // 因为工具列表是每条一个，多个为多条数据
                // 这里将相同类型工具多条数据合并为一条多个
                List<GroupTool> groupTools = (from gt in @group.GroupTools
                                        group gt by new
                                        {
                                            gt.GroupId,
                                            gt.ToolId
                                        } into gts
                                        select new GroupTool
                                        {
                                            ToolId = gts.Key.ToolId,
                                            GroupId = gts.Key.GroupId,
                                            Count = gts.Count()
                                        }).ToList();
                // 判断数量，只能多选两种，如果超过了，换一个答案匹配
                if (data.tools.Count - 2 > groupTools.Count)
                {
                    continue;
                }
                int num = 0;
                // 循环答案工具列表
                for (int a = 0; a < groupTools.Count; a++)
                {
                    var tool = groupTools[a];
                    //用户选择中是否有正确答案，计数+1，如果计数 = 正确答案数量即正确
                    bool isFind = data.tools.Exists(t => t.Id == tool.ToolId && t.count == tool.Count);
                    if (isFind)
                    {
                        // 正确 计数+1
                        num += 1;
                    }
                }
                if (num == groupTools.Count)
                {
                    // 存储正确答案groupid
                    groupid = group.Id;
                    // 将没有选择的工具也扔到tools里
                }
            }

            List<Exam02Tool> tools = new List<Exam02Tool>();
            // 无论正确不正确，用户选择的工具都要存储
            for (int i = 0, j = data.tools.Count; i < j; i++)
            {
                BuildTool tool = data.tools[i];
                tools.Add(new Exam02Tool
                {
                    ExamId = data.ExamId,
                    BillId = bill.Id,
                    ToolId = tool.Id,
                    Count = tool.count
                });
            }
            db.Exam02Tool.AddRange(tools);

            Exam exam = db.Exam.Where(e => e.Id == data.ExamId && e.Status == "进行中").SingleOrDefault();
            if (exam == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "你正参加的考试不存在或已经结束，请询问管理员。"));
            }

            if (groupid == 0)
            {
                // 不及格
                bill.Score = 0;
                exam.Score = 0;
                exam.Status = "不及格";
                exam.TimeEnd = DateTime.Now;

                // 提交成绩
                OrgUser orgUser = db.OrgUser.Where(u => u.Id == exam.UserId).SingleOrDefault();
                // 上传成绩
                if (orgUser.Login.EndsWith(_xjwtSettings.UserPad))
                {
                    SubmitScore(orgUser, exam);
                }

                db.SaveChanges();
                return Json(ResultInfo<Exam>.Success(exam));
            }

            // 正确，判断除此次BILL外是否所有BILL都有SCORE了
            bool isPass = !db.Exam02Bill.Where(eb => eb.ExamId == exam.Id && eb.Id != bill.Id).ToList().Exists(eb => eb.Score == null);
            bill.GroupId = groupid;
            bill.Score = 100;
            data.bill.Score = 100;
            data.bill.GroupId = groupid;

            if (isPass)
            {
                exam.Score = 100;
                exam.Status = "通过";
                exam.TimeEnd = DateTime.Now;
            }
            db.SaveChanges();
            if (isPass)
            {
                exam.Id = groupid;
                // 提交成绩
                OrgUser orgUser = db.OrgUser.Where(u => u.Id == exam.UserId).SingleOrDefault();
                // 上传成绩
                if (orgUser.Login.EndsWith(_xjwtSettings.UserPad))
                {
                    SubmitScore(orgUser, exam);
                }
                return Json(ResultInfo<Exam>.Success(exam));
            }
            return Json(ResultInfo<object>.Success(data));
        }
    }
}