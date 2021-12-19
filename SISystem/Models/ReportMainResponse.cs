using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class ReportMainResponse
    {
        public tbIssuingCompany TbIssuingCompany { get; set; } = new tbIssuingCompany();
        public tbCustomerOrder TbCustomerOrder { get; set; } = new tbCustomerOrder();
        public tbCustomer TbCustomer { get; set; } = new tbCustomer();
        public tbCustomerOrderDetails TbCustomerOrderDetails { get; set; } = new tbCustomerOrderDetails();
    }
}
