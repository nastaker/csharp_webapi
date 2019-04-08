using ChartsWebApi.Extensions;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Xml;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : Controller
    {
        // POST api/data
        [HttpPost]
        public ActionResult Post([FromBody]JToken token)
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            string jsonStr = JsonConvert.SerializeObject(token);
            string action = token.Value<string>("action");
            XmlNode node = JsonConvert.DeserializeXmlNode(jsonStr, "OBJ");
            XmlGet xmlget = new XmlGet
            {
                loginguid = loginguid,
                action = action,
                obj = node
            };
            var result = PDMUtils.getDataRows(xmlget);
            return Json(result);
        }

        // PUT api/data/
        [HttpPut]
        public ActionResult Put([FromBody]JToken token)
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            string jsonStr = JsonConvert.SerializeObject(token);
            XmlNode node = JsonConvert.DeserializeXmlNode(jsonStr, "OBJ");
            XmlSet xmlset = new XmlSet
            {
                loginguid = loginguid,
                obj = node
            };
            XmlResultData result = PDMUtils.modifyFormData(xmlset);
            return Json(result);
        }
    }
}
