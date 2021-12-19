using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface ITransactionServices
    {
        TimeSpan CalculateLeadTime(DateTime orderDate, DateTime dateCreated);
    }
}
