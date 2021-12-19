using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbCustomerSalesAgent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalesAgentId { get; set; }
        public decimal? OverrideRate { get; set; }
        public DateTime DateCreated { get; set; }
        public string Remarks { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsOverride { get; set; }
        public bool IsActive { get; set; }
    }
}
