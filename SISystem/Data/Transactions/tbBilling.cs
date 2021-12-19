using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbBilling
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BillingID { get; set; }
        public Guid SalesAgentID { get; set; }
        public DateTime BillingFrom { get; set; }
        public DateTime BillingTo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal TotalPayment { get; set; }
        public string Remarks { get; set; }
        public string CollectionNo { get; set; }
        public DateTime CollectionDate { get; set; }
        public decimal OverShortPayment { get; set; }
        public decimal Discount { get; set; }
        public decimal CashTotal { get; set; }
        public decimal CheckTotal { get; set; }
        public decimal BankDepositTotal { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid UserId { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid? ModifiedByID { get; set; }
    }
}
