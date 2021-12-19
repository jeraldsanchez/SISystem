using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class PaymentPartialRequest
    {
        public Guid ParentCustomerId { get; set; }
        public Guid PaymentParentId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public List<tbPaymentForm> PaymentForm { get; set; } = new List<tbPaymentForm>();
    }
}
