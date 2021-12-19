using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class RoleCategoryTbl
    {
        [Key]
        public long RoleCategoryId { get; set; }
        public string RoleSubName { get; set; }
        public string RoleSubDescription { get; set; }
    }
}
