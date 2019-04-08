using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChartsWebApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace ChartsWebApi.Services
{
    public interface ITokenCreationService
    {
        string CreateToken(IEnumerable<Claim> claims);
    }

    public class TokenCreationService : ITokenCreationService
    {
        private JwtSettings _jwtSettings;

        public TokenCreationService(IOptions<JwtSettings> _jwtSettingsAccesser)
        {
            _jwtSettings = _jwtSettingsAccesser.Value;
        }

        public string CreateToken(IEnumerable<Claim> claims)
        {
            //对称秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            //签名证书(秘钥，加密算法)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //生成token
            var jwt = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims, DateTime.Now, DateTime.Now.AddMinutes(_jwtSettings.Expires), creds);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
