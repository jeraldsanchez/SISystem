
using SISystem.Models;
using System.Collections.Generic;

namespace SISystem.Services
{
    public interface IFunctionsRepository
    {
        string CreatePin(int length);
        string CreateSalt(int length);
        string CreateSaltHashPassword(string password, string salted);
        int Between(int minimumValue, int maximumValue);
        string EncryptSHA256(string input);
        List<string> GetRoles(ConfigurationRoleList roles);
    }
}
