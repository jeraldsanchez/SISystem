using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISystem.Data;
using SISystem.Models;
using SISystem.Services;

namespace SISystem.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/SISystem/[controller]")]

    public class RegistrationsController : Controller
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IFunctionsRepository _functionsRepository;
        private readonly ICustomerInformationServices _customerServices;

        public RegistrationsController(
            IRegistrationRepository registrationRepository, 
            IFunctionsRepository functionsRepository ,
            ICustomerInformationServices customerServices
            )
        {
            _registrationRepository = registrationRepository;
            _functionsRepository = functionsRepository;
            _customerServices = customerServices;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserList()
        {
            return Accepted(await _registrationRepository.GetUser());
        }

        #region Roles
        [HttpPost("Role")]
        [Authorize]
        public async Task<IActionResult> SaveNewRoles([FromBody] RolesResponse roles)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _registrationRepository.AddRoles(roles);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SaveNewRoles.Post");
                    return Accepted(new { Message = "Role Registration successful" });
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveNewRoles.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Role")]
        [Authorize]
        public async Task<IActionResult> UpdateRoles([FromBody] UpdateRoleRequest roles)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _registrationRepository.UpdateDeleteRoles(roles.RoleDetailsId);
                    await _registrationRepository.UpdateAddRoles(roles);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "UpdateRoles.Put");
                    return Accepted(new { Message = "Role Registration successful" });
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UpdateRoles.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Role")]
        [Authorize]
        public async Task<IActionResult> GetRoleList()
        {
            return Accepted(await _registrationRepository.GetRolesComplete());
        }

        [HttpGet("Registered/Role")]
        [Authorize]
        public async Task<IActionResult> GetRoleListForRegistration()
        {
            return Accepted(await _registrationRepository.GetRoles());
        }

        [HttpGet("Role/Details")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRoleDetails()
        {
            try
            {
                var userName = User.Identity.Name;
                var identity = _registrationRepository.GetIdentity(User.Identity);
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    return Accepted(await _registrationRepository.GetRolesCompleteById(Guid.Parse(identity)));
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetRoleDetails.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Role/MainMenu")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRoleMainDetails()
        {
            try
            {
                var userName = User.Identity.Name;
                var identity = _registrationRepository.GetIdentity(User.Identity);
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    return Accepted(await _registrationRepository.GetRolesMainMenuById(Guid.Parse(identity)));
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetRoleMainDetails.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Role/MainMenu/ExceptColumn/{roleId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRolesColumnExceptByIdMethod(Guid roleId)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    return Accepted(await _registrationRepository.GetRolesColumnExceptById(roleId));
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetRolesColumnExceptByIdMethod.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpDelete("Role/{roleDetailsId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RemoveRole(Guid roleDetailsId)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _registrationRepository.DeleteRoles(roleDetailsId);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "RemoveRole.Delete");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "RemoveRole.Delete");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
        #endregion

        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Registration([FromBody] RegistrationRegister register)
        {
            try
            {
                RegistrationRegister user = await _registrationRepository.GetUsername(register.userName);
                bool userDetails = await _registrationRepository.GetUserByName(register.Name, register.MobileNo);
                if (user == null && userDetails == false)
                {
                    string salt = _functionsRepository.CreateSalt(6);
                    string password = _functionsRepository.CreateSaltHashPassword(register.password, salt);
                    int rowsAffected = _registrationRepository.Registration(register, salt, password).Result;
                    RegistrationRegister userId = await _registrationRepository.GetUsername(register.userName);
                    register.UserId = userId.UserId;
                    await _registrationRepository.SaveRolesByRegistration(register);
                    await _registrationRepository.SaveUserDetails(register);
                    int rowLogs = await _customerServices.Logs(userId.userName, userId.UserId, "success", "", "Customer.Registration");
                    return Accepted(new { Message = "Registration successful" });
                }
                else
                {
                    await _customerServices.Logs(register.userName, null, "error", "Account already exist", "Customer.Registration");
                    return BadRequest(new { Message = "Either mobile No, Username, Name or Email Address already exist" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.ToString() });
            }
        }
        #endregion

        #region Change Password
        [HttpPut("change-password")]
        //[ValidateModel]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] RegistrationChangePassword register)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(register.userName);
                if (user != null)
                {
                    string oldPassword = _functionsRepository.CreateSaltHashPassword(register.oldPassword, user.salt);
                    string newPass = _functionsRepository.CreateSaltHashPassword(register.newPassword, user.salt);
                    string salt = _functionsRepository.CreateSalt(6);
                    string password = _functionsRepository.CreateSaltHashPassword(register.newPassword, salt);

                    if (oldPassword == user.password && oldPassword != newPass)
                    {
                        int rowsAffected = _registrationRepository.ChangePassword(register, salt, password, oldPassword).Result;
                        await _customerServices.Logs(register.userName, user.UserId, "success", "", "Customer.ChangePassword");
                        return Accepted(new { Message = "Password successfully changed" });
                    }
                }
                await _customerServices.Logs(register.userName, user.UserId, "error", "Account doesn't exist or incorrect old password", "Customer.ChangePassword");
                return BadRequest(new { Message = "Account doesn't exist or incorrect old password" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = "Something went wrong" });
            }
        }
        #endregion

        #region Forgot Password
        [HttpPost("forgot-password")]
        //[ValidateModel]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword forgot)
        {
            try
            {
                //var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.VerifyAccount(forgot);
                if (user != null)
                {
                    string pin = _functionsRepository.CreatePin(9);
                    int? result = await _registrationRepository.ForgotPassword(forgot, pin);
                    //Email here

                    await _customerServices.Logs(forgot.userName, user.UserId, "success", "", "Customer.ForgotPassword");
                    return Accepted(new { Message = "Email sent successfully" });
                }
                return NotFound(new { Message = "Account not found" });
            }
            catch (Exception e)
            {
                return BadRequest(new { Message = e.ToString() });
            }
        }
        #endregion

        [HttpPost("logs")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLogs([FromBody] LogsRequest request)
        {
            try
            {
                var userName = User.Identity.Name;
                var testing = _registrationRepository.GetIdentity(User.Identity);
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null){
                    List<LogsResponse> logsResponse = new List<LogsResponse>();
                    if (request.UserName.ToLower() == "all"){
                        logsResponse = await _customerServices.GetAllLogs(request.FromDate, request.ToDate);
                    }
                    else{
                        logsResponse = await _customerServices.GetLogs(request.UserName, request.FromDate, request.ToDate);
                    }
                    return Accepted(logsResponse);
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetLogs.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("employee/logs")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetEmployeeLogs([FromBody] EmployeeLogsRequest request)
        {
            try
            {
                var userName = User.Identity.Name;
                var testing = _registrationRepository.GetIdentity(User.Identity);
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    return Accepted(await _customerServices.GetEmployeeLogs(request.FromDate, request.ToDate, user.UserId));
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetEmployeeLogs.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

    }
}