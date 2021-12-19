using Microsoft.EntityFrameworkCore;
using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class CustomerInformationServices : ICustomerInformationServices
    {
        private readonly SISystemDbContext _sISystemDbContext;

        public CustomerInformationServices(SISystemDbContext sISystemDbContext)
        {
            _sISystemDbContext = sISystemDbContext;
        }

        #region Insert user details
        public async Task<int> UserDetails(UserDetails register, RegistrationRegister reg)
        {
            tbUserDetails user = new tbUserDetails
            {
                Name = reg.Name,
                MobileNo = register.mobileNo,
                UserId = reg.UserId,
                EmailAddress = reg.emailAddress,
            };
            _sISystemDbContext.Add(user);
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Insert Employee role details
        public async Task<int> UserEmployeeRole(Guid userId, Guid roleId)
        {
            tbUserRole user = new tbUserRole
            {
                Remarks = "",
                UserId = userId,
                RolesId = roleId
            };
            _sISystemDbContext.Add(user);
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Insert admin role details
        public async Task<int> UserRole(RegistrationRegister register)
        {
            tbUserRole user = new tbUserRole
            {
                Remarks = "",
                RolesId = register.RolesId,
                UserId = register.UserId
            };
            _sISystemDbContext.Add(user);
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Lock Account
        public async Task<int> LockAccount(Guid Id)
        {
            var user = await _sISystemDbContext.tbUser.Where(x => x.Id == Id && x.IsLocked == false).FirstOrDefaultAsync();
            if (user != null)
            {
                user.IsLocked = true;
                _sISystemDbContext.tbUser.Update(user);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<int> UnLockAccount(Guid Id)
        {
            var user = await _sISystemDbContext.tbUser.Where(x => x.Id == Id && x.IsLocked == true).FirstOrDefaultAsync();
            if (user != null)
            {
                user.IsLocked = false;
                _sISystemDbContext.tbUser.Update(user);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion


        #region Get role details
        //public async Task<ConfigurationRoleList> GetRoles(long userId)
        //{
        //    ConfigurationRoleList role = await Task.FromResult<ConfigurationRoleList>(
        //        _sISystemDbContext
        //        .UserRoleTbl
        //        .Where(x => x.UserId == userId)
        //        .Select(s => new ConfigurationRoleList
        //        {
        //            UserId = s.UserId,
        //            RoleSubDescription = _sISystemDbContext
        //                        .RoleCategoryTbl
        //                        .Where(y => y.RoleCategoryId == s.RoleCategoryId)
        //                        .Select(q => q.RoleSubDescription)
        //                        .FirstOrDefault(),
        //            RoleSubName = _sISystemDbContext
        //                        .RoleCategoryTbl
        //                        .Where(y => y.RoleCategoryId == s.RoleCategoryId)
        //                        .Select(q => q.RoleSubName)
        //                        .FirstOrDefault(),
        //            ConfigurationRole = _sISystemDbContext
        //                        .RolesTbl
        //                        .Where(a => a.RoleId == s.RoleId)
        //                        .Select(b => new ConfigurationRole
        //                        {
        //                            Accounting = b.Accounting,
        //                            BackOffice = b.BackOffice,
        //                            RoleAccess = b.RoleAccess,
        //                            Store = b.Store,
        //                            Warehouse = b.Warehouse,
        //                            Admin = b.Administrator
        //                        }).FirstOrDefault()
        //        }).FirstOrDefault());

        //    return role;
        //}
        #endregion

        #region Get role
        public async Task<Guid> GetRolesId(Guid Id)
        {
            return await _sISystemDbContext.tbUserRole.Where(x => x.UserId == Id).Select(y => y.RolesId).FirstOrDefaultAsync();
        }

        public async Task<Guid> GetRoleDetailsById(Guid Id)
        {
            return await _sISystemDbContext.tbUserRole.Where(x => x.UserId == Id).Select(y => y.RolesId).FirstOrDefaultAsync();
        }

        //public async Task<ConfigurationRoleList> GetRolesId(long userId)
        //{
        //    ConfigurationRoleList role = await Task.FromResult<ConfigurationRoleList>(
        //        _sISystemDbContext
        //        .UserRoleTbl
        //        .Where(x => x.UserId == userId)
        //        .Select(s => new ConfigurationRoleList
        //        {
        //            UserId = s.UserId,
        //            RoleSubDescription = _sISystemDbContext
        //                        .RoleCategoryTbl
        //                        .Where(y => y.RoleCategoryId == s.RoleCategoryId)
        //                        .Select(q => q.RoleSubDescription)
        //                        .FirstOrDefault(),
        //            //RoleSubName = _sISystemDbContext
        //            //            .RoleSubCategoryTbl
        //            //            .Where(y => y.RoleSubCategory == s.RoleSubCategory)
        //            //            .Select(q => q.RoleSubName)
        //            //            .FirstOrDefault()
        //        }).FirstOrDefault());

        //    return role;
        //}
        #endregion

        #region Insert Role
        public async Task<int> Role(ConfigurationRole config)
        {
            RolesTbl req = new RolesTbl
            {
                Accounting = config.Accounting,
                Administrator = false,
                BackOffice = config.BackOffice,
                Warehouse = config.Warehouse,
                Store = config.Store,
                SuperAdmin = false,
                RoleAccess = config.RoleAccess,
                RoleDescription = config.RoleDescription,
                RoleExpirationDate = DateTime.Now.AddMonths(100),//verify
                CompanyId = config.CompanyId,
                UserId = config.UserId
            };
            _sISystemDbContext.Add(req);
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Get Roles By CompanyID
        //public async Task<List<ConfigurationRole>> RoleByCompanyId(long companyId)
        //{
        //    List<ConfigurationRole> role = await Task.FromResult<List<ConfigurationRole>>(
        //        _sISystemDbContext
        //        .RolesTbl
        //        .Where(x => x.CompanyId == companyId)
        //        .Select(s => new ConfigurationRole
        //        {
        //            Store = s.Store,
        //            Accounting = s.Accounting,
        //            BackOffice = s.BackOffice,
        //            RoleAccess = s.RoleAccess,
        //            RoleDescription = s.RoleDescription,
        //            Warehouse = s.Warehouse,
        //            RoleId = s.RoleId
        //        }).ToList());

        //    return role;
        //}
        #endregion

        #region Insert logs
        public async Task<int> Logs(string userName, Guid? userId, string response, string responseDetails, string transactionType)
        {
            tbLogs user = new tbLogs
            {
                Username = userName,
                UserId = userId,
                DateCreated = DateTime.Now,
                Response = response,
                ResponseDetails = responseDetails,
                TransactionType = transactionType
            };
            _sISystemDbContext.tbLogs.Add(user);
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<int> LogInEmployee(Guid? userId)
        {
            var employee = await _sISystemDbContext.tbEmployeeLogs.Where(x => x.UserId == userId && x.DateCreated.Date == DateTime.Now.Date).OrderBy(y => y.LogIn).FirstOrDefaultAsync();
            if (employee == null) {
                tbEmployeeLogs user = new tbEmployeeLogs
                {
                    UserId = userId,
                    DateCreated = DateTime.Now,
                    LogIn = DateTime.Now,
                    LogOut = DateTime.Now
                };
                _sISystemDbContext.tbEmployeeLogs.Add(user);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<int> LogOutEmployee(Guid? userId)
        {
            var employee = await _sISystemDbContext.tbEmployeeLogs.Where(x => x.UserId == userId && x.DateCreated.Date == DateTime.Now.Date).OrderBy(y => y.LogIn).FirstOrDefaultAsync();

            if (employee != null) {
                employee.LogOut = DateTime.Now;
                TimeSpan ts = employee.LogOut - employee.LogIn;
                employee.TotalMinutes = ts.Minutes;
                _sISystemDbContext.tbEmployeeLogs.Update(employee);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        public async Task<List<LogsResponse>> GetLogs(string userId, DateTime fromDate, DateTime toDate)
        {
            return await _sISystemDbContext.tbLogs.Where(x => x.Username == userId && (x.DateCreated >= fromDate && x.DateCreated <= toDate)).Select(
                    y => new LogsResponse { 
                    UserName = y.Username,
                    ResponseDetails = y.ResponseDetails,
                    Response = y.Response,
                    DateCreated = y.DateCreated,
                    TransactionType = y.TransactionType
                    }).OrderByDescending(u => u.DateCreated).ToListAsync();
        }

        public async Task<List<LogsResponse>> GetAllLogs(DateTime fromDate, DateTime toDate)
        {
            return await _sISystemDbContext.tbLogs.Where(x => x.DateCreated >= fromDate && x.DateCreated <= toDate).Select(
                    y => new LogsResponse{
                        UserName = y.Username,
                        ResponseDetails = y.ResponseDetails,
                        Response = y.Response,
                        DateCreated = y.DateCreated,
                        TransactionType = y.TransactionType
                    }).OrderByDescending(u => u.DateCreated).ToListAsync();
        }

        public async Task<List<EmployeeLogsResponse>> GetEmployeeLogs(DateTime fromDate, DateTime toDate, Guid? userId)
        {
            return await _sISystemDbContext.tbEmployeeLogs.Where(x => x.UserId == userId && (x.DateCreated >= fromDate && x.DateCreated <= toDate)).Select(
                    y => new EmployeeLogsResponse
                    {
                        LogIn = y.LogIn,
                        LogOut = y.LogOut,
                        TotalMinutes = y.TotalMinutes,
                        DateCreated = y.DateCreated,
                    }).OrderByDescending(u => u.DateCreated).ToListAsync();
        }
    }
}
