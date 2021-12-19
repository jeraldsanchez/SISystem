using Microsoft.EntityFrameworkCore;
using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SISystemDbContext _sISystemDbContext;
        private readonly IDataTransform _dataTransform;

        public TransactionRepository(SISystemDbContext sISystemDbContext,
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

        public async Task<List<PaymentsReceivedResponse>> GetPaymentReceived()//Join
        {
            var order = await _sISystemDbContext.tbCustomerOrderMain
                .Where(x => x.OrderStatusId == 8 &&  x.IsBadDebt == false)
                .Select(x => x).ToListAsync();

            var joinedTable = order
                .Join(_sISystemDbContext.tbPaymentParentCustomer.Where(x => x.Remarks == "Fully Paid").ToList(), p => p.Id, pc => pc.CustomerOrderId, (p, pc) => new { p, pc })
                .Join(_sISystemDbContext.tbPaymentParentDetails, ppc => ppc.pc.PaymentParentId, c => c.Id, (ppc, c) => new { ppc, c })
                .Select(x => new PaymentsReceivedResponse
                {
                    Id = x.c.Id,
                    AppliedAmount = x.c.TotalPayable, //x.TotalPayable,
                    PaidAmount = _sISystemDbContext.tbPaymentForm.Where(y => y.PaymentParentId == x.c.Id).Sum(j => j.Amount),
                    CustomerName = x.c.AgentName, //_sISystemDbContext.tbPaymentParentCustomer.Where(y => y.PaymentParentId == x.Id).Select(j => j.CustomerName).FirstOrDefault().ToString(),
                    EncodedBy = _sISystemDbContext.tbUserDetails.Where(y => y.UserId == x.c.UserId).Select(j => j.Name).FirstOrDefault().ToString(),
                    Status = x.ppc.pc.Remarks,//PaymentStatus(_sISystemDbContext.tbPaymentForm.Where(y => y.PaymentParentId == x.c.Id).Sum(j => j.Amount), x.c.TotalPayable),//_sISystemDbContext.tbPaymentParentCustomer.Where(y => y.PaymentParentId == x.Id).Select(j => j.Remarks).FirstOrDefault(),
                    //Balance = _sISystemDbContext.tbPaymentForm.Where(y => y.PaymentParentId == x.c.Id).Sum(j => j.Amount) - x.c.TotalPayable,
                    DateReceived = x.c.DateCreated,
                    TransactionCount = _sISystemDbContext.tbPaymentParentCustomer.Where(y => y.PaymentParentId == x.c.Id && x.c.Remarks == "Fully Paid").Count()
                }).ToList();

            return joinedTable;
        }

        public async Task<List<PaymentsReceivedResponse>> GetPaymentReceivedNew()//Join
        {
            var order = await _sISystemDbContext.tbPaymentParentDetails
                .Select(x => x).ToListAsync();

            var joinedTable = order.Join(_sISystemDbContext.tbPaymentParentCustomer,
                parent => parent.Id,
                customer => customer.PaymentParentId,
                (parent, customer) => new PaymentsReceivedResponse{
                    Id = parent.Id,
                    AppliedAmount = parent.TotalPayable,
                    PaidAmount = _sISystemDbContext.tbPaymentForm.Where(y => y.PaymentParentId == parent.Id).Sum(j => j.Amount),
                    CustomerName = parent.AgentName,
                    EncodedBy = _sISystemDbContext.tbUserDetails.Where(y => y.UserId == parent.UserId).Select(j => j.Name).FirstOrDefault().ToString(),
                    //Status = x.ppc.pc.Remarks,
                    //Balance = _sISystemDbContext.tbPaymentForm.Where(y => y.PaymentParentId == x.c.Id).Sum(j => j.Amount) - x.c.TotalPayable,
                    DateReceived = parent.DateCreated,
                    TransactionCount = _sISystemDbContext.tbPaymentParentCustomer.Where(y => y.PaymentParentId == parent.Id).Count()
                }).ToList();

            foreach (var item in joinedTable)
            {
                if(item.AppliedAmount == item.PaidAmount){
                    item.Status = "Fully Paid";
                }
                else if (item.AppliedAmount > item.PaidAmount){
                    item.Status = "Partial";
                }
                else if (item.AppliedAmount < item.PaidAmount){
                    item.Status = "Over Payment";
                }
            }

            List<PaymentsReceivedResponse> joinGrouped = joinedTable
             .GroupBy(x => x.Id)
             .Select(x => new PaymentsReceivedResponse{
                 Id = x.Select(y => y.Id).FirstOrDefault(),
                 AppliedAmount = x.Select(y => y.AppliedAmount).FirstOrDefault(),
                 Balance = x.Select(y => y.Balance).FirstOrDefault(),
                 CustomerName = x.Select(y => y.CustomerName).FirstOrDefault(),
                 PaidAmount = x.Select(y => y.PaidAmount).FirstOrDefault(),
                 Status = x.Select(y => y.Status).FirstOrDefault(),
                 DateReceived = x.Select(y => y.DateReceived).FirstOrDefault(),
                 EncodedBy = x.Select(y => y.EncodedBy).FirstOrDefault(),
                 TransactionCount = x.Select(y => y.TransactionCount).FirstOrDefault(),

             }).ToList();

            return joinGrouped;
        }

        public async Task<IEnumerable<PaymentSummaryResponse>> GetPaymentReceivedSummary()//
        {
            var testing = await _sISystemDbContext.tbPaymentParentDetails.Where(x => x.DateCreated <= DateTime.Now && x.DateCreated >= DateTime.Now.AddMonths(-1)).OrderBy(x => x.DateCreated).ToListAsync();

            var test2 = testing.Join(_sISystemDbContext.tbPaymentForm,
                parent => parent.Id,
                form => form.PaymentParentId,
                (parent, form) => new PaymentSummaryJoin
                {
                    ParentId = parent.Id,
                    PaymentMode = form.PaymentMode,
                    Amount = form.Amount,
                    FormId = form.Id,
                    DateCreated = parent.DateCreated
                }).ToList();

            var responses = test2.GroupBy(x => x.DateCreated.Date)
                .Select(x => new PaymentSummaryResponse
                {
                    TransactionDate = x.Key.Date,
                    TransactionDay = x.Key.DayOfWeek.ToString(),
                    Cash = x.Where(y => y.PaymentMode.ToLower() == "cash").Sum(item => item.Amount),
                    BankDeposit = x.Where(y => y.PaymentMode.ToLower() == "bank deposit").Sum(item => item.Amount),
                    Check = x.Where(y => y.PaymentMode.ToLower() == "check").Sum(item => item.Amount),
                    Total = x.Sum(item => item.Amount)
                });

            return responses;
        }

        public async Task<bool> GetSIChecker(Guid issuingCompId, string sINo)
        {
            bool si = false;
            var order = await _sISystemDbContext.tbCustomerOrderDetails.Where(x => x.IssuingCompanyId == issuingCompId && x.SINo == sINo).ToListAsync();
            if(order == null || !order.Any()){
                si = true;
            }
            return si;
        }

        public async Task<bool> GetORChecker(Guid issuingCompId, string oRNo)
        {
            bool si = false;
            var order = await _sISystemDbContext.tbCustomerOrderDetails.Where(x => x.IssuingCompanyId == issuingCompId && x.CRNo == oRNo).ToListAsync();
            if (order == null || !order.Any())
            {
                si = true;
            }
            return si;
        }

        public async Task<SOAResponse> GenerateSOA(Guid customerOrderId)
        {
            var test = await _sISystemDbContext.tbSOA.Where(x => x.DateCreated.Date == DateTime.Now.Date).ToListAsync();
            string soa = String.Empty;
            int counter = 0;
            soa = DateTime.Now.ToString(@"yyyy-MM-dd");
            counter = test.Count + 1;
            soa = $"{soa}-{counter.ToString("0000")}";

            tbSOA sOAdd = new tbSOA { 
                CustomerOrderId = customerOrderId,
                DateCreated = DateTime.Now,
                SOAGenerated = soa
            };

            _sISystemDbContext.tbSOA.Add(sOAdd);
            await _sISystemDbContext.SaveChangesAsync();
            SOAResponse response = new SOAResponse {
                GenSOA = soa
            };
            return response;
        }

        public async Task<List<InvoiceOrderDetailsResponse>> GetOrderDetailsById(Guid customerOrderId)
        {
            var customer = await _sISystemDbContext.tbCustomerOrderDetails.Where(x => x.CustomerOrderId == customerOrderId).ToListAsync();

            var issuingJoin = customer.Join(
                    _sISystemDbContext.tbIssuingCompany
                    , order => order.IssuingCompanyId
                    , issuing => issuing.Id
                    , (order, issuing) => new InvoiceOrderDetailsResponse { 
                        CRNo = order.CRNo,
                        InvoiceDate = order.InvoiceDate,
                        SalesAmount = order.SalesAmount,
                        SINo = order.SINo,
                        SupplierAddress = issuing.Address,
                        SupplierName = issuing.CompanyName,
                        TinNo = issuing.TIN
                    }).ToList();

            return issuingJoin;
        }

            public async Task<List<PaymentsReceivedResponse>> GetAllPaymentReceivedSummary()//
        {
            return await _sISystemDbContext.tbPaymentParentDetails
                .Select(x => new PaymentsReceivedResponse
                {
                    Id = x.Id,
                    AppliedAmount = x.TotalPayable,
                    PaidAmount = _sISystemDbContext.tbPaymentForm.Where(y => y.PaymentParentId == x.Id).Sum(j => j.Amount),
                    CustomerName = x.AgentName, //_sISystemDbContext.tbPaymentParentCustomer.Where(y => y.PaymentParentId == x.Id).Select(j => j.CustomerName).FirstOrDefault().ToString(),
                    EncodedBy = _sISystemDbContext.tbUserDetails.Where(y => y.UserId == x.UserId).Select(j => j.Name).FirstOrDefault().ToString(),
                    Status = PaymentStatus(_sISystemDbContext.tbPaymentForm.Where(y => y.PaymentParentId == x.Id).Sum(j => j.Amount), x.TotalPayable),//_sISystemDbContext.tbPaymentParentCustomer.Where(y => y.PaymentParentId == x.Id).Select(j => j.Remarks).FirstOrDefault(),
                    Balance = _sISystemDbContext.tbPaymentForm.Where(y => y.PaymentParentId == x.Id).Sum(j => j.Amount) - x.TotalPayable,
                    DateReceived = x.DateCreated,
                    TransactionCount = _sISystemDbContext.tbPaymentParentCustomer.Where(y => y.PaymentParentId == x.Id).Count()
                }).ToListAsync();
        }
        public string PaymentStatus(decimal payFormSum, decimal totalPayable)
        {
            decimal diff = payFormSum - totalPayable;
            string ret = string.Empty;
            if (diff < 0)
            {
                ret = "Partial";
            }
            else if (diff == 0)
            {
                ret = "Paid";
            }
            else if (diff > 0){
                ret = "Over Payment";
            }
            return ret;
        }

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

        public async Task<tbPaymentParentDetails> AddPaymentParentDetails(tbPaymentParentDetails dest)
        {
            await _sISystemDbContext.tbPaymentParentDetails.AddAsync(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest;
        }

        public async Task<tbPaymentParentCustomer> AddPaymentParentCustomer(tbPaymentParentCustomer dest)
        {
            await _sISystemDbContext.tbPaymentParentCustomer.AddAsync(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest;
        }

        public async Task<List<tbPaymentForm>> AddPaymentForm(List<tbPaymentForm> dest)
        {
            await _sISystemDbContext.tbPaymentForm.AddRangeAsync(dest);
            await _sISystemDbContext.SaveChangesAsync();
            return dest;
        }
        #endregion

        #endregion

        #region CustomerOrder

        #region Update
        public async Task<int> UpdateCustomerOrderCancel(Guid id, string prosName, Guid guid)
        {
            var data = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (data != null)
            {
                data.IsCancelled = true;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }

            var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
            if (process != null)
            {
                process.CancelledByName = prosName;
                process.CancelledById = guid;
                process.CancelDate = DateTime.Now;
                _sISystemDbContext.tbCustomerOrderProcessor.Update(process);
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

        public async Task<int> RevertCancel(Guid id)
        {
            var data = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (data != null)
            {
                data.IsCancelled = false;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }

            var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
            if (process != null)
            {
                process.CancelledByName = null;
                process.CancelledById = null;
                process.CancelDate = null;
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
                    CancelledPage = _sISystemDbContext.tbOrderStatus.Where(x => x.OrderStatusID == id.OrderStatusId).Select(y => y.OrderStatusPage).FirstOrDefault(),
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

        public async Task<List<tbPaymentParentCustomer>> GetPaymentCustomerDetails(Guid id)
        {
            return await _sISystemDbContext
                .tbPaymentParentCustomer.Where(x => x.PaymentParentId == id).ToListAsync();
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
                    //OrderStatus = _sISystemDbContext
                    //                .tbOrderStatus.Where(c => c.OrderStatusID == x.OrderStatusID).Select(d => d.Status).FirstOrDefault(),
                    //OrderDetails = _sISystemDbContext
                    //        .tbCustomerOrderDetails.Where(j => j.CustomerOrderID == x.CustomerOrderID)
                    //        .Select(k => k).FirstOrDefault(),
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

        #region Delete
        public async Task<int> DeleteCancelledCustomerOrder(Guid orderId)
        {
            var orderMain = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == orderId).FirstOrDefaultAsync();
            _sISystemDbContext.tbCustomerOrderMain.Remove(orderMain);

            var orderProcess = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == orderId).FirstOrDefaultAsync();
            _sISystemDbContext.tbCustomerOrderProcessor.Remove(orderProcess);

            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }
        #endregion

        #endregion

        #region Invoice
        public async Task<List<InvoiceMainResponse>> GetAllInvoiceReponse(int statusId)
        {
            List<tbCustomerOrderMain> orderMain = new List<tbCustomerOrderMain>();

            if (statusId == 0)
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => j.IsCancelled == true).OrderBy(y => y.DateCreated).ToListAsync();
            }
            else if (statusId == 8)
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => j.OrderStatusId == 2 || j.OrderStatusId == 3 || j.OrderStatusId == 4 && j.IsCancelled == false).OrderBy(y => y.DateCreated).ToListAsync();
            }
            else
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => j.OrderStatusId == statusId && j.IsCancelled == false).OrderBy(y => y.DateCreated).ToListAsync();
            }

            var processor = orderMain
                .Join(_sISystemDbContext.tbCustomerOrderProcessor,
                id => id.Id,
                id2 => id2.CustomerOrderId,
                (id, id2) => new InvoiceMainResponse
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
                    LeadTime = "",//._dataTransform.CalculateLeadTime((DateTime)id2.ProcessDate, (DateTime)id2.OrderDate),//Convert.ToInt32(id.LeadTime).ToString(),
                    SOANo = id.SOANo,
                    Address = _sISystemDbContext.tbCustomer.Where(x => x.Id == id.CustomerId).Select(y => y.CustomerAddress).FirstOrDefault(),
                    ProcessorId = id2.Id,
                    CustomerOrderId = id2.CustomerOrderId,
                    ProcessDate = id2.ProcessDate,
                    ProcessedById = id2.ProcessedById,
                    ProcessedByName = id2.ProcessedByName,
                    TransferDate = id2.TransferDate,
                    TransferredById = id2.TransferredById,
                    TransferredByName = id2.TransferredByName,
                    ApprovedById = id2.ApprovedById,
                    ApprovedByName = id2.ApprovedByName,
                    OrderDate = id2.OrderDate,
                    RemarksInvoice = id2.RemarksInvoice,
                    CustomerOrderDetails = OrderResponse(id.Id),
                    TotalProcessAmount = _sISystemDbContext.tbCustomerOrderDetails.Where(j => j.CustomerOrderId == id2.CustomerOrderId).Sum(i => i.SalesAmount)
                }).OrderByDescending(y => y.OrderDate).ToList();

            return processor;
        }
        public  List<InvoiceOrderDetailsResponse> OrderResponse(Guid customerOrderId)
        {
            var customer = _sISystemDbContext.tbCustomerOrderDetails.Where(x => x.CustomerOrderId == customerOrderId).ToList();

            var issuingJoin = customer.Join(
                    _sISystemDbContext.tbIssuingCompany
                    , order => order.IssuingCompanyId
                    , issuing => issuing.Id
                    , (order, issuing) => new InvoiceOrderDetailsResponse
                    {
                        CRNo = order.CRNo,
                        InvoiceDate = order.InvoiceDate,
                        SalesAmount = order.SalesAmount,
                        SINo = order.SINo,
                        SupplierAddress = issuing.Address,
                        SupplierName = issuing.CompanyName,
                        TinNo = issuing.TIN,
                        Id = order.Id,
                        CustomerOrderId = order.CustomerOrderId,
                        IssuingCompanyId = order.IssuingCompanyId,
                        SalesAgentId = order.SalesAgentId
                    }).ToList();

            return issuingJoin;
        }
        public async Task<int> UpdateApproveInvoice(Guid id, string prosName, Guid guid)
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

                data.OrderStatusId = 4;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }

            var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
            if (process != null)
            {
                process.TransferredByName = prosName;
                process.TransferredById = guid;
                process.TransferDate = DateTime.Now;
                _sISystemDbContext.tbCustomerOrderProcessor.Update(process);
            }
            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }
        public async Task<int> UpdateProcessInvoice(Guid id, string sOANo, string prosName, Guid guid)
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

                var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
                if (process != null)
                {
                    process.ProcessedByName = prosName;
                    process.ProcessedById = guid;
                    process.ProcessDate = DateTime.Now;//Change
                    _sISystemDbContext.tbCustomerOrderProcessor.Update(process);
                }

                data.SOANo = sOANo;
                data.OrderStatusId = 3;
                TimeSpan diff = process.ProcessDate.Value - process.OrderDate.Value;
                data.LeadTime = (int)diff.TotalDays;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }

            
            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }
        public async Task<int> UpdateProcessInvoicePut(Guid id, string prosName, Guid guid)///////////////////////////
        {
            var data = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (data != null)
            {
                var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
                if (process != null)
                {
                    process.ProcessedByName = prosName;
                    process.ProcessedById = guid;
                    process.ProcessDate = DateTime.Now;
                    _sISystemDbContext.tbCustomerOrderProcessor.Update(process);
                }

                //data.OrderStatusId = 3;
                TimeSpan diff = process.ProcessDate.Value - process.OrderDate.Value;
                data.LeadTime = (int)diff.TotalDays;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }

            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }

        //public async Task<int> DeleteProcessInvoicePut(Guid id)
        //{
        //    var data = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == id).FirstOrDefaultAsync();

        //    if (data != null)
        //    {
        //        var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();

        //        data.SOANo = sOANo;
        //        data.OrderStatusId = 3;
        //        TimeSpan diff = process.ProcessDate.Value - process.OrderDate.Value;
        //        data.LeadTime = (int)diff.TotalDays;
        //        _sISystemDbContext.tbCustomerOrderMain.Update(data);
        //    }

        //    await _sISystemDbContext.SaveChangesAsync();
        //    return 0;
        //}

        #endregion

        #region For Delivery
        public async Task<int> UpdateForDelivery(Guid id, string prosName, Guid guid)
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

                data.OrderStatusId = 5;
                data.IsPaid = false;
                data.IsBadDebt = false; 
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }

            var process = await _sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.CustomerOrderId == id).FirstOrDefaultAsync();
            if (process != null)
            {
                process.MarkDeliveredByName = prosName;
                process.MarkDeliveredById = guid;
                process.MarkDeliverDate = DateTime.Now;
                _sISystemDbContext.tbCustomerOrderProcessor.Update(process);
            }
            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }
        public async Task<List<ForDeliveryMainResponse>> GetAllForDeliveryReponse(int statusId)
        {
            List<tbCustomerOrderMain> orderMain = new List<tbCustomerOrderMain>();

            if (statusId == 0)
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => j.IsCancelled == true).OrderBy(y => y.DateCreated).ToListAsync();
            }
            else if (statusId == 9)
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => (j.OrderStatusId == 4 || j.OrderStatusId == 5) && j.IsCancelled == false).OrderBy(y => y.DateCreated).ToListAsync();
            }
            else
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => (j.OrderStatusId == statusId) && j.IsCancelled == false).OrderBy(y => y.DateCreated).ToListAsync();
            }

            var processor = orderMain
                .Join(_sISystemDbContext.tbCustomerOrderProcessor.Where(x => x.ProcessDate != null),
                id => id.Id,
                id2 => id2.CustomerOrderId,
                (id, id2) => new ForDeliveryMainResponse
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
                    LeadTime = _dataTransform.CalculateLeadTime((DateTime)id2.ProcessDate, (DateTime)id2.OrderDate),

                    ProcessorId = id2.Id,
                    CustomerOrderId = id2.CustomerOrderId,
                    ProcessDate = id2.ProcessDate,
                    ProcessedById = id2.ProcessedById,
                    ProcessedByName = id2.ProcessedByName,
                    MarkDeliverDate = id2.MarkDeliverDate,
                    MarkDeliveredById = id2.MarkDeliveredById,
                    MarkDeliveredByName = id2.MarkDeliveredByName,
                    OrderDate = id2.OrderDate,
                    RemarksDelivered = id2.RemarksDelivered,
                    TotalProcessAmount = _sISystemDbContext.tbCustomerOrderDetails.Where(j => j.CustomerOrderId == id2.CustomerOrderId).Sum(i => i.SalesAmount)
                }).OrderByDescending(y => y.OrderDate).ToList();

            return processor;
        }
        #endregion

        #region Delivered
        public async Task<List<DeliveredMainResponse>> GetAllDeliveredReponse(int statusId)
        {
            List<tbCustomerOrderMain> orderMain = new List<tbCustomerOrderMain>();

            if (statusId == 0)
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => j.IsCancelled == true).OrderBy(y => y.DateCreated).ToListAsync();
            }
            else if (statusId == 10)
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => (j.OrderStatusId == 5 || j.OrderStatusId == 6) && j.IsCancelled == false).OrderBy(y => y.DateCreated).ToListAsync();
            }
            else
            {
                orderMain = await _sISystemDbContext.tbCustomerOrderMain.Select(x => x).Where(j => (j.OrderStatusId == statusId) && j.IsCancelled == false).OrderBy(y => y.DateCreated).ToListAsync();
            }

            var perct = orderMain
                .Join(_sISystemDbContext.tbCustomerSalesAgent,
                x => new { order = x.CustomerId, sales = x.SalesAgentId },
                y => new { order = y.CustomerId, sales = y.SalesAgentId },
                 (order, sales) => new
                 {
                     order,
                     sales
                 }).ToList();

            var processor = perct
                .Join(_sISystemDbContext.tbCustomerOrderProcessor,
                id => id.order.Id,
                id2 => id2.CustomerOrderId,
                (id, id2) => new DeliveredMainResponse
                {
                    Id = id.order.Id,
                    CustomerId = id.order.CustomerId,
                    SalesAgentId = id.order.SalesAgentId,
                    SubSalesAgentId = id.order.SubSalesAgentId,
                    SalesAgentName = id.order.SalesAgentName,
                    OrderStatusId = id.order.OrderStatusId,
                    SubSalesAgentName = id.order.SubSalesAgentName,
                    CustomerName = id.order.CustomerName,
                    TinNo = id.order.TinNo,
                    OrderAmount = id.order.OrderAmount,
                    ForMonthOf = id.order.ForMonthOf.ToString("Y"),
                    DateCreated = id.order.DateCreated,
                    IsCancelled = id.order.IsCancelled,
                    LeadTime = _dataTransform.CalculateLeadTime((DateTime)id2.MarkDeliverDate, (DateTime)id2.OrderDate),//Convert.ToInt32(id.order.LeadTime).ToString(),
                    IsPaid = id.order.IsPaid,
                    Rate = id.sales.OverrideRate.ToString() + "%",
                    MarkDeliveredDate = id2.MarkDeliverDate,
                    ProcessorId = id2.Id,
                    CustomerOrderId = id2.CustomerOrderId,
                    OrderDate = id2.OrderDate,
                    ChargeAmount = Math.Round(_sISystemDbContext.tbCustomerOrderDetails.Where(j => j.CustomerOrderId == id2.CustomerOrderId).Sum(i => i.SalesAmount) * (id.sales.OverrideRate / 100).Value,2),
                    TotalProcessAmount = _sISystemDbContext.tbCustomerOrderDetails.Where(j => j.CustomerOrderId == id2.CustomerOrderId).Sum(i => i.SalesAmount)
                }).OrderByDescending(y => y.OrderDate).ToList();

            return processor;
        }
        #endregion

        #region Payments

        public async Task<tbPaymentParentCustomer> GetParentCustomer(Guid parentCustomerid, Guid parentPaymentId)
        {
            return await _sISystemDbContext.tbPaymentParentCustomer.Where(x => x.Id == parentCustomerid && x.PaymentParentId == parentPaymentId).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateParentCustomer(tbPaymentParentCustomer parentCustomer)
        {
            _sISystemDbContext.tbPaymentParentCustomer.Update(parentCustomer);
            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }
        public async Task<int> UpdatePaymentStatus(Guid id, int status, string prosName)
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

                data.OrderStatusId = status;
                _sISystemDbContext.tbCustomerOrderMain.Update(data);
            }
            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }

        public async Task<int> VoidPaymentStatus(List<tbPaymentParentCustomer> customer, int status, string prosName)
        {
            foreach (var item in customer)
            {
                var data = await _sISystemDbContext.tbCustomerOrderMain.Where(x => x.Id == item.CustomerOrderId).FirstOrDefaultAsync();

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

                    data.OrderStatusId = status;
                    _sISystemDbContext.tbCustomerOrderMain.Update(data);
                }
                await _sISystemDbContext.SaveChangesAsync();
             }
            return 0;
        }
        #endregion

        #region Delete
        public async Task<int> DeletePaymentStatus(Guid paymentId)
        {
            var parentCustomer = await _sISystemDbContext.tbPaymentParentCustomer.Where(x => x.PaymentParentId == paymentId).ToListAsync();
            _sISystemDbContext.tbPaymentParentCustomer.RemoveRange(parentCustomer);

            var paymentForm = await _sISystemDbContext.tbPaymentForm.Where(x => x.PaymentParentId == paymentId).ToListAsync();
            _sISystemDbContext.tbPaymentForm.RemoveRange(paymentForm);

            var paymentParent = await _sISystemDbContext.tbPaymentParentDetails.Where(x => x.Id == paymentId).FirstOrDefaultAsync();
            _sISystemDbContext.tbPaymentParentDetails.Remove(paymentParent);

            await _sISystemDbContext.SaveChangesAsync();
            return 0;
        }

        public async Task<bool> DeleteCustomerOrderDetailsList(Guid id)
        {
            _sISystemDbContext.tbCustomerOrderDetails.RemoveRange(_sISystemDbContext.tbCustomerOrderDetails.Where(x => x.CustomerOrderId == id));
            await _sISystemDbContext.SaveChangesAsync();
            return true;
        }
        #endregion
    }
}
