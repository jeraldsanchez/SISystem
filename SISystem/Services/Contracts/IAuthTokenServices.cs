using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace SISystem.Services
{
    public interface IAuthTokenServices
    {
        string GenerateToken(string userName, string id, Guid guid); //ConfigurationRoleList roles
        string GenerateRefreshToken(string userName, string id);
        object GenerateClaims(string userName, string id, ConfigurationRoleList roles);
    }
}
