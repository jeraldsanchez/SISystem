using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class SubscriptionTbl
    {
        [Key]
        public long SubscriptionId { get; set; }
        public string SubsAccess { get; set; }
        public long PayMethodId { get; set; }
        public decimal SubsAmount { get; set; }
        public long PayModeId { get; set; }
        public string SubsDescription { get; set; }
        public DateTime SubsExpirationDate { get; set; }
    }
}
