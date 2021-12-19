using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class RolesRequest
    {
        public tbRolesDetails tbRolesDetails { get; set; } = new tbRolesDetails();
        public List<tbRoles> tbRoles { get; set; } = new List<tbRoles>();
        public List<tbRolesColumnExcept> tbRolesColumnExcept { get; set; } = new List<tbRolesColumnExcept>();
    }
}
