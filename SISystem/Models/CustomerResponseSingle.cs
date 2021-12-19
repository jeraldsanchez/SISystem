using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerResponseSingle
    {
        public tbCustomer Customer { get; set; } = new tbCustomer();
        //public List<tbCustomer> Customers { get; set; } = new List<tbCustomer>();
        public List<CustomerResponseSalesAgent> Customers { get; set; } = new List<CustomerResponseSalesAgent>();

    }

}
