using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CompanySalesResponse
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string TIN { get; set; }
        public decimal? Rate { get; set; }
        public bool IsRetired { get; set; }
        public string CustomerAddress { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DateCreated { get; set; }

        public List<tbCustomerSalesAgent> SalesAgents { get; set; }
    }
}
