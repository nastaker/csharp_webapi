using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Newtonsoft.Json.Linq;
using ChartsWebApi.ViewModel;
using Microsoft.AspNetCore.Cors;

namespace ChartsWebApi.Controllers
{
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5/index
        [HttpGet("{id}")]
        public void Get(int id, string index)
        {
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
