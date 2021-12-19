using System;

namespace SISystem.Models
{
    public class LogsRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string UserName { get; set; }
    }
}
