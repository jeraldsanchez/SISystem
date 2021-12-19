using Microsoft.EntityFrameworkCore;
using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class SalesAgentRepository : ISalesAgentRepository
    {
        private readonly SISystemDbContext _sISystemDbContext;

        public SalesAgentRepository(SISystemDbContext sISystemDbContext)
        {
            _sISystemDbContext = sISystemDbContext;
        }

        public async Task<IEnumerable<CustomerSalesAgentResponse>> GetCustomerSalesAgent(Guid idx)
        {
            var result = await _sISystemDbContext.tbSalesAgent.Where(y => y.Id == idx).Select(x => x).ToListAsync();
            var test = result.Join(_sISystemDbContext.tbCustomerSalesAgent,
                id => id.Id,
                id2 => id2.SalesAgentId,
                (id, id2) => new
                {
                    salesAgent = id,
                    customerSales = id2
                }).ToList();

            var result2 = await _sISystemDbContext.tbCustomer.Select(x => x).ToListAsync();
            var AllForm = test.SelectMany(c => result2.Where(o => o.Id == c.customerSales.CustomerId),
                (c, o) => new {
                    c,
                    o
                }).GroupBy(c => c.c.salesAgent.Id)
                     .Select(c => new CustomerSalesAgentResponse
                     {
                         Agent = c.Select(h => h.c.salesAgent).FirstOrDefault(),
                         Customers = c.Select(z => new CustomerResponseSalesAgent
                         {
                             CustomerId = z.o.Id,
                             SalesAgentId = z.c.salesAgent.Id,
                             CustomerAddress = z.o.CustomerAddress,
                             CustomerName = z.o.CustomerName,
                             DateCreated = z.o.DateCreated,
                             Status = z.o.Status,
                             TIN = z.o.TIN,
                             SalesFullName = z.c.salesAgent.FullName,
                             SalesRate = z.c.salesAgent.Rate,
                             SalesTerms = z.c.salesAgent.Terms,
                             SubAgentId = _sISystemDbContext.tbSalesSubAgent.Where(q => q.SalesAgentId == z.c.salesAgent.Id)
                                                    .Select(t => t.Id).FirstOrDefault()
                         }).ToList()
                     });

            return AllForm;
        }
        public async Task<IEnumerable<CustomerSalesAgentResponse>> GetCustomerSalesAgents2()
        {
            var result = await _sISystemDbContext.tbSalesAgent.Select(x => x).ToListAsync();
            var test = result.Join(_sISystemDbContext.tbCustomerSalesAgent,
                id => id.Id,
                id2 => id2.SalesAgentId,
                (id, id2) => new 
                {
                    salesAgent = id,
                    customerSales = id2
                }).ToList();

            var result2 = await _sISystemDbContext.tbCustomer.Select(x => x).ToListAsync();
            var AllForm = test.SelectMany(c => result2.Where(o => o.Id == c.customerSales.CustomerId),
                (c, o) => new {
                    c,
                    o
                }).GroupBy(c => c.c.salesAgent.Id)
                     .Select(c => new CustomerSalesAgentResponse
                     {
                         Agent = c.Select(h => h.c.salesAgent).FirstOrDefault(),
                         Customers = c.Select(z => new CustomerResponseSalesAgent { 
                                        CustomerId = z.o.Id,
                                        SalesAgentId = z.c.salesAgent.Id,
                                        CustomerAddress = z.o.CustomerAddress,
                                        CustomerName = z.o.CustomerName,
                                        DateCreated = z.o.DateCreated,
                                        Status = z.o.Status,
                                        TIN = z.o.TIN,
                                        SalesFullName = z.c.salesAgent.FullName,
                                        SalesRate = z.c.salesAgent.Rate,
                                        SalesTerms = z.c.salesAgent.Terms,
                                        SubAgentId = _sISystemDbContext.tbSalesSubAgent.Where(q => q.SalesAgentId == z.c.salesAgent.Id)
                                                                .Select(t => t.Id).FirstOrDefault()
                         }).ToList()
                     });

            return AllForm;
        }

        #region Agents
        public async Task<List<tbSalesAgent>> GetAllActiveSalesAgent(string status)
        {
            return await _sISystemDbContext.tbSalesAgent.Where(x => x.Status.ToLower() == status.ToLower()).OrderBy(y => y.FullName).ToListAsync();
        }
        public async Task<List<tbSalesAgent>> GetAllSalesAgent()
        {
            return await _sISystemDbContext.tbSalesAgent.OrderBy(y => y.FullName).ToListAsync();
        }
        public async Task<tbSalesAgent> GetSalesAgentById(Guid req)
        {
            return await _sISystemDbContext.tbSalesAgent.Where(x => x.Id == req).FirstOrDefaultAsync();
        }
        public async Task<tbSalesAgent> GetSalesAgentChecker(string mobileNo)
        {
            return await _sISystemDbContext.tbSalesAgent.Where(x => x.MobileNo1 == mobileNo || x.MobileNo2 == mobileNo || x.MobileNo3 == mobileNo).FirstOrDefaultAsync();
        }
        public async Task<Guid> SaveSalesAgent(tbSalesAgent req)
        {
            req.Status = "Active";
            req.DateCreated = DateTime.Now;
            _sISystemDbContext.tbSalesAgent.Add(req);
            await _sISystemDbContext.SaveChangesAsync();
            return req.Id;
        }
        public async Task<tbSalesAgent> UpdateSalesAgent(tbSalesAgent req)
        {
            tbSalesAgent agent = await _sISystemDbContext.tbSalesAgent.Where(x => x.Id == req.Id && x.Status.ToLower() == "active").FirstOrDefaultAsync();
            if (agent != null)
            {
                agent.MobileNo1 = req.MobileNo1;
                agent.MobileNo2 = req.MobileNo2;
                agent.MobileNo3 = req.MobileNo3;
                agent.FullName = req.FullName;
                agent.EmailAddress1 = req.EmailAddress1;
                agent.EmailAddress2 = req.EmailAddress2;
                agent.EmailAddress3 = req.EmailAddress3;
                agent.Rate = req.Rate;
                agent.Terms = req.Terms;
                _sISystemDbContext.tbSalesAgent.Update(agent);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return agent;
        }
        public async Task<int> DeleteSalesAgent(Guid guid)
        {
            tbSalesAgent agent = await _sISystemDbContext.tbSalesAgent.Where(x => x.Id == guid && x.Status.ToLower() == "active").FirstOrDefaultAsync();
            if (agent != null)
            {
                //Check If have sales, if yes retire, if no delete
                agent.Status = "Retired";
                _sISystemDbContext.tbSalesAgent.Update(agent);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<List<tbCustomerSalesAgent>> UpdateOverrideRemarks(List<tbCustomerSalesAgent> agent)
        {
            foreach (var item in agent)
            {
               var salesAgent = await _sISystemDbContext.tbSalesAgent.Select(y => new { y.Rate, y.Id}).Where(x => x.Id == item.SalesAgentId).FirstOrDefaultAsync();
                if (item.OverrideRate == salesAgent.Rate)
                {
                    item.IsOverride = true;
                }
                else
                {
                    item.IsOverride = false;
                }
            }
            return agent;
        }

        #endregion

        #region Sub Agents
        public async Task<Guid> SaveSubSalesAgent(tbSalesSubAgent req)
        {
            req.Status = "Active";
            req.DateCreated = DateTime.Now;
            _sISystemDbContext.tbSalesSubAgent.Add(req);
            await _sISystemDbContext.SaveChangesAsync();
            return req.Id;
        }
        public async Task<int> DeleteSubSalesAgent(Guid agentId, Guid subAgentId)
        {
            tbSalesSubAgent agent = await _sISystemDbContext.tbSalesSubAgent.Where(x => x.Id == subAgentId && x.SalesAgentId == agentId && x.Status.ToLower() == "active").FirstOrDefaultAsync();
            if (agent != null)
            {
                //Check If have sales, if yes retire, if no delete
                agent.Status = "Retired";
                _sISystemDbContext.tbSalesSubAgent.Update(agent);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        public async Task<tbSalesSubAgent> UpdateSubSalesAgent(tbSalesSubAgent req)
        {
            tbSalesSubAgent agent = await _sISystemDbContext.tbSalesSubAgent.Where(x => x.Id == req.Id && x.SalesAgentId == req.SalesAgentId && x.Status.ToLower() == "active").FirstOrDefaultAsync();
            if (agent != null)
            {
                agent.SubAgentName = req.SubAgentName;
                _sISystemDbContext.tbSalesSubAgent.Update(agent);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return agent;
        }

        public async Task<List<tbSalesSubAgent>> GetSubAgentsByAgentId(Guid id)
        {
            return await _sISystemDbContext.tbSalesSubAgent.Where(x => x.SalesAgentId == id && x.Status.ToLower() == "active").OrderBy(y => y.SalesAgentId).ToListAsync();
        }
        public async Task<List<tbSalesSubAgent>> GetSubAgents(Guid id)
        {
            return await _sISystemDbContext.tbSalesSubAgent.Where(x => x.Id == id && x.Status.ToLower() == "active").OrderBy(y => y.SalesAgentId).ToListAsync();
        }

        public async Task<List<SubAgentResponse>> GetAllSubAgents()
        {
            var sub = await _sISystemDbContext.tbSalesSubAgent.Select(x => x).OrderBy(y => y.SalesAgentId).ToListAsync();

            var agent = sub
                .Join(_sISystemDbContext.tbSalesAgent,
                id => id.SalesAgentId,
                id2 => id2.Id,
                (id, id2) => new SubAgentResponse
                {
                    Id = id.Id,
                    SubAgentName = id.SubAgentName,
                    AgentName = id2.FullName,
                    Status = id.Status,
                    AgentId = id2.Id
                }).OrderBy(y => y.AgentName).ToList();

            return agent;
        }

        public async Task<List<SubAgentResponse>> GetAllSubAgentsActive(string status)
        {
            var sub = await _sISystemDbContext.tbSalesSubAgent.Select(x => x).Where(s => s.Status.ToLower() == status.ToLower()).OrderBy(y => y.SalesAgentId).ToListAsync();

            var agent = sub
                .Join(_sISystemDbContext.tbSalesAgent,
                id => id.SalesAgentId,
                id2 => id2.Id,
                (id, id2) => new SubAgentResponse
                {
                    Id = id.Id,
                    SubAgentName = id.SubAgentName,
                    AgentName = id2.FullName,
                    Status = id.Status,
                    AgentId = id2.Id
                }).OrderBy(y => y.AgentName).ToList();

            return agent;
        }
        #endregion

        #region Agents Contacts
        public async Task<Guid> SaveSalesContact(tbSalesAgentContact req)
        {
            req.IsActive = true;
            req.DateCreated = DateTime.Now;
            _sISystemDbContext.tbSalesAgentContact.Add(req);
            await _sISystemDbContext.SaveChangesAsync();
            return req.Id;
        }
        public async Task<int> DeleteSalesContact(tbSalesAgentContact req)
        {
            tbSalesAgentContact agent = await _sISystemDbContext.tbSalesAgentContact.Where(x => x.Id == req.Id && x.SalesAgentId == req.SalesAgentId && x.IsActive).FirstOrDefaultAsync();
            if (agent != null)
            {
                agent.IsActive = false;
                _sISystemDbContext.tbSalesAgentContact.Update(agent);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        public async Task<tbSalesAgentContact> UpdateSalesContact(tbSalesAgentContact req)
        {
            tbSalesAgentContact agent = await _sISystemDbContext.tbSalesAgentContact.Where(x => x.Id == req.Id && x.SalesAgentId == req.SalesAgentId && x.IsActive).FirstOrDefaultAsync();
            if (agent != null)
            {
                agent.IsActive = req.IsActive;
                agent.Type = req.Type;
                agent.Value = req.Value;
                _sISystemDbContext.tbSalesAgentContact.Update(agent);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return agent;
        }
        public async Task<List<tbSalesAgentContact>> GetSalesAgentContact(Guid id)
        {
            return await _sISystemDbContext.tbSalesAgentContact.Where(x => x.SalesAgentId == id && x.IsActive).ToListAsync();
        }
        #endregion
    }
}
