using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbLogs
    {
        [Key]
        public long Id { get; set; }
        public string Username { get; set; }
        public Guid? UserId { get; set; }
        public string Response { get; set; }
        public string ResponseDetails { get; set; }
        public string TransactionType { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
