﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChartsWebApi.Models;
using ChartsWebApi.ViewModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChartsWebApi.Controllers
{
    [EnableCors("CorsChartNodeClient")]
    [Route("api/[controller]")]
    public class AuthorizeController : Controller
    {
        private JwtSettings _jwtSettings;

        public AuthorizeController(IOptions<JwtSettings> _jwtSettingsAccesser)
        {
            _jwtSettings = _jwtSettingsAccesser.Value;
        }
        
        [HttpPost]
        public IActionResult Post([FromBody]LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)//判断是否合法
            {
                return BadRequest();
            }
            if (!(viewModel.User == "openinfo" && viewModel.Password == "123123"))//判断账号密码是否正确
            {
                return BadRequest();
            }
            var claim = new Claim[]{
                new Claim(ClaimTypes.Name,"openinfo"),
                new Claim(ClaimTypes.Role,"admin")
            };
            //对称秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            //签名证书(秘钥，加密算法)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //生成token
            var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claim, DateTime.Now, DateTime.Now.AddMinutes(30), creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}