using GetPDMObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Xml;

namespace ChartsWebApi.Controllers
{
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : Controller
    {

        private IMemoryCache _cache;

        public ExportController(IMemoryCache cache)
        {
            _cache = cache;
        }

        // GET api/file
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            ResultInfo<XmlResultFile> result;
            if (_cache.TryGetValue(id, out result))
            {
                XmlResultFile file = result.obj;
                if (System.IO.File.Exists(file.path))
                {
                    var provider = new FileExtensionContentTypeProvider();
                    var suffix = file.path.Substring(file.path.LastIndexOf('.'));
                    string contentType;
                    if (!provider.TryGetContentType(file.path, out contentType))
                    {
                        contentType = "application/octet-stream";
                    }
                    return File(new FileStream(file.path, FileMode.Open), contentType, file.name + suffix);
                }
                return Content("文件已经被移动或删除");
            }
            return Content("链接已过期");
        }

        // POST: api/Download
        [Authorize]
        [HttpPost]
        public ActionResult Post([FromBody] JToken token)
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            string jsonStr = JsonConvert.SerializeObject(token);
            string action = token.Value<string>("action");
            XmlNode node = JsonConvert.DeserializeXmlNode(jsonStr, "OBJ");
            XmlGet xmlget = new XmlGet
            {
                loginguid = loginguid,
                action = action,
                obj = node
            };
            var result = PDMUtils.getExportData(xmlget);
            // 保存到Session中，然后返回session的key，也就是随机guid
            string key = Guid.NewGuid().ToString("N");
            _cache.Set(key, result);
            return Ok(new ResultInfo<string>
            {
                code = "0",
                obj = key
            });
        }
    }
}