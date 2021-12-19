using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class AccountsSummaryResponse
    {
        public Guid? SalesAgentId { get; set; }
        public Guid? CustomerOrderId { get; set; }
        public string SalesAgentName { get; set; }
        public int TransactionCount { get; set; }
        public decimal TransactionBalance { get; set; }
        public decimal Transaction1to30 { get; set; }
        public decimal Transaction31to60 { get; set; }
        public decimal Transaction61to90 { get; set; }
        public decimal Transaction91 { get; set; }
    }
}
