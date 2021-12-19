using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerResponseSalesAgent
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string TIN { get; set; }
        public decimal? Rate { get; set; }
        public string Status { get; set; }
        public string CustomerAddress { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid SalesAgentId { get; set; }
        public string SalesFullName { get; set; }
        public decimal? SalesRate { get; set; }
        public int SalesTerms { get; set; }
        public Guid SubAgentId { get; set; }
    }
}
