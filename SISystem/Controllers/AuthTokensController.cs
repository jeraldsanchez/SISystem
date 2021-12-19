using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISystem.Models;
using SISystem.Services;

namespace SISystem.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/SISystem/[controller]")]
    public class AuthTokensController : Controller
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IAuthTokenServices _authTokenServices;
        private readonly IFunctionsRepository _functionsRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly ITokenServices _tokenServices;

        public AuthTokensController(
            IRegistrationRepository registrationRepository, 
            IFunctionsRepository functionsRepository,
            ICustomerInformationServices customerServices, 
            IAuthTokenServices authTokenServices,
            ITokenServices tokenServices)
        {
            _registrationRepository = registrationRepository;
            _authTokenServices = authTokenServices;
            _functionsRepository = functionsRepository;
            _customerServices = customerServices;
            _tokenServices = tokenServices;
        }
        #region Login
        [HttpPost("login/FacebookGoogle")]
        //[ValidateModel]
        public IActionResult LoginViaThirdParty([FromBody] Login login)
        {
            return Ok();
        }
        #endregion


        #region Login
        [HttpPost("login")]
        //[ValidateModel]
        public async Task<object> Login([FromBody] Login login)
        {
            try
            {
                RegistrationRegister user = await _registrationRepository.GetUsername(login.userName);

                if (user != null)
                {
                    string oldPassword = _functionsRepository.CreateSaltHashPassword(login.password, user.salt);
                    if (oldPassword == user.password )
                    {
                        //RegistrationRegister companyId = await _registrationRepository.GetCompanyId(user.userName);
                        ConfigurationRoleList roles = new ConfigurationRoleList(); //await _customerServices.GetRoles(user.UserId);
                        var roleId = await _customerServices.GetRolesId(user.UserId);
                        string token = _authTokenServices.GenerateToken(login.userName, user.UserId.ToString(), roleId);
                        string refreshToken = _authTokenServices.GenerateRefreshToken(login.userName, user.UserId.ToString());

                        TokenServiceGenerateToken tokenServ = new TokenServiceGenerateToken() {
                            Username = user.userName,
                            AccessToken = token,
                            RefreshedAccessToken = refreshToken
                        };
                        int saveToken = await _tokenServices.UserToken(tokenServ);

                        Auth auth = new Auth {
                            accessToken = token,
                            refreshToken = refreshToken
                        };
                        await _customerServices.LogInEmployee(user.UserId);
                        await _customerServices.Logs(login.userName, user.UserId, "success", "", "AuthTokens.Login");
                        return Ok(auth);
                    }
                }
                await _customerServices.Logs(login.userName, null, "error", "", "AuthTokens.Login");
                return BadRequest(new { Message = "Either user name or password is incorrect or locked" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.ToString() });
            }
        }
        #endregion

        #region Logout
        [HttpPut("logout")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<object> Logout([FromBody] Logout logout)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    int saveToken = await _tokenServices.RevokeToken(logout, user.userName);
                    await _customerServices.LogOutEmployee(user.UserId);
                    await _customerServices.Logs(user.userName, user.UserId, "success", "", "AuthTokens.Logout");
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.ToString() });
            }
        }
        #endregion

        #region Refresh Token
        [HttpPost("refresh")]
        //[ValidateModel]
        public async Task<object> RefreshToken([FromBody] CheckRefreshToken refresh)
        {
            try
            {
                RegistrationRegister user = await _registrationRepository.GetUsername(refresh.userName);
                bool check = await _tokenServices.CheckRefreshToken(refresh);
                if (check)
                {
                    //ConfigurationRoleList roles = await _customerServices.GetRoles(user.UserId);
                    var roleId = await _customerServices.GetRolesId(user.UserId);
                    string token = _authTokenServices.GenerateToken(refresh.userName, user.UserId.ToString(), roleId);
                    string refreshToken = _authTokenServices.GenerateRefreshToken(refresh.userName, user.UserId.ToString());

                    TokenServiceGenerateToken tokenServ = new TokenServiceGenerateToken()
                    {
                        Username = user.userName,
                        AccessToken = token,
                        RefreshedAccessToken = refreshToken
                    };
                    int saveToken = await _tokenServices.UserToken(tokenServ);
                    Auth auth = new Auth
                    {
                        accessToken = token,
                        refreshToken = refreshToken
                    };
                    return Ok(auth);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Something went wrong" });
            }
        }
        #endregion

        [HttpPut("lock-account/")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> LockAccount()
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _customerServices.LockAccount(user.UserId);
                    await _customerServices.Logs(userName, user.UserId, "success", "Updated: " + user.userName, "LockAccount.Put");
                    return Accepted();
                }
                await _customerServices.Logs(userName, null, "error", "", "LockAccount.Put");
                return BadRequest();
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "LockAccount.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("unlock-account/{employee}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> UnLockAccount(string employee)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    RegistrationRegister user2 = await _registrationRepository.GetUsernameLocked(employee);
                    await _customerServices.UnLockAccount(user2.UserId);
                    await _customerServices.Logs(userName, user.UserId, "success", "Updated: " + user2.userName, "UnLockAccount.Put");
                    return Accepted();
                }
                await _customerServices.Logs(userName, null, "error", "", "UnLockAccount.Put");
                return BadRequest();
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UnLockAccount.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
    }
}