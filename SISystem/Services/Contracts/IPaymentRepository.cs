using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface IPaymentRepository
    {
        #region Update
        Task<int> AccountSummaryRevertCancel(Guid id);
        Task<int> UpdateAccountSummaryCancel(Guid id, string remarks, string prosName, Guid guid);
        Task<int> UpdateCustomerOrder(Guid id, string prosName);
        Task<int> UpdateOrderMarkedAsDebt(List<Guid> dest);
        Task<int> UpdateCustomerOrderApprove(Guid id, string prosName, Guid guid);
        #endregion

        #region Get
        Task<List<CustomerOrderMainResponse>> GetAllCustomerOrderReponseCancelled();
        Task<List<CustomerOrderMainResponse>> GetAllCustomerOrderReponse(int statusID);
        Task<List<BillingStatementResponse>> GetBillingStatement(Guid guid);
        Task<object> GetAllCustomerOrder();
        Task<List<tbCustomerOrderDetails>> GetCustomerOrderDetailsList(Guid id);
        Task<object> GetAllCustomerOrderByStatus(int id);
        Task<object> GetAllBilling();
        Task<object> GetCustomerOrderDetailsById(Guid id);
        Task<object> GetCustomerOrderWithOrderDetails(Guid id);
        Task<object> GetAllPaymentDetails();
        Task<tbCustomerOrderVersion> GetCustomerVersion(Guid id);
        Task<tbCustomerOrderMain> GetCustomerOrderById(Guid id);
        #endregion

        #region Add
        Task<Guid> AddtbCustomerOrderProcessor(tbCustomerOrderProcessor dest);
        Task<Guid> AddtbCustomerOrderMain(tbCustomerOrderMain dest);
        Task<List<tbCustomerOrder>> AddCustomerOrderList(List<tbCustomerOrder> dest);
        Task<List<tbCustomerOrderDetails>> AddCustomerOrderDetailsList(List<tbCustomerOrderDetails> dest);
        Task<Guid> AddBillingDetails(tbBillingDetails dest);
        Task<Guid> AddBilling(tbBilling dest);
        Task<List<tbPaymentDetails>> AddPaymentDetails(List<tbPaymentDetails> dest);
        Task<Guid> AddCustomerOrder(tbCustomerOrder dest);
        Task<Guid> AddCustomerOrderDetails(tbCustomerOrderDetails dest);
        Task<Guid> AddCustomerOrderVersion(tbCustomerOrderVersion dest);
        #endregion
    }
}
