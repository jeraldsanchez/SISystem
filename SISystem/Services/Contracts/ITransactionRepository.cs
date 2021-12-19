using SISystem.Data;
using SISystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SISystem.Services
{
    public interface ITransactionRepository
    {
        #region Update
        Task<int> UpdateForDelivery(Guid id, string prosName, Guid guid);
        Task<int> RevertCancel(Guid id);
        Task<int> UpdateCustomerOrderCancel(Guid id, string prosName, Guid guid);
        Task<int> UpdateCustomerOrder(Guid id, string prosName);
        Task<int> UpdateOrderMarkedAsDebt(List<Guid> dest);
        Task<int> UpdateCustomerOrderApprove(Guid id, string prosName, Guid guid);
        Task<int> UpdateApproveInvoice(Guid id, string prosName, Guid guid);
        Task<int> UpdatePaymentStatus(Guid id, int status, string prosName);
        Task<int> UpdateProcessInvoicePut(Guid id, string prosName, Guid guid);
        Task<int> VoidPaymentStatus(List<tbPaymentParentCustomer> customer, int status, string prosName);
        Task<int> UpdateParentCustomer(tbPaymentParentCustomer parentCustomer);
        #endregion

        #region Get
        Task<List<PaymentsReceivedResponse>> GetPaymentReceivedNew();
        Task<tbPaymentParentCustomer> GetParentCustomer(Guid parentCustomerid, Guid parentPaymentId);
        Task<List<tbPaymentParentCustomer>> GetPaymentCustomerDetails(Guid id);
        Task<bool> GetSIChecker(Guid issuingCompId, string sINo);
        Task<bool> GetORChecker(Guid issuingCompId, string oRNo);
        Task<List<InvoiceOrderDetailsResponse>> GetOrderDetailsById(Guid customerOrderId);
        Task<List<DeliveredMainResponse>> GetAllDeliveredReponse(int statusId);
        Task<List<ForDeliveryMainResponse>> GetAllForDeliveryReponse(int statusId);
        Task<List<InvoiceMainResponse>> GetAllInvoiceReponse(int statusId);
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
        Task<List<tbPaymentForm>> AddPaymentForm(List<tbPaymentForm> dest);
        Task<tbPaymentParentCustomer> AddPaymentParentCustomer(tbPaymentParentCustomer dest);
        Task<tbPaymentParentDetails> AddPaymentParentDetails(tbPaymentParentDetails dest);
        Task<List<PaymentsReceivedResponse>> GetPaymentReceived();
        Task<IEnumerable<PaymentSummaryResponse>> GetPaymentReceivedSummary();
        #endregion

        #region Add
        Task<SOAResponse> GenerateSOA(Guid customerOrderId);
        Task<Guid> AddtbCustomerOrderProcessor(tbCustomerOrderProcessor dest);
        Task<Guid> AddtbCustomerOrderMain(tbCustomerOrderMain dest);
        Task<int> UpdateProcessInvoice(Guid id, string sOANo, string prosName, Guid guid);
        Task<List<tbCustomerOrder>> AddCustomerOrderList(List<tbCustomerOrder> dest);
        Task<List<tbCustomerOrderDetails>> AddCustomerOrderDetailsList(List<tbCustomerOrderDetails> dest);
        Task<Guid> AddBillingDetails(tbBillingDetails dest);
        Task<Guid> AddBilling(tbBilling dest);
        Task<List<tbPaymentDetails>> AddPaymentDetails(List<tbPaymentDetails> dest);
        Task<Guid> AddCustomerOrder(tbCustomerOrder dest);
        Task<Guid> AddCustomerOrderDetails(tbCustomerOrderDetails dest);
        Task<Guid> AddCustomerOrderVersion(tbCustomerOrderVersion dest);
        #endregion

        #region Delete
        Task<bool> DeleteCustomerOrderDetailsList(Guid id);
        Task<int> DeletePaymentStatus(Guid paymentId);
        Task<int> DeleteCancelledCustomerOrder(Guid orderId);
        #endregion
    }
}
