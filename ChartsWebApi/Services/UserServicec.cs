using ChartsWebApi.Models;
using GetPDMObject;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChartsWebApi.Services
{
    public interface IUserService
    {
        ResultInfo<XmlResultUserLogin> Authenticate(string username, string password);
    }

    public class UserService : IUserService
    {
        private readonly JwtSettings _jwtSettings;

        public UserService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public ResultInfo<XmlResultUserLogin> Authenticate(string username, string password)
        {
            ResultInfo<XmlResultUserLogin> result = PDMUtils.login(username, password, "chpdms");
            var user = result.obj;
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.username)
                }),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.Expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.token = tokenHandler.WriteToken(token);

            return result;
        }
    }
}
