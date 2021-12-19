using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface ICustomerInformationServices
    {
        Task<int> UserDetails(UserDetails register, RegistrationRegister user);
        Task<int> UserRole(RegistrationRegister register);
        Task<int> Logs(string userName, Guid? userId, string response, string responseDetails, string transactionType);
        Task<int> LogInEmployee(Guid? userId);
        Task<int> LogOutEmployee(Guid? userId);
        Task<List<EmployeeLogsResponse>> GetEmployeeLogs(DateTime fromDate, DateTime toDate, Guid? userId);
        //Task<ConfigurationRoleList> GetRoles(long userId);
        //Task<ConfigurationRoleList> GetRolesId(long userId);
        Task<int> UserEmployeeRole(Guid userId, Guid roleId);
        //Task<List<ConfigurationRole>> RoleByCompanyId(long companyId);
        Task<int> Role(ConfigurationRole config);
        Task<Guid> GetRolesId(Guid Id);
        Task<List<LogsResponse>> GetLogs(string userName, DateTime fromDate, DateTime toDate);
        Task<List<LogsResponse>> GetAllLogs(DateTime fromDate, DateTime toDate);
        Task<int> LockAccount(Guid Id);
        Task<int> UnLockAccount(Guid Id);
    }
}
