using ChartsWebApi.Extensions;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        // GET: api/Message/5
        [HttpGet]
        public ActionResult Get()
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            XmlGet xmlGet = new XmlGet
            {
                loginguid = loginguid
            }; 
            return Json(PDMUtils.getMessage(xmlGet));
        }
    }
}
