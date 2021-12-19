using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class IssuingCompanyReponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string CompanyName { get; set; }
        public string TIN { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string RDO { get; set; }
        public DateTime BIRRegDate { get; set; }
        public DateTime RetireDate { get; set; }
        public string Status { get; set; }
        public Guid CompanyTypeId { get; set; }
    }
}
