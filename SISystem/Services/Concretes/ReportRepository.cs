using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SISystem.Services
{
    public class ReportRepository : IReportRepository
    {
        private readonly SISystemDbContext _sISystemDbContext;

        public ReportRepository(SISystemDbContext sISystemDbContext)
        {
            _sISystemDbContext = sISystemDbContext;
        }

        public async Task<List<ReportSOAResponse>> GetReportSOA(Guid guid)
        {
            List<ReportSOAResponse> response = await _sISystemDbContext
                                            .ReportSOAResponse
                                            .FromSql($"EXEC spSalesInvoiceList {guid}")
                                            .ToListAsync();
            return response;
        }
    }
}
