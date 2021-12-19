using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class RegistrationRegister
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        [Required(ErrorMessage = "Email can't be empty")]
        public string emailAddress { get; set; }

        [Required(ErrorMessage = "Username can't be empty")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Password can't be empty")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        
        public string salt { get; set; }

        public Guid RolesId { get; set; }

    }
}
