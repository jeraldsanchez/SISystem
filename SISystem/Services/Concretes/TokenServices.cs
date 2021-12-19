using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly SISystemDbContext _sISystemDbContext;
        public IConfiguration Configuration { get; }

        public TokenServices(SISystemDbContext sISystemDbContext, IConfiguration configuration)
        {
            _sISystemDbContext = sISystemDbContext;
            Configuration = configuration;
        }

        #region Insert Token
        public async Task<int> UserToken(TokenServiceGenerateToken token)
        {
            OAuthTbl oAuth = new OAuthTbl {
                Username = token.Username,
                AccessToken = token.AccessToken,
                RefreshedAccessToken = token.RefreshedAccessToken,
                ExpirationDate = DateTime.Now.AddMinutes(Convert.ToInt32(Configuration["Token:ExpireMinutes"])),
                RefreshedExpirationDate = DateTime.Now.AddMinutes(Convert.ToInt32(Configuration["Token:RefreshExpireMinutes"])),
                DateCreated = DateTime.Now
            };
            _sISystemDbContext.Add(oAuth);
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Revoke Token
        public async Task<int> RevokeToken(Logout logout, string userName)
        {
            var auth = _sISystemDbContext.OAuthTbl.FirstOrDefault(item => item.Username == userName && item.AccessToken == logout.accessToken);
            if (auth != null)
            {
                auth.RefreshedAccessToken = null;
                _sISystemDbContext.Update(auth);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Check if refresh Token
        public async Task<bool> CheckRefreshToken(CheckRefreshToken token)
        {
            bool checkIfExist = false;
            var auth = await _sISystemDbContext.OAuthTbl.FirstOrDefaultAsync(item => item.Username == token.userName && item.AccessToken == token.accessToken && item.RefreshedAccessToken == token.refreshToken && item.RefreshedExpirationDate >= DateTime.Now);
            if (auth != null)
            {
                checkIfExist = true;
            }
            return checkIfExist;
        }
        #endregion
    }
}
