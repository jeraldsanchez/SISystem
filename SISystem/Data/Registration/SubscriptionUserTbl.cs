using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class SubscriptionUserTbl
    {
        [Key]
        public long SubscriptionUserId { get; set; }
        public long SubscriptionId { get; set; }
        public long UserId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
