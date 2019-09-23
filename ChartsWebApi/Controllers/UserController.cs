using ChartsWebApi.Models;
using ChartsWebApi.Services;
using ChartsWebApi.ViewModel;
using ilabHelper;
using GetPDMObject;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Xml;
using System.IO;
using ChartsWebApi.models;
using System.Linq;
using System.Web;
using System.Net.Http;
using System;

namespace ChartsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;
        private XJwtSettings _xjwtSettings;
        private DataContext db;

        public UserController(IUserService userService, IOptions<XJwtSettings> _jwtSettingsAccesser, DataContext _db)
        {
            _userService = userService;
            _xjwtSettings = _jwtSettingsAccesser.Value;
            db = _db;
        }

        [Route("valid")]
        [HttpPost]
        public ActionResult Valid(GuestUser user)
        {
            XJWT xjwt = new XJWT(_xjwtSettings.Secret, _xjwtSettings.AesKey, _xjwtSettings.IssueId);
            ByteBuffer outbyte = ByteBuffer.Allocate(64);
            xjwt.verifyAndDecrypt(user.Token, outbyte, Utils.GetTimestamp());
            string resultStr = Utils.Trim(Encoding.UTF8.GetString(outbyte.GetBuffer()).Substring(8));
            XlibUser xuser = JsonConvert.DeserializeObject<XlibUser>(resultStr);
            var result = _userService.Authenticate(xuser.id, xuser.un, xuser.dis);
            return Json(result);
        }

        [Route("submitScore/{examid}/{attachid?}")]
        [HttpGet]
        public ActionResult SubmitScore(int examid, int? attachid)
        {
            //获取要提交的考试
            Exam exam = db.Exam.Where(e => e.Id == examid).SingleOrDefault();
            if (exam == null)
            {
                return Json(ResultInfo<object>.Fail("2001", "要提交成绩的考试不存在！"));
            }
            if (!exam.TimeEnd.HasValue)
            {
                return Json(ResultInfo<object>.Fail("2002", "考试尚未完成，无法上传成绩！"));
            }
            if (!exam.Score.HasValue)
            {
                return Json(ResultInfo<object>.Fail("2003", "考试成绩还未批改，无法上传成绩！"));
            }
            int userid = exam.UserId;
            OrgUser orgUser = db.OrgUser.Where(u => u.Id == userid).SingleOrDefault();
            if (orgUser == null)
            {
                return Json(ResultInfo<object>.Fail("1001", "用户不存在！"));
            }
            if (!orgUser.Login.EndsWith(_xjwtSettings.UserPad))
            {
                return Json(ResultInfo<object>.Fail("1002", "用户不属于实验空间用户，无法上传成绩！"));
            }
            long startDate = Utils.GetTimestamp(exam.TimeStart);
            long endDate = Utils.GetTimestamp(exam.TimeEnd.Value);
            int timeUsed = (int)(endDate - startDate) / 1000 / 60;
            if (timeUsed <= 0)
            {
                timeUsed = 1;
            }
            int score = decimal.ToInt32(exam.Score ?? 0);
            XlibUserScore userScore = new XlibUserScore
            {
                username = orgUser.Login.Replace(_xjwtSettings.UserPad, string.Empty),
                projectTitle = exam.BuildName,
                childProjectTitle = exam.BuildName,
                startDate = startDate,
                endDate = endDate,
                issuerId = _xjwtSettings.IssueId,
                score = score,
                status = exam.Status == "超时" ? 2 : 1,
                timeUsed = timeUsed
            };
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            string body = JsonConvert.SerializeObject(userScore, settings);
            XJWT xjwtObj = new XJWT(_xjwtSettings.Secret, _xjwtSettings.AesKey, _xjwtSettings.IssueId);
            string token = xjwtObj.createToken(body, XJWT.Type.JSON, Utils.GetTimestamp());
            string xjwt = HttpUtility.UrlEncode(token);
            HttpClient client = new HttpClient();
            string url = string.Format("{0}{1}?xjwt={2}", _xjwtSettings.Server, _xjwtSettings.UrlSubmitScore, xjwt);
            client.Timeout = TimeSpan.FromSeconds(10);
            var responseString = client.GetStringAsync(url);
            string str = responseString.Result;
            TestSubmitResult r = JsonConvert.DeserializeObject<TestSubmitResult>(str);
            switch(r.code)
            {
                case 0:
                    return Json(ResultInfo<string>.Success("已成功提交成绩！"));
                case 2:
                    return Json(ResultInfo<string>.Fail(r.code.ToString(), "解密失败"));
                case 3:
                case 4:
                case 5:
                case 6:
                default:
                    return Json(ResultInfo<string>.Fail(r.code.ToString(), r.msg));
            }
        }

        [Route("new")]
        [HttpPost]
        public ActionResult Add(GuestUser user)
        {
            // 这里需要模拟登录下
            ResultInfo<XmlResultUserLogin> admin = _userService.Authenticate("admin", "admin");

            XmlUserRegist xmlUserRegist = new XmlUserRegist
            {
                classname = "User",
                guid = "",
                type = "社会注册人员",
                parentClass = "GROUPS",
                parentGuid = "563A55BD-1D91-4956-A2B7-49F0A45F8264",
                login = user.Login,
                username = user.Username,
                password = user.Password,
                repeatPassword = user.Password,
                email = user.Email,
                phone = user.PhoneNumber
            };
            XmlNode node = PDMUtils.SerializeToXmlDocument(xmlUserRegist);
            XmlSet xmlset = new XmlSet
            {
                loginguid = admin.obj.loginguid,
                obj = node
            };
            // 新增一个记录
            var result = PDMUtils.modifyFormData(xmlset);
            // 返回结果
            return Json(result);
        }
    }
}