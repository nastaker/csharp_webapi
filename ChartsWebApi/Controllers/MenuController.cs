using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using GetPDMObject;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    public class MenuController : Controller
    {
        // GET api/values
        [HttpGet]
        public ActionResult Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlGet xmlGet = new XmlGet
            {
                loginguid = loginGuid
            };
            List<XmlResultMenu> menulist = PDMUtils.getMenu(xmlGet);
            return Json(menulist);
        }

        // GET api/values/5/index
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            if (id == 0)
            {
                return NoContent();
            }
            return NoContent();
        }

        // POST api/values
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
            var result = PDMUtils.changeRole(xmlset);
            return Json(result);
        }


        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}