using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class RolesResponse
    {
        public tbRolesDetails tbRolesDetails { get; set; } = new tbRolesDetails();
        public List<RoleWithColumn> roleWithColumn { get; set; } = new List<RoleWithColumn>();
    }

    public class RoleWithColumn {
        public tbRoles tbRoles { get; set; } = new tbRoles();
        public List<tbRolesColumnExcept> tbRolesColumnExcept { get; set; } = new List<tbRolesColumnExcept>();
    }
}
