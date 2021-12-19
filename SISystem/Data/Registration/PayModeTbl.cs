using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class PayModeTbl
    {
        [Key]
        public int PayModeId { get; set; }
        public string PayMode { get; set; }
        public string PayModeDescription { get; set; }
    }
}
