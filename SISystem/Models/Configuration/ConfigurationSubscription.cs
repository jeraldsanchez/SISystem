using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class ConfigurationSubscription
    {
        public string SubsAccess { get; set; }
        public long PayMethodId { get; set; }
        public decimal SubsAmount { get; set; }
        public long PayModeId { get; set; }
        public string SubsDescription { get; set; }
        public int SubsExpirationDate { get; set; }
    }
}
