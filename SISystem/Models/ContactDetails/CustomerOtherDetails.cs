using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerOtherDetails
    {
        public long ContactId { get; set; }
        public DateTime MembershipSinceDate { get; set; }
        public int CreditTerms { get; set; }
        public string SalesAgentId { get; set; }
        public long MembershipTypeId { get; set; }
        public bool IsLoyaltyPoints { get; set; }
        public bool IsCreditTerms { get; set; }
        public bool IsReceipts { get; set; }
        public decimal CommissionPercent { get; set; }
        public string CommissionBased { get; set; }
    }
}
