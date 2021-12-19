using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class TransactionServices
    {

        public TimeSpan CalculateLeadTime(DateTime orderDate, DateTime dateCreated)
        {
            TimeSpan time = orderDate.Subtract(dateCreated);
            return time;
        }
    }
}
