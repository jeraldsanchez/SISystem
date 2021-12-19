using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface IReportRepository
    {
        Task<List<ReportSOAResponse>> GetReportSOA(Guid guid);
    }
}
