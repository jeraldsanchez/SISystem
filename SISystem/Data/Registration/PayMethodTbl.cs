using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class PayMethodTbl
    {
        [Key]
        public long PayMethodId { get; set; }
        public string PayMethod { get; set; }
        public string PayMethodDescription { get; set; }
        public long CompanyId { get; set; }
        public bool IsActive { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
