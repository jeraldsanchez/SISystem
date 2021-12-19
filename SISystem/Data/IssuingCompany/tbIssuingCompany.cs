using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbIssuingCompany
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CompanyTypeID { get; set; }
        public string CompanyName { get; set; }
        public string TIN { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string RDO { get; set; }
        public DateTime BIRRegDate { get; set; }
        public DateTime RetireDate { get; set; }
        public string Status { get; set; }
    }
}
