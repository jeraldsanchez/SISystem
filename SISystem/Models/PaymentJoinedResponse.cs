using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class PaymentJoinedResponse
    {
        public tbPaymentParentCustomer parentCustomer { get; set; }
        public tbCustomerOrderMain orderMain { get; set; }
    }
}
