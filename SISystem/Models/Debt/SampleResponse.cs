using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class SampleResponse
    {
        public Guid? SalesAgentId { get; set; }
        public string SalesAgentName { get; set; }
        public Guid CustomerOrderId { get; set; }
       public List<tbCustomerOrderDetails> customerOrderDetails { get; set; }
    }
}
