using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class InvoiceMainResponse
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalesAgentId { get; set; }
        public Guid? SubSalesAgentId { get; set; }
        public string Address { get; set; }
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
        public string SOANo { get; set; }

        public Guid ProcessorId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public Guid? ProcessedById { get; set; }
        public string ProcessedByName { get; set; }
        public DateTime? ProcessDate { get; set; }
        public Guid? TransferredById { get; set; }
        public string TransferredByName { get; set; }
        public DateTime? TransferDate { get; set; }
        public DateTime? OrderDate { get; set; }
        public Guid? ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public string RemarksInvoice { get; set; }
        public decimal TotalProcessAmount { get; set; }
        public List<InvoiceOrderDetailsResponse> CustomerOrderDetails { get; set; }
    }
}
