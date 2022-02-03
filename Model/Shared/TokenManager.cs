using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebAPI.Model.Response;
using Microsoft.AspNetCore.Http;

namespace WebAPI.Model
{
    public class TokenManager
    {
        private Appsetting _appSettings;

        public TokenManager(Appsetting appsettings)
        {
            //Setup appsettings.json 
            _appSettings = appsettings;
        }


        public string GenToken(UserResponse user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("userid", user.id.ToString()),
                    new Claim("country_code", user.country_code.ToString()),
                    new Claim("ph_no", user.ph_no.ToString()),
                    new Claim("user_role", user.role.ToString())
                    }),
                    Issuer = _appSettings.Issuer,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string RefreshToken(Token requset)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("userid", requset.userid.ToString()),
                    new Claim("country_code", requset.country_code.ToString()),
                    new Claim("ph_no", requset.ph_no.ToString()),
                    new Claim("user_role", requset.user_role.ToString())
                    }),
                    Issuer = _appSettings.Issuer,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Token GetClaimToken(HttpContext context)
        {
            Token claim = new Token();
            foreach (var item in context.User.Claims)
            {
                if (item.Type == "userid")
                {
                    claim.userid = item.Value;
                }
                else if (item.Type == "country_code")
                {
                    claim.country_code = item.Value;
                }
                else if(item.Type == "ph_no")
                {
                    claim.ph_no = item.Value;
                }
                else if(item.Type == "user_role")
                {
                    claim.user_role = item.Value;
                }
            }
            return claim;
        }
    }
}
