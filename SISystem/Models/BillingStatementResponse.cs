using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class BillingStatementResponse
    {
        [Key]
        public Guid CustomerOrderID { get; set; }
        public Guid CustomerID { get; set; }
        public Guid SalesAgentID { get; set; }
        public string MarkDeliveredDate { get; set; }
        public string CustCompany { get; set; }
        public string TIN { get; set; }
        public decimal TotalAmount { get; set; }
        public string Rate { get; set; }
        public decimal AmountPayable { get; set; }
        public bool IsBadDebt { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string FullName { get; set; }
        public string AgentName { get; set; }
    }
}
