using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface IDataTransform
    {
        string CalculateLeadTime(DateTime orderDate, DateTime dateCreated);
        tbCustomerOrderVersion GetData(tbCustomerOrderMain or, string revertBy);
        string GetTransactionCode();
        Guid? NullSubAgent(Guid? id);
        tbCustomerOrderMain GetDataCustomerOrderPage(CustomerOrderMainRequest or);
        tbCustomerOrderProcessor GetDataCustomerOrderPageProcessor(CustomerOrderMainRequest or, Guid id, string encodedName, Guid encodedId);
    }
}
