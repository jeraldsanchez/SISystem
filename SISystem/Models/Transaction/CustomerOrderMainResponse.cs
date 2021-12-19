using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerOrderMainResponse
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
        public bool? IsCancelled { get; set; }
        public string CancelledPage { get; set; }
        public Guid ProcessorId { get; set; }
        public Guid CustomerOrderId { get; set; }
        public Guid? EndcodedById { get; set; }
        public string EncodedByName { get; set; }
        public DateTime? OrderDate { get; set; }
        public Guid? ApprovedById { get; set; }
        public string ApprovedByName { get; set; }
        public Guid? CancelledById { get; set; }
        public string CancelledByName { get; set; }
        public DateTime? CancelDate { get; set; }
        public string RemarksCustomerOrder { get; set; }


        //public tbCustomerOrderMain CustomerOrder { get; set; }
        //public tbCustomerOrderProcessor OrderProcessor { get; set; }
    }
}
