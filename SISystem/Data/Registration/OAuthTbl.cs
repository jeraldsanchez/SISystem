using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Data
{
    public class OAuthTbl
    {
        [Key]
        public long SysId { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshedAccessToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime RefreshedExpirationDate { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
