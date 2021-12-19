using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerSalesAgentResponse
    {
        //public Guid AgentId { get; set; }
        //public decimal? Rate { get; set; }
        public tbSalesAgent Agent { get; set; } = new tbSalesAgent();
        //public List<tbCustomer> Customers { get; set; } = new List<tbCustomer>();
        public List<CustomerResponseSalesAgent> Customers { get; set; } = new List<CustomerResponseSalesAgent>();
        //public string AgentName { get; set; }
    }
}
