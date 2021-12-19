using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class ConfigurationRole
    {
        public string RoleAccess { get; set; }
        public bool Warehouse { get; set; }
        public bool Store { get; set; }
        public bool BackOffice { get; set; }
        public bool Accounting { get; set; }
        public bool Admin { get; set; }
        public string RoleDescription { get; set; }
        public int RoleExpirationDate { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }
}
