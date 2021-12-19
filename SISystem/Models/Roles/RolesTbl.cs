using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class RolesTbl
    {
        [Key]
        public long RoleId { get; set; }
        public bool Warehouse { get; set; }
        public bool Store { get; set; }
        public bool BackOffice { get; set; }
        public bool Accounting { get; set; }
        public bool Administrator { get; set; }
        public bool SuperAdmin { get; set; }
        public string RoleAccess { get; set; }
        public string RoleDescription { get; set; }
        public DateTime RoleExpirationDate { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
    }
}
