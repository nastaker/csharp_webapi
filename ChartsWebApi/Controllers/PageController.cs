using System.Collections.Generic;
using System.Security.Claims;
using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsChartNodeClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : Controller
    {
        // GET: api/Page
        [HttpGet("{type}", Name = "GetPageType")]
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
            XmlGetForm xmlCreateForm = new XmlGetForm
            {
                loginguid = loginGuid,
                obj = new XmlSetForm
                {
                    classname = type
                }
            };
            rtn = PDMUtils.getModifyForm(xmlCreateForm);
            return Json(rtn);
        }

        // GET: api/Page/5
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
            XmlGetForm xmlModForm = new XmlGetForm
            {
                loginguid = loginGuid,
                obj = new XmlSetForm
                {
                    classname = type,
                    guid = guid
                }
            };
            rtn = PDMUtils.getModifyForm(xmlModForm);
            return Json(rtn);
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
