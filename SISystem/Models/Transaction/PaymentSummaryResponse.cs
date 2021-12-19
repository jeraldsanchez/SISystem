using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class PaymentSummaryResponse
    {
        public DateTime TransactionDate { get; set; }
        public string TransactionDay { get; set; }
        public decimal Cash { get; set; }
        public decimal Check { get; set; }
        public decimal BankDeposit { get; set; }
        public decimal Total { get; set; }
    }
}
