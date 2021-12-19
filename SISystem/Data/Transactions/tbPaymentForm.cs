using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISystem.Data
{
    public class tbPaymentForm
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid PaymentParentId { get; set; }
        public string PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
        public string AccountName { get; set; }
        public string AccountNo { get; set; }
    }
}
