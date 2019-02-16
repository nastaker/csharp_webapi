using Microsoft.AspNetCore.Mvc;
using ChartsWebApi.ViewModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ChartsWebApi.Controllers
{
    [Authorize]
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET api/file
        [HttpGet]
        public void Get()
        {
        }

        // POST api/file
        [HttpPost]
        public ActionResult Post([FromForm]MyModel myModel)
        {
            // 如果有文件，这里上传
            var file = Request.Form.Files[0];
            string folderName = "Upload";
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string guid = Guid.NewGuid().ToString("N");
                string suffix = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                string newFileName = guid + suffix;
                string fullPath = Path.Combine(newPath, newFileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Content(Path.Combine(folderName, newFileName));
            }
            return NoContent();
        }

        // PUT api/file/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/file/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
