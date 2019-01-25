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
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
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
            XmlResultUserRole role = PDMUtils.getRole(loginGuid);
            return Json(role);
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
        public void Post([FromBody]string value)
        {
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