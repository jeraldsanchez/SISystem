using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerContacts
    {
        public long CompanyId { get; set; }
        public long ContactId { get; set; }
        public long ContactType { get; set; }

        [Required(ErrorMessage = "Name can't be empty")]
        public string ContactName { get; set; }
        public string ContactCode { get; set; }
        public string ContactTIN { get; set; }
        public bool IsActive { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
