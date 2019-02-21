using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ChartsWebApi.ViewModel;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : Controller
    {
        // GET: api/data
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/data/5
        [HttpGet("{id}")]
        public void Get(int id)
        {
        }

        // POST api/data
        [HttpPost]
        public ActionResult Post([FromBody]JToken token)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            string jsonStr = JsonConvert.SerializeObject(token);
            XmlNode node = JsonConvert.DeserializeXmlNode(jsonStr, "OBJ");
            XmlGet xmlget = new XmlGet
            {
                loginguid = loginGuid,
                obj = node
            };
            var result = PDMUtils.getDataRows(xmlget);
            return Json(result);
        }

        // PUT api/data/
        [HttpPut]
        public ActionResult Put([FromBody]JToken token)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            string jsonStr = JsonConvert.SerializeObject(token);
            XmlNode node = JsonConvert.DeserializeXmlNode(jsonStr, "OBJ");
            XmlSet xmlset = new XmlSet
            {
                loginguid = loginGuid,
                obj = node
            };
            XmlResultData result = PDMUtils.modifyFormData(xmlset);
            return Json(result);
        }

        // DELETE api/data/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
