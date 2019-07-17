using ChartsWebApi.Services;
using ChartsWebApi.ViewModel;
using GetPDMObject;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml;

namespace ChartsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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