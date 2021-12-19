using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class ConfigurationRoleList
    {
        public long UserId { get; set; }
        public string RoleSubName { get; set; }
        public string RoleSubId { get; set; }
        public string RoleSubDescription { get; set; }
        public ConfigurationRole ConfigurationRole { get; set; } = new ConfigurationRole();
    }
}
