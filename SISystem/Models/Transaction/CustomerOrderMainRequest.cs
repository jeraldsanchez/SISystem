using System;

namespace SISystem.Models
{
    public class CustomerOrderMainRequest
    {
        public Guid CustomerId { get; set; }
        public Guid SalesAgentId { get; set; }
        public Guid? SubSalesAgentId { get; set; }
        public string SalesAgentName { get; set; }
        public string SubSalesAgentName { get; set; }
        public string CustomerName { get; set; }
        public string TinNo { get; set; }
        public string TransactionCode { get; set; }
        public decimal OrderAmount { get; set; }
        public DateTime ForMonthOf { get; set; }
        public DateTime OrderDate { get; set; }

        //public Guid EndcodedById { get; set; } //get by token
        //public string EncodedByName { get; set; }
        public string RemarksCustomerOrder { get; set; }
    }
}
