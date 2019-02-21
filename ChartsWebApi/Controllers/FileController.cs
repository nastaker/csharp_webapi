using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;

namespace ChartsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IMemoryCache _cache;

        public FileController(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        // GET api/file
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            XmlResultFile file;
            if (_cache.TryGetValue(id, out file))
            {
                if (System.IO.File.Exists(file.path))
                {
                    var provider = new FileExtensionContentTypeProvider();
                    var suffix = file.path.Substring(file.path.LastIndexOf('.'));
                    string contentType;
                    if (!provider.TryGetContentType(file.path, out contentType))
                    {
                        contentType = "application/octet-stream";
                    }
                    if (file.type == "downfile")
                    {
                        return File(new FileStream(file.path, FileMode.Open), contentType, file.name + suffix);
                    }
                    else
                    {
                        return File(new FileStream(file.path, FileMode.Open), contentType);
                    }
                }
                else
                {
                    return Content("文件已经被移动或删除");
                }
            }
            return Content("链接已过期");
        }

        // POST api/file
        [Authorize]
        [EnableCors("CorsGuowenyan")]
        [HttpPost]
        [DisableRequestSizeLimit]
        public ActionResult Post()
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
        [HttpPut]
        public void Put()
        {
        }

        // DELETE api/file/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
