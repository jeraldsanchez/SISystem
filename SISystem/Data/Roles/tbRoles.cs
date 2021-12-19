using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbRoles
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid RoleDetailsId { get; set; }
        public string MainFeature { get; set; }
        public string SubFeature { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool ViewFunc { get; set; }
        public bool AddFunc { get; set; }
        public bool EditFunc { get; set; }
        public bool DeleteFunc { get; set; }
        public bool RevertFunc { get; set; }
    }
}
