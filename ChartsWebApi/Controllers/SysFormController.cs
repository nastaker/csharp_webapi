using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public class SysFormController : Controller
    {
        // GET: api/SysForm
        [HttpGet]
        public ActionResult Get(string type)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlResultForm rtn = null;
            XmlGetForm xmlCreateForm = new XmlGetForm
            {
                loginguid = loginGuid,
                obj = new XmlGetDataRows
                {
                    classname = type
                }
            };
            rtn = PDMUtils.getModifyForm(xmlCreateForm);
            return Json(rtn);
        }

        // GET: api/SysForm/5
        [HttpGet("{type}/{guid}", Name = "GetPageTypeEdit")]
        public ActionResult Get(string type, string guid)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginGuid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlResultForm rtn = null;
            XmlGetForm xmlModForm = new XmlGetForm
            {
                loginguid = loginGuid,
                obj = new XmlGetDataRows
                {
                    classname = type,
                    guid = guid
                }
            };
            rtn = PDMUtils.getModifyForm(xmlModForm);
            return Json(rtn);
        }

        // POST: api/SysForm
        [HttpPost]
        public ActionResult Post([FromBody]ViewModel.FormData form)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginguid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlSet xmlset = new XmlSet
            {
                loginguid = loginguid,
                desc = "",
                obj = new XmlSetFormData
                {
                    classname = form.type,
                    guid = form.CN_GUID,
                    name = form.CN_NAME,
                    desc = form.CN_DESC,
                    login = form.CN_LOGIN,
                    email = form.CN_EMAIL,
                    entrydate = form.CN_ENTRYDAY,
                    gender = form.CN_SEX,
                    isLeader = form.CN_LEADER ? 1 : 0,
                    password = form.CN_PSW,
                    phone = form.CN_PHONE,
                    usercode = form.CN_USERCODE
                }
            };
            if (!string.IsNullOrWhiteSpace(form.parentguid))
            {
                xmlset.parent = new XmlSetTypeParent
                {
                    obj = new XmlSetType
                    {
                        classname = form.parenttype,
                        guid = form.parentguid
                    }
                };
            }
            XmlResult result = PDMUtils.modifyFormData(xmlset);
            return Json(result);
        }

        // PUT: api/SysForm/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{type}/{guid}")]
        public ActionResult Delete(string type, string guid)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            IEnumerable<Claim> claims = identity.Claims;
            string loginguid = identity.FindFirst(ClaimTypes.Hash).Value;
            XmlSet xmlset = new XmlSet
            {
                loginguid = loginguid,
                desc = "",
                obj = new XmlSetType
                {
                    classname = type,
                    guid = guid,
                }
            };
            XmlResult result = PDMUtils.delete(xmlset);
            return Json(result);
        }
    }
}
