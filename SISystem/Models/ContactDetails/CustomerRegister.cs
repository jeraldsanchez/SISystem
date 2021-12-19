using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerRegister
    {
        public long CustomerId { get; set; }

        [Required(ErrorMessage = "Name can't be empty")]
        public string CustomerName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string CustomerEmail { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerAddress { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
    }
}
