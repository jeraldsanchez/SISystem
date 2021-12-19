using Microsoft.EntityFrameworkCore;
using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SISystemDbContext _sISystemDbContext;

        public CustomerRepository(SISystemDbContext sISystemDbContext)
        {
            _sISystemDbContext = sISystemDbContext;
        }

        public async Task<List<tbCustomer>> GetAllCustomer()
        {
            return await _sISystemDbContext.tbCustomer.ToListAsync();
        }

        public async Task<tbCustomer> GetAllCustomerById(Guid id)
        {
            return await _sISystemDbContext.tbCustomer.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Object> GetAllCustomerMultipleByStatus(string status)
        {
            var fullEntries = _sISystemDbContext.tbCustomer.Where(u => u.Status.ToLower() == status.ToLower())
                        .Join(_sISystemDbContext.tbCustomerSalesAgent.Where(t => t.IsActive == true),
                                id => id.Id,
                                id2 => id2.CustomerId,
                            (id, id2) => new { id, id2 }
                        )
                        .Join(
                            _sISystemDbContext.tbSalesAgent,
                            combinedEntry => combinedEntry.id2.SalesAgentId,
                            title => title.Id,
                            (combinedEntry, title) => new CustomerResponse
                            {
                                CustomerID = combinedEntry.id.Id,
                                CustomerName = combinedEntry.id.CustomerName,
                                CustomerAddress = combinedEntry.id.CustomerAddress,
                                TIN = combinedEntry.id.TIN,
                                Status = combinedEntry.id.Status,
                                DateCreated = combinedEntry.id.DateCreated,

                                CustomerSalesId = combinedEntry.id2.Id,
                                SalesAgentId = combinedEntry.id2.SalesAgentId,
                                OverrideRate = combinedEntry.id2.OverrideRate,
                                DateSalesCreated = combinedEntry.id2.DateCreated,

                                SalesAgentName = title.FullName,
                                EmailAddress1 = title.EmailAddress1,
                                MobileNo1 = title.MobileNo1
                            }
                        ).OrderBy(j => j.CustomerName);

            return fullEntries;
        }

        public async Task<Object> GetAllCustomerMultiple()
        {
             var fullEntries = _sISystemDbContext.tbCustomer
                        .Join(_sISystemDbContext.tbCustomerSalesAgent.Where(t => t.IsActive == true),
                                id => id.Id,
                                id2 => id2.CustomerId,
                            (id, id2) => new { id, id2 }
                        )
                        .Join(
                            _sISystemDbContext.tbSalesAgent,
                            combinedEntry => combinedEntry.id2.SalesAgentId,
                            title => title.Id,
                            (combinedEntry, title) => new CustomerResponse
                            {
                                CustomerID = combinedEntry.id.Id,
                                CustomerName = combinedEntry.id.CustomerName,
                                CustomerAddress = combinedEntry.id.CustomerAddress,
                                TIN = combinedEntry.id.TIN,
                                Status = combinedEntry.id.Status,
                                DateCreated = combinedEntry.id.DateCreated,

                                CustomerSalesId = combinedEntry.id2.Id,
                                SalesAgentId = combinedEntry.id2.SalesAgentId,
                                OverrideRate = combinedEntry.id2.OverrideRate,
                                DateSalesCreated = combinedEntry.id2.DateCreated,

                                SalesAgentName = title.FullName,
                                EmailAddress1 = title.EmailAddress1,
                                MobileNo1 = title.MobileNo1
                            }
                        ).OrderBy( j => j.CustomerName);

            return fullEntries;
        }

        public async Task<Guid> AddCustomer(tbCustomer cust)
        {
            cust.Status = "Active";
            cust.DateCreated = DateTime.Now;
            _sISystemDbContext.tbCustomer.Add(cust);
            await _sISystemDbContext.SaveChangesAsync();
            return cust.Id;
        }
        public async Task<bool> AddCustomerSalesAgentMultiple(List<tbCustomerSalesAgent> agent)
        {
            _sISystemDbContext.tbCustomerSalesAgent.AddRange(agent);
            await _sISystemDbContext.SaveChangesAsync();
            
            return true;
        }

        public async Task<tbCustomer> UpdateCustomer(tbCustomer cu)
        {
            tbCustomer cust = await _sISystemDbContext.tbCustomer.Where(x => x.Id == cu.Id).FirstOrDefaultAsync();
            if (cust != null)
            {
                cust.TIN = cu.TIN;
                cust.CustomerAddress = cu.CustomerAddress;
                cust.CustomerName = cu.CustomerName;
                _sISystemDbContext.tbCustomer.Update(cust);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return cust;
        }

        public async Task<List<tbCustomerSalesAgent>> UpdateSalesCustomer(List<tbCustomerSalesAgent> cu)
        {
            _sISystemDbContext.tbCustomerSalesAgent.UpdateRange(cu);
            await _sISystemDbContext.SaveChangesAsync();
            return cu;
        }

        public async Task<int> DeleteCustomer(Guid id)
        {
            tbCustomer cust = await _sISystemDbContext.tbCustomer.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (cust != null)
            {
                cust.Status = "Retired";
                _sISystemDbContext.tbCustomer.Update(cust);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteSalesAgent(Guid id, Guid salesAgentId)
        {
            tbCustomerSalesAgent cust = await _sISystemDbContext.tbCustomerSalesAgent.Where(x => x.CustomerId == id && x.SalesAgentId == salesAgentId).FirstOrDefaultAsync();
            if (cust != null)
            {
                cust.IsActive = false;
                _sISystemDbContext.tbCustomerSalesAgent.Update(cust);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerSalesAgentResponse>> GetCustomerSalesAgent(Guid id)//SalesAgentId
        {
            List<CustomerSalesAgentResponse> AllForm = new List<CustomerSalesAgentResponse>();
            return AllForm;
        }

        public async Task<IEnumerable<CustomerSalesAgentResponse>> GetCustomerSalesAgents2()
        {
            List<CustomerSalesAgentResponse> AllForm = new List<CustomerSalesAgentResponse>();
            return AllForm;
         }

        public async Task<Guid> AddCustomerSalesAgent(tbCustomerSalesAgent agent)
        {
            tbCustomer cust = await _sISystemDbContext.tbCustomer.Where(x => x.Id == agent.CustomerId).FirstOrDefaultAsync();
            if (cust != null)
            {
                _sISystemDbContext.tbCustomerSalesAgent.Add(agent);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return agent.Id;
        }

        public async Task<tbCustomerSalesAgent> UpdateCustomerSalesAgent(tbCustomerSalesAgent ag)
        {
            tbCustomerSalesAgent agent = await _sISystemDbContext.tbCustomerSalesAgent.Where(x => x.Id == ag.Id && x.CustomerId == ag.CustomerId).FirstOrDefaultAsync();
            if (agent != null)
            {
                _sISystemDbContext.tbCustomerSalesAgent.Update(agent);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return agent;
        }

        public async Task<int> DeleteCustomerSalesAgent(Guid id)
        {
            tbCustomerSalesAgent agent = await _sISystemDbContext.tbCustomerSalesAgent.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (agent != null)
            {
                //agent.IsRetired = true;
                _sISystemDbContext.tbCustomerSalesAgent.Update(agent);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
    }
}
