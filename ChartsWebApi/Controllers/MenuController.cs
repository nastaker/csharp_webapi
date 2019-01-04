using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using GetPDMObject;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsChartNodeClient")]
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
            List<XmlMenu> menulist = PDMUtils.getMenu(loginGuid);
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