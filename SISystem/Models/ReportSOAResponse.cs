using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class ReportSOAResponse
    {
        [Key]
        public Guid IssuingCompanyID { get; set; }
        public decimal Amount { get; set; }
        public string SINo { get; set; }
        public string CRNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DateCreated { get; set; }
        public int OrderStatusID { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string ClientTIN { get; set; }
        public string CustomerAddress { get; set; }
        public string CompanyName { get; set; }
        public string TIN { get; set; }
        public string IssuingAddress { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
