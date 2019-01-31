using System.Collections.Generic;
using System.Security.Claims;
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
    public class ActionController : Controller
    {
        // GET: api/Action
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Action/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Action
        [HttpPost]
        public ActionResult Post([FromBody] JToken token)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            string jsonStr = JsonConvert.SerializeObject(token);
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonStr, "SET");
            XmlElement root = doc.DocumentElement;
            XmlElement elemLoginguid = doc.CreateElement("LOGINGUID");
            elemLoginguid.InnerText = loginGuid;
            root.AppendChild(elemLoginguid);
            var result = PDMUtils.setAction(doc);
            return Json(result);
        }

        // PUT: api/Action/5
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
