using System;
using System.ComponentModel.DataAnnotations;

namespace SISystem.Data
{
    public class UserForgotPassword
    {
        [Key]
        public long SysId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Pin { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsVerify { get; set; }
    }
}
