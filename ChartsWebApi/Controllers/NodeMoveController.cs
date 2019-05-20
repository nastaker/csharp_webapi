using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class NodeMoveController : Controller
    {
        // POST api/data
        [HttpPost]
        public ActionResult Post([FromBody]JToken token)
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            string jsonStr = JsonConvert.SerializeObject(token);
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonStr, "SET");
            XmlElement root = doc.DocumentElement;
            XmlElement elemLoginguid = doc.CreateElement("LOGINGUID");
            elemLoginguid.InnerText = loginguid;
            root.AppendChild(elemLoginguid);
            var result = PDMUtils.getString(doc);
            return Json(result);
        }
    }
}