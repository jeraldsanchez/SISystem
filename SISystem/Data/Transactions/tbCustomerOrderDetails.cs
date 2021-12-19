using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbCustomerOrderDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CustomerOrderId { get; set; }
        public Guid IssuingCompanyId { get; set; }
        public Guid SalesAgentId { get; set; }
        public decimal SalesAmount { get; set; }
        public string SINo { get; set; }
        public string CRNo { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
