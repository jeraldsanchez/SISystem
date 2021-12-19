using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbSalesAgent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string MobileNo1 { get; set; }
        public string MobileNo2 { get; set; }
        public string MobileNo3 { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string EmailAddress3 { get; set; }
        public decimal Rate { get; set; }
        public int Terms { get; set; }
        public string Remarks { get; set; }
        public DateTime DateCreated { get; set; }
        public string Status { get; set; }
    }
}
