using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;

namespace ChartsWebApi.Controllers
{
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    public class ChartsController : Controller
    {
        // GET api/values
        [HttpGet]
        public List<Function> Get()
        {
            ChartDbContext dbcontext = new ChartDbContext();
            return dbcontext.Functions.ToList();
        }

        // GET api/values/5/index
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            if(id == 0)
            {
                return NoContent();
            }
            ChartDbContext dbcontext = new ChartDbContext();
            var model = dbcontext.Functions.Where(f => f.Id == id).SingleOrDefault();
            if(model == null)
            {
                return NoContent();
            }
            return Json(model);
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