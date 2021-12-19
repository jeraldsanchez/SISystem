using System;

namespace SISystem.Models
{
    public class BadDebtAgentResponse
    {
        public Guid CustomerOrderId { get; set; }
        public Guid? SalesAgentId { get; set; }
        public string SalesAgentName { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime? BadDebtDate { get; set; }
        public DateTime? MarkDeliverDate { get; set; }
        public string SubAgentName { get; set; }
        public string CustomerName { get; set; }
        public string TinNumber { get; set; }
        public decimal DeliveredAmount { get; set; }
        public string OverrideRate { get; set; }
        public decimal? ReceivableAmount { get; set; }
        public decimal PartialPayment { get; set; }
        public decimal? Balance { get; set; }
        public int AgeDays { get; set; }
        public string Remarks { get; set; }
    }
}
