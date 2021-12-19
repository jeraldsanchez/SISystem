using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class UpdateRoleRequest
    {
        //public tbRolesDetails tbRolesDetails { get; set; } = new tbRolesDetails();
        public Guid RoleDetailsId { get; set; }
        public List<RoleWithColumn> roleWithColumn { get; set; } = new List<RoleWithColumn>();
    }
}
