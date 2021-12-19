using System;

namespace SISystem.Models
{
    public class PaymentsReceivedResponse
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public int TransactionCount { get; set; }
        public decimal AppliedAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
        public string EncodedBy { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
