using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class SysFormController : Controller
    {
        // GET: api/SysForm
        [HttpGet("{type}")]
        public ActionResult Get(string type)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlResultForm rtn = null;
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("OBJ");
            root.SetAttribute("CLASSNAME", type);
            doc.AppendChild(root);
            XmlGetForm xmlCreateForm = new XmlGetForm
            {
                loginguid = loginGuid,
                obj = doc
            };
            rtn = PDMUtils.getModifyForm(xmlCreateForm);
            return Json(rtn);
        }

        // GET: api/SysForm/5
        [HttpGet("{type}/{guid}", Name = "GetPageTypeEdit")]
        public ActionResult Get(string type, string guid)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlResultForm rtn = null;
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("OBJ");
            XmlElement eleGuid = doc.CreateElement("GUID");
            root.SetAttribute("GUID", guid);
            root.AppendChild(eleGuid);
            eleGuid.InnerText = guid;
            root.SetAttribute("CLASSNAME", type);
            doc.AppendChild(root);
            XmlGetForm xmlModForm = new XmlGetForm
            {
                loginguid = loginGuid,
                obj = doc
            };
            rtn = PDMUtils.getModifyForm(xmlModForm);
            return Json(rtn);
        }

        // POST: api/SysForm
        [HttpPost]
        public ActionResult Post([FromBody]JsonToken token)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginguid = identity.FindFirst(ClaimTypes.Hash).Value;
            string jsonStr = JsonConvert.SerializeObject(token);
            XmlNode node = JsonConvert.DeserializeXmlNode(jsonStr, "OBJ");
            XmlSet xmlset = new XmlSet
            {
                loginguid = loginguid,
                desc = "",
                obj = node
            };
            XmlResult result = PDMUtils.modifyFormData(xmlset);
            return Json(result);
        }

        // PUT: api/SysForm/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{type}/{guid}")]
        public ActionResult Delete(string type, string guid)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginguid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlSet xmlset = new XmlSet
            {
                loginguid = loginguid,
                desc = ""
            };
            XmlResult result = PDMUtils.delete(xmlset);
            return Json(result);
        }
    }
}
