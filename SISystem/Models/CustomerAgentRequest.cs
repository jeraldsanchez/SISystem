using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerAgentRequest
    {
        public tbCustomer customer { get; set; }
        public List<tbCustomerSalesAgent> customerSalesAgent { get; set; }
    }
}
