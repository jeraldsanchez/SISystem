using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface ISalesAgentRepository
    {
        Task<tbSalesAgent> GetSalesAgentChecker(string mobileNo);
        Task<IEnumerable<CustomerSalesAgentResponse>> GetCustomerSalesAgents2();
        Task<IEnumerable<CustomerSalesAgentResponse>> GetCustomerSalesAgent(Guid id);
        #region Agents
        Task<List<tbSalesAgent>> GetAllSalesAgent();
        Task<tbSalesAgent> GetSalesAgentById(Guid req);
        Task<tbSalesAgent> UpdateSalesAgent(tbSalesAgent req);
        Task<int> DeleteSalesAgent(Guid guid);
        Task<Guid> SaveSalesAgent(tbSalesAgent req);
        Task<List<tbSalesAgent>> GetAllActiveSalesAgent(string status);
        Task<List<tbCustomerSalesAgent>> UpdateOverrideRemarks(List<tbCustomerSalesAgent> agent);
        #endregion

        #region Sub Agents
        Task<Guid> SaveSubSalesAgent(tbSalesSubAgent req);
        Task<int> DeleteSubSalesAgent(Guid agentId, Guid subAgentId);
        Task<tbSalesSubAgent> UpdateSubSalesAgent(tbSalesSubAgent req);
        Task<List<tbSalesSubAgent>> GetSubAgentsByAgentId(Guid id);
        Task<List<tbSalesSubAgent>> GetSubAgents(Guid id);
        Task<List<SubAgentResponse>> GetAllSubAgents();
        Task<List<SubAgentResponse>> GetAllSubAgentsActive(string status); 
        #endregion

        #region Agents Contacts
        Task<int> DeleteSalesContact(tbSalesAgentContact req);
        Task<Guid> SaveSalesContact(tbSalesAgentContact req);
        Task<tbSalesAgentContact> UpdateSalesContact(tbSalesAgentContact req);
        Task<List<tbSalesAgentContact>> GetSalesAgentContact(Guid id);
        #endregion
    }
}
