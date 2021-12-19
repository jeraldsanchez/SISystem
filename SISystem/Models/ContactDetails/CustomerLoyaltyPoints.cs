using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerLoyaltyPoints
    {
        public long ContactId { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
