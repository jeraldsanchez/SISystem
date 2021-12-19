using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface ICustomerRepository
    {
        Task<List<tbCustomerSalesAgent>> UpdateSalesCustomer(List<tbCustomerSalesAgent> cu);
        Task<int> DeleteSalesAgent(Guid id, Guid salesAgentId);
        Task<Object> GetAllCustomerMultipleByStatus(string status);
        Task<Object> GetAllCustomerMultiple();
        Task<List<tbCustomer>> GetAllCustomer();
        Task<tbCustomer> GetAllCustomerById(Guid id);
        Task<Guid> AddCustomer(tbCustomer cust);
        Task<tbCustomer> UpdateCustomer(tbCustomer cu);
        Task<int> DeleteCustomer(Guid id);
        Task<IEnumerable<CustomerSalesAgentResponse>> GetCustomerSalesAgent(Guid id);
        Task<Guid> AddCustomerSalesAgent(tbCustomerSalesAgent agent);
        Task<tbCustomerSalesAgent> UpdateCustomerSalesAgent(tbCustomerSalesAgent ag);
        Task<int> DeleteCustomerSalesAgent(Guid id);
        Task<IEnumerable<CustomerSalesAgentResponse>> GetCustomerSalesAgents2();
        Task<bool> AddCustomerSalesAgentMultiple(List<tbCustomerSalesAgent> agent);
    }
}
