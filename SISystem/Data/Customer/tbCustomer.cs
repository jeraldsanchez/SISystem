
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class tbCustomer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string TIN { get; set; }
        public string Status { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
