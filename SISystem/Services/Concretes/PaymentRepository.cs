using Microsoft.EntityFrameworkCore;
using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly SISystemDbContext _sISystemDbContext;
        private readonly IDataTransform _dataTransform;

        public PaymentRepository(SISystemDbContext sISystemDbContext,
            IDataTransform dataTransform)
        {
            _sISystemDbContext = sISystemDbContext;
            _dataTransform = dataTransform;
        }

        public TimeSpan CalculateLeadTime(DateTime orderDate, DateTime dateCreated)
        {
            TimeSpan time = orderDate.Subtract(dateCreated);
            return time;
        }

        #region Billing

        #region Get

        public async Task<List<BillingStatementResponse>> GetBillingStatement(Guid guid)
        {
            List<BillingStatementResponse> response = await _sISystemDbContext
                                            .BillingStatementResponse
                                            .FromSql($"EXEC spBillingStatement {guid}")
                                            .ToListAsync();
            return response;
        }

        public async Task<object> GetAllPaymentDetails()
        {
            var test = await _sISystemDbContext
                .tbPaymentDetails
                .Select(x => new
                {
                    x.PaymentDetailsID,
                    x.Amount,
                    x.Branch,
                    x.CheckDate,
                    x.CheckNumber,
                    x.DepositoryBank,
                    x.PaymentType,
                    x.ReferenceDate,
                    Billing = _sISystemDbContext
                          .tbBilling.Where(y => y.BillingID == x.BillingID)
                          .Select(s => s).FirstOrDefault()
                }).ToListAsync();

            return test;
        }
        public async Task<object> GetAllBilling()
        {
            var test = await _sISystemDbContext
                .tbBilling
                .Select(x => new { 
                    x.BillingID,
                    SalesAgent = _sISystemDbContext.tbSalesAgent.Where(y => y.Id == x.SalesAgentID).Select(t => t.FullName).FirstOrDefault(),
                    x.BankDepositTotal,
                    x.BillingFrom,
                    x.BillingTo,
                    x.CashTotal,
                    x.CheckTotal,
                    x.CollectionDate,
                    x.CollectionNo,
                    x.Discount,
                    x.OverShortPayment,
                    x.Remarks,
                    x.TotalAmount,
                    x.TotalPayable,
                    x.TotalPayment,
                    x.DateCreated,
                    x.DateModified,
                    x.ModifiedByID,
                    Username = _sISystemDbContext.tbUser.Where(a => a.Id == x.UserId).Select(b => b.Username).FirstOrDefault()
                }).ToListAsync();

            return test;
        }
        #endregion

        #region Add
        public async Task<Guid> AddBillingDetails(tbBillingDetails dest)
        {
            _sISystemDbContext.tbBillingDetails.Add(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest.BillingDetailsID;
        }

        public async Task<Guid> AddBilling(tbBilling dest)
        {
            _sISystemDbContext.tbBilling.Add(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest.BillingID;
        }

        public async Task<List<tbPaymentDetails>> AddPaymentDetails(List<tbPaymentDetails> dest)
        {
            _sISystemDbContext.tbPaymentDetails.AddRange(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest;
        }
        #endregion

        #endregion

        #region CustomerOrder

        #region Update
        public async Task<int> UpdateAccountSummaryCancel(Guid id, string remarks, string prosName, Guid guid)
        {
            var data = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id && x.IsBadDebt == false && x.OrderStatusId == 5).FirstOrDefaultAsync();

            if (data != null)
            {
                data.OrderStatusId = 9;
                data.IsBadDebt = true;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);

                var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
                if (process != null)
                {
                    process.BadDebtByName = prosName;
                    process.BadDebtById = guid;
                    process.BadDebtDate = DateTime.Now;
                    process.RemarksBadDebt = remarks;
                    _sISystemDbContext.tbCustomerOrderProcessor.Update(process);
                }
            }
            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }

        public async Task<int> UpdateCustomerOrderApprove(Guid id, string prosName, Guid guid)
        {
            var data = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id).FirstOrDefaultAsync();
            
            if (data != null)
            {
                tbCustomerOrderVersion version = new tbCustomerOrderVersion()
                {
                    TransactionBy = prosName,
                    CustomerOrderId = data.Id,
                    DateCreated = DateTime.Now,
                    OrderStatusID = data.OrderStatusId
                };
                _sISystemDbContext.tbCustomerOrderVersion.Add(version);

                data.OrderStatusId = 2;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }

            var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
            if (process != null)
            {
                process.ApprovedByName = prosName;
                process.ApprovedById = guid;
                _sISystemDbContext.tbCustomerOrderProcessor.Update(process);
            }
            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }

        public async Task<int> AccountSummaryRevertCancel(Guid id)
        {
            var data = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (data != null)
            {
                data.IsBadDebt = false;
                data.OrderStatusId = 5;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }

            var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
            if (process != null)
            {
                process.BadDebtByName = null;
                process.BadDebtById = null;
                process.BadDebtDate = null;
                process.RemarksBadDebt = null;
                _sISystemDbContext.tbCustomerOrderProcessor.Update(process);
            }
            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }

        public async Task<int> UpdateCustomerOrder(Guid id, string prosName)
        {
            var orderVersion = await _sISystemDbContext.tbCustomerOrderVersion.Where(x => x.CustomerOrderId == id).OrderByDescending(y => y.DateCreated).FirstOrDefaultAsync();
            if (orderVersion != null)
            {
                var currData = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (currData != null)
                {
                    tbCustomerOrderVersion version = new tbCustomerOrderVersion()
                    {
                        TransactionBy = prosName,
                        CustomerOrderId = currData.Id,
                        DateCreated = DateTime.Now,
                        OrderStatusID = currData.OrderStatusId
                    };
                    _sISystemDbContext.tbCustomerOrderVersion.Add(version);

                    currData.OrderStatusId = orderVersion.OrderStatusID;
                    _sISystemDbContext.tbCustomerOrderMain.Update(currData);
                    await _sISystemDbContext.SaveChangesAsync();
                }
            }
            return 0;
        }

        public async Task<int> UpdateOrderMarkedAsDebt(List<Guid> dest)
        {
            foreach (var item in dest)
            {
                var data = await _sISystemDbContext.tbCustomerOrder.Where(x => x.CustomerOrderID == item).FirstOrDefaultAsync();
                if (data != null)
                {
                    data.IsBadDebt = true;
                    _sISystemDbContext.Entry(data).Property("IsBadDebt").IsModified = true;
                }
            }
            await _sISystemDbContext.SaveChangesAsync();
            return 1;
        }
        #endregion

        #region Get
        public async Task<List<CustomerOrderMainResponse>> GetAllCustomerOrderReponseCancelled()
        {
            var orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => j.IsCancelled == true).OrderBy(y => y.DateCreated).ToListAsync();
           
            var processor = orderMain
                .Join(_sISystemDbContext.tbCustomerOrderProcessor,
                id => id.Id,
                id2 => id2.CustomerOrderId,
                (id, id2) => new CustomerOrderMainResponse
                {
                    Id = id.Id,
                    CustomerId = id.CustomerId,
                    SalesAgentId = id.SalesAgentId,
                    SubSalesAgentId = id.SubSalesAgentId,
                    SalesAgentName = id.SalesAgentName,
                    OrderStatusId = id.OrderStatusId,
                    SubSalesAgentName = id.SubSalesAgentName,
                    CustomerName = id.CustomerName,
                    TinNo = id.TinNo,
                    OrderAmount = id.OrderAmount,
                    ForMonthOf = id.ForMonthOf.ToString("Y"),
                    DateCreated = id.DateCreated,
                    IsCancelled = id.IsCancelled,

                    ProcessorId = id2.Id,
                    CustomerOrderId = id2.CustomerOrderId,
                    EndcodedById = id2.EndcodedById,
                    EncodedByName = id2.EncodedByName,
                    ApprovedById = id2.ApprovedById,
                    ApprovedByName = id2.ApprovedByName,
                    OrderDate = id2.OrderDate,
                    RemarksCustomerOrder = id2.RemarksCustomerOrder,
                    CancelDate = id2.CancelDate,
                    CancelledById = id2.CancelledById,
                    CancelledByName = id2.CancelledByName

                }).OrderByDescending(y => y.OrderDate).ToList();

            return processor;
        }

        public async Task<List<CustomerOrderMainResponse>> GetAllCustomerOrderReponse(int statusId)
        {
            List<tbCustomerOrderMain> orderMain = new List<tbCustomerOrderMain>();

            if (statusId == 0)
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => j.IsCancelled == true).OrderBy(y => y.DateCreated).ToListAsync();
            }
            else if (statusId == 7)
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => (j.OrderStatusId == 1 || j.OrderStatusId == 2) && j.IsCancelled == false).OrderBy(y => y.DateCreated).ToListAsync();
            }
            else
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => j.OrderStatusId == statusId && j.IsCancelled == false).OrderBy(y => y.DateCreated).ToListAsync();
            }

            var processor = orderMain
                .Join(_sISystemDbContext.tbCustomerOrderProcessor,
                id => id.Id,
                id2 => id2.CustomerOrderId,
                (id, id2) => new CustomerOrderMainResponse
                {
                    Id = id.Id,
                    CustomerId = id.CustomerId,
                    SalesAgentId = id.SalesAgentId,
                    SubSalesAgentId = id.SubSalesAgentId,
                    SalesAgentName = id.SalesAgentName,
                    OrderStatusId = id.OrderStatusId,
                    SubSalesAgentName = id.SubSalesAgentName,
                    CustomerName = id.CustomerName,
                    TinNo = id.TinNo,
                    OrderAmount = id.OrderAmount,
                    ForMonthOf = id.ForMonthOf.ToString("Y"),
                    DateCreated = id.DateCreated,
                    IsCancelled = id.IsCancelled,

                    ProcessorId = id2.Id,
                    CustomerOrderId = id2.CustomerOrderId,
                    EndcodedById = id2.EndcodedById,
                    EncodedByName = id2.EncodedByName,
                    ApprovedById = id2.ApprovedById,
                    ApprovedByName = id2.ApprovedByName,
                    OrderDate = id2.OrderDate,
                    RemarksCustomerOrder = id2.RemarksCustomerOrder

                }).OrderByDescending(y => y.OrderDate).ToList();

            return processor;
        }

        public async Task<object> GetCustomerOrderDetailsById(Guid id)//Get By ID
        {
            return await _sISystemDbContext
                .tbCustomerOrderDetails.Where(x => x.Id == id)
                .Select(y => new
                {
                    y.IssuingCompanyId,
                    y.SalesAmount,
                    y.CRNo,
                    y.InvoiceDate,
                    y.SINo
                }).FirstOrDefaultAsync();
        }

        public async Task<List<tbCustomerOrderDetails>> GetCustomerOrderDetailsList(Guid id)//Get By ID
        {
            return await _sISystemDbContext
                .tbCustomerOrderDetails.Where(x => x.Id == id).ToListAsync();
        }

        public async Task<tbCustomerOrderVersion> GetCustomerVersion(Guid id)//Get last version
        {
            return await _sISystemDbContext
                .tbCustomerOrderVersion.Where(x => x.CustomerOrderId == id).OrderByDescending(y => y.DateCreated).FirstOrDefaultAsync();
        }

        public async Task<tbCustomerOrder> UpdateCustomerVersion(tbCustomerOrder order)//Get last version
        {
            var ords = await _sISystemDbContext.tbCustomerOrder.Where(x => x.CustomerOrderID == order.CustomerOrderID).FirstOrDefaultAsync();
            if(ords != null)
            {
                _sISystemDbContext.tbCustomerOrder.Update(order);
                await _sISystemDbContext.SaveChangesAsync();
            }
            return order;
        }

        public async Task<tbCustomerOrderMain> GetCustomerOrderById(Guid id)
        {
            return await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id).FirstOrDefaultAsync();
            
        }

        public async Task<object> GetCustomerOrderWithOrderDetails(Guid id)//Get Order with Order details
        {
            var test = await _sISystemDbContext
                .tbCustomerOrder.Where(s => s.CustomerOrderID == id)
                .Select(x => new
                {
                    CustomerName = _sISystemDbContext
                                    .tbCustomer.Where(y => y.Id == x.CustomerID).Select(i => i.CustomerName).FirstOrDefault(),
                    SalesAgent = _sISystemDbContext
                                    .tbSalesAgent.Where(a => a.Id == x.SalesAgentID).Select(b => b.FullName).FirstOrDefault(),
                    SubAgent = _sISystemDbContext
                                    .tbSalesSubAgent.Where(t => t.Id == x.SubSalesAgentID).Select(y => y.SubAgentName).FirstOrDefault(),
                    OrderStatus = _sISystemDbContext
                                    .tbOrderStatus.Where(c => c.OrderStatusID == x.OrderStatusID).Select(d => d.Status).FirstOrDefault(),
                    OrderDetails = _sISystemDbContext
                            .tbCustomerOrderDetails.Where(j => j.Id == x.CustomerOrderID)
                            .Select(k => k).FirstOrDefault(),
                    x.TransactionCode,
                    x.OrderDate,
                    x.TotalAmount,
                    x.Remarks,
                    LeadTime = _dataTransform.CalculateLeadTime(x.OrderDate, x.DateCreated),
                    ForMonthOf = x.ForMonthOf.ToString("Y"),
                    x.ReasonForCancellation,
                    x.DateCancelled,
                    x.IsBadDebt,
                    x.IsBilled,
                    x.DateCreated
                }).FirstOrDefaultAsync();

            return test;
        }
        public async Task<object> GetAllCustomerOrder()
        {
            var test = await _sISystemDbContext
                .tbCustomerOrder
                .Select(x => new
                {
                    x.CustomerOrderID,
                    x.CustomerID,
                    CustomerName = _sISystemDbContext
                                    .tbCustomer.Where(y => y.Id == x.CustomerID).Select(i => i.CustomerName).FirstOrDefault(),
                    x.SalesAgentID,
                    SalesAgent = _sISystemDbContext
                                    .tbSalesAgent.Where(a => a.Id == x.SalesAgentID).Select(b => b.FullName).FirstOrDefault(),
                    x.SubSalesAgentID,
                    SubAgent = _sISystemDbContext
                                    .tbSalesSubAgent.Where(t => t.Id == x.SubSalesAgentID).Select(y => y.SubAgentName).FirstOrDefault(),
                    Tin = _sISystemDbContext
                               .tbCustomer.Where(y => y.Id == x.CustomerID).Select(i => i.TIN).FirstOrDefault(),
                    x.TotalAmount,
                    ForMonthOf = x.ForMonthOf.ToString("Y"),
                    x.UserId,
                    CreatedBy = "Test",
                    x.ForMonthOf.Month,
                    LeadTime = _dataTransform.CalculateLeadTime(x.OrderDate, x.DateCreated),
                    x.DateCreated,
                    x.OrderDate,
                    x.MarkDeliveredDate
                }).OrderByDescending(z => z.ForMonthOf).ToListAsync();

            return test;
        }

        public async Task<object> GetAllCustomerOrderByStatus(int orderStatus)
        {
            var test = await _sISystemDbContext
                .tbCustomerOrder
                .Where(q => q.OrderStatusID == orderStatus)
                .Select(x => new
                {
                    x.CustomerOrderID,
                    x.CustomerID,
                    CustomerName = _sISystemDbContext
                                    .tbCustomer.Where(y => y.Id == x.CustomerID).Select(i => i.CustomerName).FirstOrDefault(),
                    x.SalesAgentID,
                    SalesAgent = _sISystemDbContext
                                    .tbSalesAgent.Where(a => a.Id == x.SalesAgentID).Select(b => b.FullName).FirstOrDefault(),
                    x.SubSalesAgentID,
                    SubAgent = _sISystemDbContext
                                    .tbSalesSubAgent.Where(t => t.Id == x.SubSalesAgentID).Select(y => y.SubAgentName).FirstOrDefault(),
                    Tin = _sISystemDbContext
                               .tbCustomer.Where(y => y.Id == x.CustomerID).Select(i => i.TIN).FirstOrDefault(),
                    x.TotalAmount,
                    ForMonthOf = x.ForMonthOf.ToString("Y"),
                    x.UserId,
                    CreatedBy = "Test",
                    x.ForMonthOf.Month,
                    LeadTime = _dataTransform.CalculateLeadTime(x.OrderDate, x.DateCreated),
                    x.DateCreated,
                    x.OrderDate,
                    x.MarkDeliveredDate
                }).OrderByDescending(z => z.ForMonthOf).ToListAsync();

            return test;
        }
        #endregion

        #region Add
        public async Task<Guid> AddtbCustomerOrderMain(tbCustomerOrderMain dest)
        {
            _sISystemDbContext.tbCustomerOrderMain.Add(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest.Id;
        }
        public async Task<Guid> AddtbCustomerOrderProcessor(tbCustomerOrderProcessor dest)
        {
            _sISystemDbContext.tbCustomerOrderProcessor.Add(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest.Id;
        }

        public async Task<Guid> AddCustomerOrder(tbCustomerOrder dest)
        {
            _sISystemDbContext.tbCustomerOrder.Add(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest.CustomerOrderID;
        }

        public async Task<List<tbCustomerOrder>> AddCustomerOrderList(List<tbCustomerOrder> dest)
        {
            _sISystemDbContext.tbCustomerOrder.AddRange(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest;
        }

        public async Task<List<tbCustomerOrderDetails>> AddCustomerOrderDetailsList(List<tbCustomerOrderDetails> dest)
        {
            _sISystemDbContext.tbCustomerOrderDetails.AddRange(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest;
        }

        public async Task<Guid> AddCustomerOrderDetails(tbCustomerOrderDetails dest)
        {
            _sISystemDbContext.tbCustomerOrderDetails.Add(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest.Id;
        }
        public async Task<Guid> AddCustomerOrderVersion(tbCustomerOrderVersion dest)
        {
            _sISystemDbContext.tbCustomerOrderVersion.Add(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest.CustomerOrderId;
        }
        #endregion

        #endregion
    }
}
