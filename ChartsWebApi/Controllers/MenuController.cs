using ChartsWebApi.Extensions;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    public class MenuController : Controller
    {
        // GET api/values
        [HttpGet]
        public ActionResult Get()
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            XmlGet xmlGet = new XmlGet
            {
                loginguid = loginguid
            };
            var result = PDMUtils.getMenu(xmlGet);
            return Json(result);
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] XmlSet xmlset)
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            xmlset.loginguid = loginguid;
            var result = PDMUtils.changeRole(xmlset);
            return Json(result);
        }
    }
}