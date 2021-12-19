using System;

namespace SISystem.Models
{
    public class LogsResponse
    {
        public string UserName { get; set; }
        public string TransactionType { get; set; }
        public string ResponseDetails { get; set; }
        public string Response { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
