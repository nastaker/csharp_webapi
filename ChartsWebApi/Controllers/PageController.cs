﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Xml;
using ChartsWebApi.Extensions;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : Controller
    {
        // POST: api/Page
        [HttpPost]
        public ActionResult Post([FromBody] XmlSet xmlset)
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            xmlset.loginguid = loginguid;
            var result = PDMUtils.setAction(xmlset);
            return Json(result);
        }
    }
}
