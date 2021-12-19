using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class RegistrationChangePassword
    {
        public string userName { get; set; }
        public string newPassword { get; set; }
        public string oldPassword { get; set; }
    }
}
