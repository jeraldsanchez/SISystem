using SISystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class InvoiceOrderDetailsResponse
    {
        public string SupplierName { get; set; }
        public string TinNo { get; set; }
        public string SupplierAddress { get; set; }
        public string SINo { get; set; }
        public string CRNo { get; set; }
        public decimal? SalesAmount { get; set; }
        public DateTime InvoiceDate { get; set; }


        public Guid Id { get; set; }
        public Guid CustomerOrderId { get; set; }
        public Guid IssuingCompanyId { get; set; }
        public Guid SalesAgentId { get; set; }
    }
}
