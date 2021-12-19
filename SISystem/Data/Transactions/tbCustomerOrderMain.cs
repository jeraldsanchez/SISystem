using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbCustomerOrderMain
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalesAgentId { get; set; }
        public Guid? SubSalesAgentId { get; set; }
        public int OrderStatusId { get; set; }
        public string SalesAgentName { get; set; }
        public string SubSalesAgentName { get; set; }
        public string CustomerName { get; set; }
        public string TinNo { get; set; }
        public string TransactionCode { get; set; }
        public decimal OrderAmount { get; set; }
        public DateTime ForMonthOf { get; set; }
        public decimal LeadTime { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsCancelled { get; set; }
        public string SOANo { get; set; }
        public bool? IsPaid { get; set; }
        public bool? IsBadDebt { get; set; }
    }
}
