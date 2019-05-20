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
    public class DownloadController : Controller
    {

        private IMemoryCache _cache;

        public DownloadController(IMemoryCache cache)
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

        // POST: api/Download
        [Authorize]
        [HttpPost]
        public ActionResult Post([FromBody] JToken token)
        {
            string loginguid = string.Empty;
            HttpContext.Request.Cookies.TryGetValue("loginguid", out loginguid);
            string jsonStr = JsonConvert.SerializeObject(token);
            XmlDocument doc = JsonConvert.DeserializeXmlNode(jsonStr, "SET");
            XmlElement root = doc.DocumentElement;
            XmlElement elemLoginguid = doc.CreateElement("LOGINGUID");
            elemLoginguid.InnerText = loginguid;
            root.AppendChild(elemLoginguid);
            ResultInfo<XmlResultFile> result = PDMUtils.getDownload(doc);
            // 保存到Session中，然后返回session的key，也就是随机guid
            string key = Guid.NewGuid().ToString("N");
            _cache.Set(key, result);
            return Ok(new ResultInfo<string> {
                code = "0",
                obj = key
            });
        }
    }
}
