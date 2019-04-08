using ChartsWebApi.Extensions;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Xml;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class TreeController : Controller
    {
        // POST: api/Tree
        [HttpPost]
        public ActionResult Post([FromBody]JToken token)
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            var jsonStr = JsonConvert.SerializeObject(token);
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonStr, "GET");
            XmlElement root = doc.DocumentElement;
            XmlElement elemLoginguid = doc.CreateElement("LOGINGUID");
            elemLoginguid.InnerText = loginguid;
            root.AppendChild(elemLoginguid);
            var result = PDMUtils.getTreeNodes(doc);
            return Json(result);
        }
    }
}
