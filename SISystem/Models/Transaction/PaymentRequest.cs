using SISystem.Data;
using System.Collections.Generic;

namespace SISystem.Models
{
    public class PaymentRequest
    {
        public tbPaymentParentDetails PaymentParentDetails { get; set; } = new tbPaymentParentDetails();
        public List<tbPaymentParentCustomer> PaymentParentCustomer { get; set; } = new List<tbPaymentParentCustomer>();
        public List<tbPaymentForm> PaymentForm { get; set; } = new List<tbPaymentForm>();
    }
}