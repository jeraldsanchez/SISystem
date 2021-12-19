using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbPaymentParentCustomer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid PaymentParentId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal ReceivableAmount { get; set; }
        public decimal PartialAmount { get; set; }
        public string Remarks { get; set; }
    }
}
