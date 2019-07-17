using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChartsWebApi.models;
using ChartsWebApi.ViewModel;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : Controller
    {
        private DataContext db;
        private const string DICT_KEY_MAX_EXAM_COUNT = "MAXNUM";

        private readonly IHostingEnvironment _hostingEnvironment;

        public ExamController(DataContext _db, IHostingEnvironment hostingEnvironment)
        {
            db = _db;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("getParts")]
        [HttpPost]
        public ActionResult GetParts()
        {
            List<BuildItem> buildItems = db.BuildItem.Where(bi => bi.IsPart == "Y").ToList();
            return Json(ResultInfo<List<BuildItem>>.Success(buildItems));
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
            decimal partMaxScore = 0;
            decimal.TryParse(dictExamScorePart.Value, out partMaxScore);
            // 获取buildItem表
            List<BuildItem> buildItems = db.BuildItem.Where(bi => bi.PartId == exam.BuildPartId).ToList();
            // 错误列表
            List<Exam01Bom> emptyItems = new List<Exam01Bom>();
            // 获取bom表
            List<Exam01Bom> boms = db.Exam01Bom.Where(eb => eb.ExamId == examId).ToList();
            //每条数据的分值为 -  30 / BuildItem总条数
            decimal itemScore = -1 * partMaxScore / buildItems.Count();
            // 循环BOM表
            for(int i = 0; i < boms.Count; i++)
            {
                Exam01Bom bom = boms[i];
                if (string.IsNullOrWhiteSpace(bom.Part) && string.IsNullOrWhiteSpace(bom.Item))
                {
                    // 从总分中扣除此项分数，理由：未填写
                    partMaxScore += itemScore;
                    bom.Score = itemScore;
                    bom.Desc = "填入了空内容";
                }
                bom.Part = bom.Part.Trim();
                bom.Item = bom.Item.Trim();
                // 循环BuildItem表（正确答案）
                for (int j = 0; j < buildItems.Count; j++)
                {
                    BuildItem item = buildItems[j];
                    if (item.isMapped)
                    {
                        continue;
                    }
                    if (bom.Part == item.PartName || bom.Part == item.PartNameAlias1 || bom.Part == item.PartNameAlias2)
                    {
                        if (bom.Item == item.Name || bom.Item == item.NameAlias1 || bom.Item == item.NameAlias2)
                        {
                            // 正确，跳出循环
                            bom.isMapped = true;
                            item.isMapped = true;
                            continue;
                        }
                    }
                }
                if (!bom.isMapped)
                {
                    // 从总分中扣除此项分数
                    partMaxScore += itemScore;
                    bom.Score = itemScore;
                    bom.Desc = "填写错误";
                }
            }
            // 循环BuildItem表，查找未被mapped的，全部统一扣分
            for (int i = 0; i < buildItems.Count; i++)
            {
                if (!buildItems[i].isMapped)
                {
                    // 加入错误列表中
                    emptyItems.Add(new Exam01Bom
                    {
                        ExamId = exam.Id,
                        Part = buildItems[i].PartName,
                        Item = buildItems[i].Name,
                        Score = itemScore,
                        Desc = "未填写",
                        Order = 99999
                    });
                    // 从总分中扣除此项分数
                    partMaxScore += itemScore;
                }
            }
            // 不允许负分
            if (partMaxScore < 0)
            {
                partMaxScore = 0;
            }
            // 判分
            bill.Score = partMaxScore;
            exam.Score = bill.Score;
            exam.Status = "阅卷中";
            db.Exam01Bom.AddRange(emptyItems);
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
            decimal? score = 0;
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
            score = bills.Sum(b => b.Score);
            if (final)
            {
                // 
                int qualifyScore = 0;
                Dict dictQualifyScore = db.Dict.Where(d => d.Key == "EXAM_QUALIFY_01").FirstOrDefault();
                if (dictQualifyScore == null)
                {
                    // 判断是否已经全部评分了，如果全部评完，返回一个分数
                    return Json(ResultInfo<object>.Fail("2000", "没有获取到及格分数字典数据！"));
                }
                int.TryParse(dictQualifyScore.Value, out qualifyScore);
                if (score >= qualifyScore)
                {
                    exam.Status = "通过";
                }
                else
                {
                    exam.Status = "不及格";
                }
            }
            // 判断是否已经全部评分了，如果全部评完，返回一个分数
            return Json(ResultInfo<object>.Success(new
            {
                final,
                score,
                msg = "评卷完成!"
            }));
        }

        [Route("getExam/{userguid}")]
        [HttpGet]
        public ActionResult GetExaming(string userguid)
        {
            if (string.IsNullOrWhiteSpace(userguid))
            {
                return Json(ResultInfo<string>.Fail("0001", "用户信息错误"));
            }
            OrgUser user = db.OrgUser.Where(ua => ua.Guid == userguid).SingleOrDefault();
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
                            bills
                        }));
                    }
                    else
                    {
                        // 未超时，将此条记录直接返回
                        return Json(ResultInfo<Exam>.Success(examing[i]));
                    }
                }
            }
            // 考试类型 examType, A: 个人, B: 组队
            string examType = "A";
            if (data.GroupUsers != null && data.GroupUsers.Count > 0)
            {
                examType = "B";
            }
            // 考过的试
            List<Exam> examedList = examed.ToList();
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
            // 判断考试类型，如果需要随机
            if (data.Type == "测稿编绘")
            {
                // 将考过的排除
                List<BuildItem> buildList = buildItem.Where(bi =>
                {
                    return !examedList.Exists(ea => ea.BuildPartCode == bi.Code);
                }).ToList();
                // 随机其中一种，新建数据到Exam表
                int random = new Random().Next(buildList.Count);
                BuildItem build = buildList[random];

                exam.BuildPartId = build.Id;
                exam.BuildPartCode = build.Code;
                exam.BuildPartName = build.Name;
            }
            db.Exam.Add(exam);
            db.SaveChanges();
            if (data.Type == "虚拟测量")
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
                if (examType == "B")
                {
                    // 获取所有用户名
                    IEnumerable<string> names = data.GroupUsers.Select(gu => gu.Name);
                    // 获取所有用户ID
                    IEnumerable<int> ids = data.GroupUsers.Select(gu => gu.Id);
                    //组队试验，加上组长的名字
                    exam.GroupName = string.Format("{0},{1}", user.Name, string.Join(",", names));
                    //
                }
            }
            else if (data.Type == "测稿编绘")
            {
                // 添加EXAM_01_BILL表数据：01 构建表，02 虚拟照片，03 草图绘制
                List<Exam01Bill> bills1 = new List<Exam01Bill>();
                bills1.Add(new Exam01Bill
                {
                    ExamId = exam.Id,
                    Code = "01",
                    Name = "构建表"
                });

                bills1.Add(new Exam01Bill
                {
                    ExamId = exam.Id,
                    Code = "02",
                    Name = "虚拟照片"
                });

                bills1.Add(new Exam01Bill
                {
                    ExamId = exam.Id,
                    Code = "03",
                    Name = "草图绘制"
                });
                db.Exam01Bill.AddRange(bills1);
            }
            if (data.GroupUsers != null && data.GroupUsers.Count > 0)
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
                    bills
                }));
            }
            else
            {
                return Json(ResultInfo<Exam>.Success(exam));
            }
        }

        private int GetExamTimeByType(string type)
        {
            // 获取考试时间
            List<Dict> dicts = db.Dict.Where(d => d.Key == "EXAM_TIME_01" || d.Key == "EXAM_TIME_02").ToList();
            //
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
            int.TryParse(dict.Value, out time);
            return time;
        }

        [Route("getGroupUser/{login}")]
        [HttpGet]
        public ActionResult GetGroupUser(string login)
        {
            OrgUser leader = db.OrgUser.Where(ou => ou.Login == login && ou.Status == "有效").FirstOrDefault();
            if (leader == null)
            {
                return Json(ResultInfo<string>.Fail("1001", "组长信息获取失败！"));
            }
            // 根据组长的PAR_GUID获取所有可选择组员，不包含组长自己(默认已添加)
            List<OrgUser> users = db.OrgUser.Where(ou => ou.Login != login && ou.ParGuid == leader.ParGuid && ou.Status == "有效").ToList();
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
            string part = "";
            for (int i = 0, j = data.boms.Count; i < j; i++)
            {
                var bom = data.boms[i];
                if (!string.IsNullOrEmpty(bom.part))
                {
                    part = bom.part;
                }
                exam01Boms.Add(new Exam01Bom
                {
                    ExamId = data.ExamId,
                    Item = bom.item,
                    Part = part,
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
            // 循环工具组，判断是否有对应上工具组的工具集
            // 是否有正确答案，默认用户是错的，中间有一次正确，用户就正确。
            bool isRightAnswer = false;
            for (int i = 0, j = groups.Count; i < j; i++)
            {
                BuildSurveyGroup group = groups[i];
                // 首先，用户选择的工具的类型数量一定要对应上正确的工具的类型数量
                if (group.GroupTools == null)
                {
                    group.GroupTools = db.BuildSurveyGroupTool.Where(gt => gt.GroupId == group.Id).ToList();
                }
                if (data.tools.Count != group.GroupTools.Count)
                {
                    // 需要的工具类型数量都不一致，不可能正确，跳过此次循环。
                    continue;
                }
                // 循环用户选择的工具列表
                for (int a = 0; a < data.tools.Count; a++)
                {
                    var tool = data.tools[a];
                    //用户的工具类型和需要数量是否都在正确答案中？
                    bool isFind = group.GroupTools.Exists(t => t.ToolId == tool.Id && t.Count == tool.count);
                    if (!isFind)
                    {
                        // 不正确，跳出
                        break;
                    }
                    // 能走到这里，代表正确
                    isRightAnswer = true;
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

            if (!isRightAnswer)
            {
                // 不及格
                bill.Score = 0;
                exam.Score = 0;
                exam.Status = "不及格";
                exam.TimeEnd = DateTime.Now;
                db.SaveChanges();
                return Json(ResultInfo<Exam>.Success(exam));
            }

            // 正确，判断除此次BILL外是否所有BILL都有SCORE了
            bool isPass = !db.Exam02Bill.Where(eb => eb.ExamId == exam.Id && eb.Id != bill.Id).ToList().Exists(eb => eb.Score == null);
            bill.Score = 100;
            data.bill.Score = 100;

            if (isPass)
            {
                exam.Score = 100;
                exam.Status = "通过";
                exam.TimeEnd = DateTime.Now;
            }
            db.SaveChanges();
            if (isPass)
            {
                return Json(ResultInfo<Exam>.Success(exam));
            }
            return Json(ResultInfo<object>.Success(data));
        }
    }
}