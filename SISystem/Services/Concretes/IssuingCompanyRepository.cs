using Microsoft.EntityFrameworkCore;
using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class IssuingCompanyRepository : IIssuingCompanyRepository
    {
        private readonly SISystemDbContext _sISystemDbContext;

        public IssuingCompanyRepository(SISystemDbContext sISystemDbContext)
        {
            _sISystemDbContext = sISystemDbContext;
        }


        #region Get Issuing Company

        public async Task<tbIssuingCompany> GetIssuingCompanyChecker(string tin)
        {
            tbIssuingCompany com = await _sISystemDbContext.tbIssuingCompany.Where(x => x.TIN == tin).FirstOrDefaultAsync();
            return com;
        }

        public async Task<IEnumerable<IssuingCompanyReponse>> GetIssuingCompanyById(Guid idx)//string username
        {
            var issuing = await _sISystemDbContext.tbIssuingCompany.Where(x => x.Id == idx).OrderBy(j => j.CompanyName).ToListAsync();
            var issuingComplete = issuing
               .Join(_sISystemDbContext.tbCompanyType,
                id => id.CompanyTypeID,
               id2 => id2.Id,
               (id, id2) => new IssuingCompanyReponse
               {
                   Id = id.Id,
                   Type = id2.Type,
                   Address = id.Address,
                   CompanyName = id.CompanyName,
                   CompanyTypeId = id2.Id,
                   BIRRegDate = id.BIRRegDate,
                   Status = id.Status,
                   RDO = id.RDO,
                   RetireDate = id.RetireDate,
                   TIN = id.TIN,
                   ZipCode = id.ZipCode
               }).ToList();

            return issuingComplete;
        }
        public async Task<IEnumerable<IssuingCompanyReponse>> GetIssuingCompany()//string username
        {
            var issuing = await _sISystemDbContext.tbIssuingCompany.OrderBy(j => j.CompanyName).ToListAsync();
            var issuingComplete = issuing
               .Join(_sISystemDbContext.tbCompanyType,
                id => id.CompanyTypeID,
               id2 => id2.Id,
               (id, id2) => new IssuingCompanyReponse
               {
                   Id = id.Id,
                   Type = id2.Type,
                   Address = id.Address,
                   CompanyName = id.CompanyName,
                   CompanyTypeId = id2.Id,
                   BIRRegDate = id.BIRRegDate,
                   Status = id.Status,
                   RDO = id.RDO,
                   RetireDate = id.RetireDate,
                   TIN = id.TIN,
                   ZipCode = id.ZipCode
               }).ToList();

            return issuingComplete;
        }

        public async Task<IEnumerable<IssuingCompanyReponse>> GetIssuingCompanyByStatus(string status)//string username
        {
            var issuing = await _sISystemDbContext.tbIssuingCompany.Where(x => x.Status.ToLower() == status.ToLower()).OrderBy(j => j.CompanyName).ToListAsync();
            var issuingComplete = issuing
               .Join(_sISystemDbContext.tbCompanyType,
                id => id.CompanyTypeID,
               id2 => id2.Id,
               (id, id2) => new IssuingCompanyReponse
               {
                   Id = id.Id,
                   Type = id2.Type,
                   Address = id.Address,
                   CompanyName = id.CompanyName,
                   CompanyTypeId = id2.Id,
                   BIRRegDate = id.BIRRegDate,
                   Status = id.Status,
                   RDO = id.RDO,
                   RetireDate = id.RetireDate,
                   TIN = id.TIN,
                   ZipCode = id.ZipCode
               }).ToList();

            return issuingComplete;
        }

        public async Task<int> SaveIssuingCompany(tbIssuingCompany req)
        {
            _sISystemDbContext.tbIssuingCompany.Add(req);
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<tbCompanyType>> GetCompanyType()
        {
            return await _sISystemDbContext.tbCompanyType.ToListAsync();
        }

        public async Task<int> SaveCompanyType(tbCompanyType req)
        {
            _sISystemDbContext.tbCompanyType.Add(req);
            return await _sISystemDbContext.SaveChangesAsync();
        }

        public async Task<tbIssuingCompany> UpdateIssuingCompany(tbIssuingCompany company)
        {
            tbIssuingCompany com = await _sISystemDbContext.tbIssuingCompany.Where(x => x.Id == company.Id).FirstOrDefaultAsync();
            if (com != null)
            {
                com.CompanyName = company.CompanyName;
                com.Address = company.Address;
                com.RDO = company.RDO;
                com.TIN = company.TIN;
                com.ZipCode = company.ZipCode;
                com.BIRRegDate = company.BIRRegDate;
                _sISystemDbContext.tbIssuingCompany.Update(com);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return com; 
        }

        public async Task<tbCompanyType> UpdateCompanyType(tbCompanyType company)
        {
            tbCompanyType com = await _sISystemDbContext.tbCompanyType.Where(x => x.Id == company.Id).FirstOrDefaultAsync();
            if (com != null)
            {
                com.Type = company.Type;
                _sISystemDbContext.tbCompanyType.Update(com);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return com;
        }

        public async Task<int> DeleteIssuingCompany(Guid guid)
        {
            tbIssuingCompany com = await _sISystemDbContext.tbIssuingCompany.Where(x => x.Id == guid).FirstOrDefaultAsync();
            if (com != null)
            {
                //check if retire or delete
                com.Status = "Retired";
                com.RetireDate = DateTime.Now;
                _sISystemDbContext.tbIssuingCompany.Update(com);
            }
            return await _sISystemDbContext.SaveChangesAsync();
        }
        #endregion
    }   
}
