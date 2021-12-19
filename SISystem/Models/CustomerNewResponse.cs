using System;
using SISystem.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerNewResponse
    {
        public tbCustomer customer { get; set; }
        public List<tbCustomerSalesAgent> SalesAgents { get; set; }
    }
}
