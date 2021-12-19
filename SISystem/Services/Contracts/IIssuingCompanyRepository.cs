using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface IIssuingCompanyRepository
    {
        Task<tbIssuingCompany> GetIssuingCompanyChecker(string tin);
        Task<IEnumerable<IssuingCompanyReponse>> GetIssuingCompanyById(Guid id);
        Task<IEnumerable<IssuingCompanyReponse>> GetIssuingCompany();
        Task<IEnumerable<IssuingCompanyReponse>> GetIssuingCompanyByStatus(string status);
        Task<int> SaveIssuingCompany(tbIssuingCompany req);
        Task<IEnumerable<tbCompanyType>> GetCompanyType();
        Task<int> SaveCompanyType(tbCompanyType req);
        Task<tbIssuingCompany> UpdateIssuingCompany(tbIssuingCompany company);
        Task<tbCompanyType> UpdateCompanyType(tbCompanyType company);
        Task<int> DeleteIssuingCompany(Guid guid);
    }
}
