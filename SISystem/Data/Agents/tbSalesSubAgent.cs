using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbSalesSubAgent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid SalesAgentId { get; set; }
        public string SubAgentName { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
