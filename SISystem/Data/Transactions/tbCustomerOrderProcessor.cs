using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbCustomerOrderProcessor
    {
        public Guid Id { get; set; }
        public Guid CustomerOrderId { get; set; }
        public Guid? EndcodedById { get; set; }
        public string EncodedByName { get; set; }
        public DateTime? OrderDate { get; set; }
        public Guid? ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public Guid? ProcessedById { get; set; }
        public string ProcessedByName { get; set; }
        public DateTime? ProcessDate { get; set; }
        public Guid? TransferredById { get; set; }
        public string TransferredByName { get; set; }
        public DateTime? TransferDate { get; set; }
        public Guid? MarkDeliveredById { get; set; }
        public string MarkDeliveredByName { get; set; }
        public DateTime? MarkDeliverDate { get; set; }
        public Guid? CancelledById { get; set; }
        public string CancelledByName { get; set; }
        public DateTime? CancelDate { get; set; }
        public Guid? LastModifiedById { get; set; }
        public string LastModifiedByName { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string RemarksCustomerOrder { get; set; }
        public string RemarksInvoice { get; set; }
        public string RemarksDelivered { get; set; }
        public Guid? BadDebtById { get; set; }
        public string BadDebtByName { get; set; }
        public DateTime? BadDebtDate { get; set; }
        public string RemarksBadDebt { get; set; }
    }
}
