﻿using ChartsWebApi.Extensions;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
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
    public class ActionController : Controller
    {
        // POST: api/Action
        [HttpPost]
        public ActionResult Post([FromBody] JToken token)
        {
            string jsonStr = JsonConvert.SerializeObject(token);
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonStr, "SET");
            XmlElement root = doc.DocumentElement;
            XmlElement elemLoginguid = doc.CreateElement("LOGINGUID");
            elemLoginguid.InnerText = loginguid;
            root.AppendChild(elemLoginguid);
            var result = PDMUtils.setAction(doc);
            return Json(result);
        }
    }
}
