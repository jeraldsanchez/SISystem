using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerResponse
    {
        public Guid CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string TIN { get; set; }
        public string Status { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime DateCreated { get; set; }

        public Guid CustomerSalesId { get; set; }
        public Guid SalesAgentId { get; set; }
        public decimal? OverrideRate { get; set; }
        public DateTime DateSalesCreated { get; set; }

        public string SalesAgentName { get; set; }
        public string MobileNo1 { get; set; }
        public string EmailAddress1 { get; set; }

    }

}
