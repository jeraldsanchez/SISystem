using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbOrderStatus
    {
        [Key]
        public int OrderStatusID { get; set; }
        public string Status { get; set; }
        public string OrderStatusPage { get; set; }
        public string OrderStatusDetails { get; set; }
    }
}
