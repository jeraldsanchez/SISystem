using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class CustomerDetails
    {
        public long ContactId { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string PhoneNo { get; set; }
    }
}
