using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Security.Principal;
using SISystem.Models;
using SISystem.Data;

namespace SISystem.Services
{
    public class AuthTokenServices : IAuthTokenServices
    {
        private readonly SISystemDbContext _sISystemDbContext;
        private readonly IFunctionsRepository _functionsRepository;
        public IConfiguration Configuration { get; }
        public string BasicScheme { get; private set; }

        public AuthTokenServices(
            SISystemDbContext sISystemDbContext, 
            IConfiguration configuration,
            IFunctionsRepository functionsRepository)
        {
            _sISystemDbContext = sISystemDbContext;
            Configuration = configuration;
            _functionsRepository = functionsRepository;
        }


        #region Generate access token
        public string GenerateToken(string userName, string id, Guid guid)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, guid.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"]));
            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            int expireMinutes = Convert.ToInt32(Configuration["Token:ExpireMinutes"]);
            var token = new JwtSecurityToken(
                 issuer: Configuration["Token:Issuer"],
                 audience: Configuration["Token:Audience"],
                 expires: DateTime.Now.AddMinutes(expireMinutes),
                 claims: claims,
                 signingCredentials: signInCred
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
        #endregion

        #region Generate refresh token
        public string GenerateRefreshToken(string userName, string id)
        {
            var claims = new[]
           {
                new Claim(ClaimTypes.Name, userName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"]));
            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            int refreshExpireMinutes = Convert.ToInt32(Configuration["Token:RefreshExpireMinutes"]);
            var token = new JwtSecurityToken(
                 issuer: Configuration["Token:Issuer"],
                 expires: DateTime.Now.AddMinutes(refreshExpireMinutes),
                 claims: claims,
                 signingCredentials: signInCred
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
        #endregion

        #region Generate claims
        public object GenerateClaims(string userName, string id, ConfigurationRoleList roles)
        {
            var claims = new[]
{
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, roles.RoleSubName)
            };
           
            return claims;
        }
        #endregion
    }
}
