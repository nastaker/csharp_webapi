using System.Collections.Generic;
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
        [HttpGet("{id}")]
        public void Get(string id)
        {
        }

        // POST: api/Page
        [HttpPost]
        public ActionResult Post([FromBody] XmlSet xmlset)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            xmlset.loginguid = loginGuid;
            var result = PDMUtils.setAction(xmlset);
            return Json(result);
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
