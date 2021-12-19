using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface IRegistrationRepository
    {
        Task<int> AddRoles(RolesResponse roles);
        Task<Object> GetRolesComplete();
        Task<Object> GetRoles();
        Task<int> SaveRolesByRegistration(RegistrationRegister register);
        Task<bool> GetUserByName(string name, string mobileNo);
        Task<int> SaveUserDetails(RegistrationRegister register);
        Task<int> Registration(RegistrationRegister register, string salt, string hashedPassword);
        Task<RegistrationRegister> GetUsername(string username);
        Task<int> ChangePassword(RegistrationChangePassword register, string salt, string hashedPassword, string oldPassword);
        Task<RegistrationRegister> VerifyAccount(ForgotPassword forgot);
        Task<int> ForgotPassword(ForgotPassword forgot, string pin);
        //Task<RegistrationRegister> VerifyEmailForgotPassword(VerifyForgotPassword forgot);
        Task<int> VerifyForgotPassword(VerifyForgotPassword forgot, string salt, string hashedPassword, string emailAddress);
        Task<IEnumerable<Object>> GetUser();
        Task<int> InsertUser(tbUser tbl);
        string GetIdentity(IIdentity identity);
        Task<Object> GetRolesCompleteById(Guid guid);
        Task<RegistrationRegister> GetUsernameLocked(string username);
        Task<List<RolesNewResponse>> GetRolesMainMenuById(Guid guid);
        Task<Object> GetRolesColumnExceptById(Guid guid);
        Task<int> DeleteRoles(Guid roleDetailsId);
        Task<int> UpdateDeleteRoles(Guid roleDetailsId);
        Task<int> UpdateAddRoles(UpdateRoleRequest roles);
    }
}
