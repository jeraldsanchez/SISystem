using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SISystem.Data;
using SISystem.Models;
using SISystem.Services;

namespace SISystem.Services
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly SISystemDbContext _sISystemDbContext;

        public RegistrationRepository(SISystemDbContext sISystemDbContext)
        {
            _sISystemDbContext = sISystemDbContext;
        }

        public async Task<int> InsertUser(tbUser tbl)
        {
            _sISystemDbContext.Add(tbl);
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public string GetIdentity(IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;

            Claim claim = claimsIdentity.FindFirst(ClaimTypes.Role);

            return claim.Value;
        }

        #region Roles
        public async Task<int> AddRoles(RolesResponse roles)
        {
            tbRolesDetails rolesDetails = new tbRolesDetails
            {
                RoleName = roles.tbRolesDetails.RoleName,
                IsActive = true,
                RoleExpirationDate = DateTime.Now.AddYears(10),
                RoleDescription= roles.tbRolesDetails.RoleDescription
            };
            await _sISystemDbContext.tbRolesDetails.AddAsync(rolesDetails);

            foreach (var item in roles.roleWithColumn)
            {
                item.tbRoles.RoleDetailsId = rolesDetails.Id;
                item.tbRoles.IsActive = true;
                item.tbRoles.ExpirationDate = DateTime.Now.AddYears(10);
                await _sISystemDbContext.tbRoles.AddAsync(item.tbRoles);

                item.tbRolesColumnExcept.ForEach(x => {
                    x.RoleDetailsId = rolesDetails.Id;
                    x.RoleId = item.tbRoles.Id;
                });
                await _sISystemDbContext.tbRolesColumnExcept.AddRangeAsync(item.tbRolesColumnExcept);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateDeleteRoles(Guid roleDetailsId)
        {
            var roleMenu = await _sISystemDbContext.tbRoles.Where(x => x.RoleDetailsId == roleDetailsId).ToListAsync();
            _sISystemDbContext.tbRoles.RemoveRange(roleMenu);

            var rolesExcept = await _sISystemDbContext.tbRolesColumnExcept.Where(x => x.RoleDetailsId == roleDetailsId).ToListAsync();
            _sISystemDbContext.tbRolesColumnExcept.RemoveRange(rolesExcept);

            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }

        public async Task<int> UpdateAddRoles(UpdateRoleRequest roles)
        {
            foreach (var item in roles.roleWithColumn)
            {
                item.tbRoles.RoleDetailsId = roles.RoleDetailsId;
                item.tbRoles.IsActive = true;
                item.tbRoles.ExpirationDate = DateTime.Now.AddYears(10);
                await _sISystemDbContext.tbRoles.AddAsync(item.tbRoles);

                item.tbRolesColumnExcept.ForEach(x => {
                    x.RoleDetailsId = roles.RoleDetailsId;
                    x.RoleId = item.tbRoles.Id;
                });
                await _sISystemDbContext.tbRolesColumnExcept.AddRangeAsync(item.tbRolesColumnExcept);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        public async Task<Object> GetRolesCompleteById(Guid guid)
        {
            var roleDetails = await _sISystemDbContext.tbRolesDetails.Where(x => x.Id == guid).ToListAsync();
            var test1 = roleDetails.Select(x => new {
                rolesD = x,
                rolesR = _sISystemDbContext.tbRoles.Where(s => s.RoleDetailsId == x.Id).ToList()
            }).ToList();

            var test = test1.Select(x => new {
                role = x.rolesD,
                roleConfig = x.rolesR.Select(y => new {
                    roleFeature = y,
                    rolesExceptColumn = _sISystemDbContext.tbRolesColumnExcept.Where(s => s.RoleId == y.Id).Select(t => t.ColumnExcept).ToList()
                })
            }).ToList();
            return test;
        }

        public async Task<List<RolesNewResponse>> GetRolesMainMenuById(Guid guid)
        {
            var roleDetails = await _sISystemDbContext.tbRolesDetails.Where(x => x.Id == guid).ToListAsync();

            var joinedTable2 = roleDetails.
               Join(_sISystemDbContext.tbRoles,
               parent => parent.Id,
               child => child.RoleDetailsId,
               (parent, child) => new
               {
                   roles = parent,
                   mainmenu = child
               }).ToList();

            var groupedTable = joinedTable2
                .GroupBy(x => new { x.mainmenu.MainFeature, x.mainmenu.RoleDetailsId})
                .Select(x => new RolesNewResponse
                {
                    Id = x.Select(y => y.roles.Id).FirstOrDefault(),
                    RoleName = x.Select(y => y.roles.RoleName).FirstOrDefault(),
                    RoleDesciption = x.Select(y => y.roles.RoleDescription).FirstOrDefault(),
                    RoleMainMenus = x.Where(y => y.mainmenu.RoleDetailsId == x.Key.RoleDetailsId).Select(h => new RoleMainMenu
                    {
                        MainMenuLabel = x.Select(y => y.mainmenu.MainFeature).FirstOrDefault(),
                        MainMenuDetails = x.Where(y => y.mainmenu.RoleDetailsId == x.Key.RoleDetailsId).Select(b => new RoleMainMenuDetails
                        {
                            SubMenuId = b.mainmenu.Id,
                            AddFunc = b.mainmenu.AddFunc,
                            DeleteFunc = b.mainmenu.DeleteFunc,
                            EditFunc = b.mainmenu.EditFunc,
                            RevertFunc = b.mainmenu.RevertFunc,
                            SubMenu = b.mainmenu.SubFeature,
                            ViewFunc = b.mainmenu.ViewFunc
                        }).ToList()
                    }).FirstOrDefault()
                }).ToList();

            return groupedTable;
        }

        public async Task<Object> GetRolesColumnExceptById(Guid guid)
        {
            var roleDetails = await _sISystemDbContext.tbRolesColumnExcept
                .Where(x => x.RoleId == guid)
                .Select(y => new { 
                    y.RoleId,
                    y.RoleDetailsId,
                    y.ColumnExcept
                }).ToListAsync();

            return roleDetails;
        }

        public async Task<Object> GetRolesComplete()
        {
            var roleDetails = await _sISystemDbContext.tbRolesDetails.ToListAsync();
            var test1 = roleDetails.Select(x => new {
                rolesD = x,
                rolesR = _sISystemDbContext.tbRoles.Where(s => s.RoleDetailsId == x.Id).ToList()
            }).ToList();

            var test = test1.Select(x => new { 
                    role = x.rolesD,
                    roleConfig = x.rolesR.Select(y => new { 
                        roleFeature = y,
                        rolesExceptColumn = _sISystemDbContext.tbRolesColumnExcept.Where(s => s.RoleId == y.Id ).Select(t => t.ColumnExcept).ToList()
                    })
            }).ToList();
            return test;
        }
        public async Task<Object> GetRoles()
        {
            var roles = await _sISystemDbContext.tbRolesDetails.Select(x => x).Select(
                    y => new {
                        y.Id,
                        y.RoleName
                    }).ToListAsync();

            return roles;
        }

        public async Task<int> DeleteRoles(Guid roleDetailsId)
        {
            var parentRole = await _sISystemDbContext.tbRolesDetails.Where(x => x.Id == roleDetailsId).ToListAsync();
            _sISystemDbContext.tbRolesDetails.RemoveRange(parentRole);

            var roleMenu = await _sISystemDbContext.tbRoles.Where(x => x.RoleDetailsId == roleDetailsId).ToListAsync();
            _sISystemDbContext.tbRoles.RemoveRange(roleMenu);

            var rolesExcept = await _sISystemDbContext.tbRolesColumnExcept.Where(x => x.RoleDetailsId == roleDetailsId).ToListAsync();
            _sISystemDbContext.tbRolesColumnExcept.RemoveRange(rolesExcept);

            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }
        #endregion

        #region Register
        public async Task<bool> GetUserByName(string name, string mobileNo)
        {
            bool user = await _sISystemDbContext.tbUserDetails.Select(x => x.Name == name || x.MobileNo == mobileNo).FirstOrDefaultAsync();
            return user;
        }
        public async Task<int> SaveRolesByRegistration(RegistrationRegister register)
        {
            tbUserRole user = new tbUserRole
            {
                UserId = register.UserId,
                RolesId = register.RolesId
            };
            _sISystemDbContext.tbUserRole.Add(user);
            return await _sISystemDbContext.SaveChangesAsync();
        }
        public async Task<int> Registration(RegistrationRegister register, string salt, string hashedPassword)
        {
            tbUser user = new tbUser {
                Username = register.userName,
                EmailAddress = register.emailAddress,
                DateCreated = DateTime.Now,
                IsLocked = false,
                Salt = salt,
                HashedPassword = hashedPassword
            };
            _sISystemDbContext.tbUser.Add(user);
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<int> SaveUserDetails(RegistrationRegister register)
        {
            tbUserDetails user = new tbUserDetails
            {
                UserId = register.UserId,
                MobileNo = register.MobileNo,
                Name = register.Name,
                EmailAddress = register.emailAddress
            };
            _sISystemDbContext.tbUserDetails.Add(user);
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Get Registration
        public async Task<IEnumerable<Object>> GetUser()
        {
            var user = await _sISystemDbContext.tbUser.Select(x => x).ToListAsync();

            var userDetails = user
                .Join(_sISystemDbContext.tbUserDetails,
                 id => id.Id,
                id2 => id2.UserId,
                (id, id2) => new
                {
                    id.Username,
                    id2.MobileNo,
                    id.Id,
                    id2.Name,
                    id2.EmailAddress
                }).ToList();
            return userDetails;
        }
        public Task<RegistrationRegister> GetUsername(string username)
        {
            return Task.FromResult<RegistrationRegister>(
                _sISystemDbContext
                .tbUser
                .Where(x => x.Username == username.Trim() && x.IsLocked == false)
                .Select(s => new RegistrationRegister
                {
                    UserId = s.Id,
                    emailAddress = s.EmailAddress,
                    password = s.HashedPassword,
                    salt = s.Salt,
                    userName = s.Username,
                }).FirstOrDefault());
        }


        public Task<RegistrationRegister> GetUsernameLocked(string username)
        {
            return Task.FromResult<RegistrationRegister>(
                _sISystemDbContext
                .tbUser
                .Where(x => x.Username == username.Trim() && x.IsLocked == true)
                .Select(s => new RegistrationRegister
                {
                    UserId = s.Id,
                    emailAddress = s.EmailAddress,
                    password = s.HashedPassword,
                    salt = s.Salt,
                    userName = s.Username,
                }).FirstOrDefault());
        }
        #endregion

        #region Verify by Email Address and User Name
        public Task<RegistrationRegister> VerifyAccount(ForgotPassword forgot)
        {
            return Task.FromResult<RegistrationRegister>(
                _sISystemDbContext
                .tbUser
                .Where(x => x.Username == forgot.userName.Trim() && x.EmailAddress == forgot.emailAddress.Trim() && x.IsLocked == false)
                .Select(s => new RegistrationRegister
                {
                    userName = s.Username,
                    UserId = s.Id
                }).FirstOrDefault());
        }
        #endregion

        #region Change Password
        public async Task<int> ChangePassword(RegistrationChangePassword register, string salt, string hashedPassword, string oldPassword)
        {
            var userDetails = _sISystemDbContext.tbUser.FirstOrDefault(item => item.Username == register.userName && item.HashedPassword == oldPassword);
            if (userDetails != null)
            {
                userDetails.Salt = salt;
                userDetails.HashedPassword = hashedPassword;
                _sISystemDbContext.Update(userDetails);
            }

            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Insert Forgot Password
        public async Task<int> ForgotPassword(ForgotPassword forgot, string pin)
        {
            var userDetails = _sISystemDbContext.UserForgotPassword.FirstOrDefault(item => item.UserName == forgot.userName && item.EmailAddress == forgot.emailAddress);
            if (userDetails != null)
            {
                userDetails.Pin = pin;
                userDetails.DateCreated = DateTime.Now.AddHours(3);
                userDetails.IsVerify = false;
                _sISystemDbContext.Update(userDetails);
            }
            else
            {
                UserForgotPassword userForgot = new UserForgotPassword
                {
                    UserName = forgot.userName,
                    EmailAddress = forgot.emailAddress,
                    Pin = pin,
                    DateCreated = DateTime.Now.AddHours(3)
                };
                _sISystemDbContext.Add(userForgot);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion

        #region Verfiy Forgot Password
        public async Task<int> VerifyForgotPassword(VerifyForgotPassword forgot, string salt, string hashedPassword, string emailAddress)
        {
            var forgotDetails = _sISystemDbContext.UserForgotPassword.FirstOrDefault(item => item.UserName == forgot.userName && item.EmailAddress == emailAddress);
            if (forgotDetails != null)
            {
                forgotDetails.IsVerify = true;
                _sISystemDbContext.Update(forgotDetails);
            }

            var userDetails = _sISystemDbContext.tbUser.OrderByDescending(x => x.DateCreated).FirstOrDefault(item => item.Username == forgot.userName && item.EmailAddress == emailAddress);
            if (userDetails != null)
            {
                userDetails.Salt = salt;
                userDetails.HashedPassword = hashedPassword;
                _sISystemDbContext.Update(userDetails);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion
    }
}
