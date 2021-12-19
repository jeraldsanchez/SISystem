using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class SubAgentResponse
    {
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public string SubAgentName { get; set; }
        public string Status { get; set; }
        public string AgentName { get; set; }
    }
}
