using System;

namespace SISystem.Models
{
    public class EmployeeLogsResponse
    {
        public DateTime LogIn { get; set; }
        public DateTime LogOut { get; set; }
        public DateTime DateCreated { get; set; }
        public int TotalMinutes { get; set; }
    }
}
