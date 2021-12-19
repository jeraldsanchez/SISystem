using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbPaymentDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PaymentDetailsID { get; set; }
        public Guid BillingID { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string CheckNumber { get; set; }
        public DateTime CheckDate { get; set; }
        public string DepositoryBank { get; set; }
        public string Branch { get; set; }
        public DateTime ReferenceDate { get; set; }
    }
}
