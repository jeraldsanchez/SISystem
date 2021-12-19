using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Models
{
    public class TokenServiceGenerateToken
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshedAccessToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime RefreshedExpirationDate { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
