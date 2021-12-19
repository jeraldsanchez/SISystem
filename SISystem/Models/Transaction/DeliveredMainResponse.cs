using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class DeliveredMainResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalesAgentId { get; set; }
        public Guid? SubSalesAgentId { get; set; }
        public int OrderStatusId { get; set; }
        public string SalesAgentName { get; set; }
        public string SubSalesAgentName { get; set; }
        public string CustomerName { get; set; }
        public string TinNo { get; set; }
        public decimal OrderAmount { get; set; }
        public string ForMonthOf { get; set; }
        public DateTime DateCreated { get; set; }
        public string LeadTime { get; set; }
        public bool? IsCancelled { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime? MarkDeliveredDate { get; set; }
        public Guid ProcessorId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalProcessAmount { get; set; }
        public string Rate { get; set; }
        public decimal? ChargeAmount { get; set; }
    }
}
