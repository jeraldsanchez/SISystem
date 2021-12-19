using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class Auth
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
