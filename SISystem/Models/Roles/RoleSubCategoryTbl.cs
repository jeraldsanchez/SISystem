using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class RoleSubCategoryTbl
    {
        [Key]
        public long RoleSubCategory { get; set; }
        public string RoleSubName { get; set; }
        public string RoleSubDescription { get; set; }
        public long RoleCategoryId { get; set; }
    }
}
