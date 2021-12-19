using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbCustomerOrderVersion
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CustomerOrderId { get; set; }
        public int OrderStatusID { get; set; }
        public string TransactionBy { get; set; } //check if need to Guid
        public DateTime DateCreated { get; set; }

    }
}
