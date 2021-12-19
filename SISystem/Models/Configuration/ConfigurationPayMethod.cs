using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class ConfigurationPayMethod
    {
        public string PayMethod { get; set; }
        public string PayMethodDescription { get; set; }
        public long CompanyId { get; set; }
        public bool IsActive { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
