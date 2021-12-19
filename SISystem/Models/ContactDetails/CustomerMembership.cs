using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerMembership
    {
        public long StoreId { get; set; }
        public string MembershipName { get; set; }
        public decimal Discount1 { get; set; }
        public decimal Discount2 { get; set; }
        public decimal Discount3 { get; set; }
        public decimal Discount4 { get; set; }
        public bool IsActive { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
