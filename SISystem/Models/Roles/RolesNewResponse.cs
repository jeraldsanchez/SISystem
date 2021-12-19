using System;
using System.Collections.Generic;

namespace SISystem.Models
{
    public class RolesNewResponse
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public string RoleDesciption { get; set; }
        public RoleMainMenu RoleMainMenus { get; set; }
    }

    public class RoleMainMenu
    {
        public string MainMenuLabel { get; set; }
        //public string SubMenu { get; set; }
        //public bool ViewFunc { get; set; }
        //public bool AddFunc { get; set; }
        //public bool EditFunc { get; set; }
        //public bool DeleteFunc { get; set; }
        //public bool RevertFunc { get; set; }
        public List<RoleMainMenuDetails> MainMenuDetails { get; set; }
    }

    public class RoleMainMenuDetails
    {
        public Guid SubMenuId { get; set; }
        public string SubMenu { get; set; }
        public bool ViewFunc { get; set; }
        public bool AddFunc { get; set; }
        public bool EditFunc { get; set; }
        public bool DeleteFunc { get; set; }
        public bool RevertFunc { get; set; }
    }

}
