using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class PaymentSummaryJoin
    {
        public Guid ParentId { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid FormId { get; set; }
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
    }
}
