using ChartsWebApi.Models;
using ChartsWebApi.Services;
using ChartsWebApi.ViewModel;
using GetPDMObject;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChartsWebApi.Controllers
{
    [EnableCors("CorsGuowenyan")]
    [Route("api/[controller]")]
    public class AuthorizeController : Controller
    {
        private IUserService _userService;

        public AuthorizeController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost]
        public IActionResult Post([FromBody]LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)//判断是否合法
            {
                return BadRequest();
            }
            XmlResultUserLogin user = _userService.Authenticate(loginViewModel.User, loginViewModel.Password);

            return Ok(user);
        }
    }
}