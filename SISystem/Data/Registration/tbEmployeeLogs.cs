using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SISystem.Data
{
    public class tbEmployeeLogs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public DateTime LogIn { get; set; }
        public DateTime LogOut { get; set; }
        public DateTime DateCreated { get; set; }
        public int TotalMinutes { get; set; }
    }
}
