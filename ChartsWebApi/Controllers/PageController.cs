﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Xml;
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
        // GET: api/Page
        [HttpGet()]
        public void Get()
        {
        }

        // GET: api/Page/5
        [HttpGet("{pageid}")]
        public ActionResult Get(string pageid)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("OBJ");
            XmlElement guid = doc.CreateElement("GUID");
            guid.InnerText = pageid;
            root.AppendChild(guid);
            doc.AppendChild(root);
            XmlGetForm xmlGetForm = new XmlGetForm
            {
                loginguid = loginGuid,
                obj = doc
            };
            var result = PDMUtils.getDataDef(xmlGetForm);
            return Json(result);
        }

        // POST: api/Page
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Page/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
