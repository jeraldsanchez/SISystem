using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISystem.Data;
using SISystem.Models;
using SISystem.Services;

namespace SISystem.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/SISystem/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDataTransform _dataTransform;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;

        public TransactionsController(
            ITransactionRepository transactionRepository
            ,IDataTransform dataTransform,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _transactionRepository = transactionRepository;
            _dataTransform = dataTransform;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        #region Get

        [HttpGet("CustomerOrder/View/Cancelled")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomerOrderPageCancelled()
        {
            try
            {
                return Accepted(await _transactionRepository.GetAllCustomerOrderReponseCancelled());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetCustomerOrderPageCancelled.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("CustomerOrder/View/{statusId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomerOrderPage(int statusId)
        {
            try
            {
                return Accepted(await _transactionRepository.GetAllCustomerOrderReponse(statusId));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetCustomerOrderPage.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("CustomerOrder/SI/{issuingCompId}/{sINo}")]
        //[Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckSI(Guid issuingCompId, string sINo)
        {
            try
            {
                bool check = await _transactionRepository.GetSIChecker(issuingCompId, sINo);
                if (check == true){
                    return Accepted();
                }
                return BadRequest(new { Message = "Existing SI"});
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "CheckSI.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("CustomerOrder/OR/{issuingCompId}/{oRNo}")]
        //[Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckOR(Guid issuingCompId, string oRNo)
        {
            try
            {
                bool check = await _transactionRepository.GetORChecker(issuingCompId, oRNo);
                if (check == true)
                {
                    return Accepted();
                }
                return BadRequest(new { Message = "Existing OR" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "CheckOR.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("CustomerOrder/SOA/{customerOrderId}")]
        //[Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GenerateSOA(Guid customerOrderId)
        {
            try
            {
                return Accepted(await _transactionRepository.GenerateSOA(customerOrderId));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GenerateSOA.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
        #endregion

        [HttpPut("CustomerOrder/Action/{id}/{trans}")]//approve cancel
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveCustomerOrderPage(Guid id, string trans)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    if (trans.ToLower() == "approve")
                    {
                        await _transactionRepository.UpdateCustomerOrderApprove(id, userName, user.UserId);
                    }
                    else if (trans.ToLower() == "cancel")
                    {
                        await _transactionRepository.UpdateCustomerOrderCancel(id, userName, user.UserId);
                    }
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SaveCustomerOrderPage.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveCustomerOrderPage.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("CustomerOrder/Cancelled/Revert/Multiple")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CustomerOrderCancelRevertMultiple([FromBody] List<RevertRequestId> id)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    foreach (var item in id)
                    {
                        await _transactionRepository.RevertCancel(item.id);
                    }
                    await _customerServices.Logs(userName, user.UserId, "success", "", "CustomerOrderCancelRevertMultiple.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "CustomerOrderCancelRevertMultiple.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("CustomerOrder/Cancelled/Revert/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CustomerOrderCancelRevert(Guid id)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _transactionRepository.RevertCancel(id);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "CustomerOrderCancelRevert.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "CustomerOrderCancelRevert.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Order/Revert/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateRevertCustomerOrder(Guid id)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _transactionRepository.UpdateCustomerOrder(id, userName);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "UpdateRevertCustomerOrder.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UpdateRevertCustomerOrder.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Order/Revert/Multiple")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateRevertCustomerOrderMultiple([FromBody] List<RevertRequestId> id)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    foreach (var item in id)
                    {
                        await _transactionRepository.UpdateCustomerOrder(item.id, userName);
                    }
                    await _customerServices.Logs(userName, user.UserId, "success", "", "UpdateRevertCustomerOrderMultiple.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UpdateRevertCustomerOrderMultiple.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        #region Post
        [HttpPost("CustomerOrder/")]
        //[Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveCustomerOrderPage([FromBody] CustomerOrderMainRequest orderMain)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    orderMain.SubSalesAgentId = _dataTransform.NullSubAgent(orderMain.SubSalesAgentId);
                    orderMain.TransactionCode = _dataTransform.GetTransactionCode();
                    var order = _dataTransform.GetDataCustomerOrderPage(orderMain);
                    Guid id = await _transactionRepository.AddtbCustomerOrderMain(order);
                    //Get Encoder name and Id
                    tbCustomerOrderProcessor process = _dataTransform.GetDataCustomerOrderPageProcessor(orderMain, id, user.userName, user.UserId);
                    //process.OrderDate = order.DateCreated;
                    await _transactionRepository.AddtbCustomerOrderProcessor(process);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SaveCustomerOrderPage.Post");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveCustomerOrderPage.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("Order/CustomerList")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveCustomerOrder([FromBody] List<tbCustomerOrder> billing)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    billing.ForEach(x => x.SubSalesAgentID = _dataTransform.NullSubAgent(x.SubSalesAgentID));
                    billing.ForEach(x => x.TransactionCode = _dataTransform.GetTransactionCode());
                    await _transactionRepository.AddCustomerOrderList(billing);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SaveCustomerOrder.Post");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveCustomerOrder.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("Invoice/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveInvoiceDetails([FromBody] List<tbCustomerOrderDetails> invoice)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    tbCustomerOrderMain order = await _transactionRepository.GetCustomerOrderById(invoice[0].CustomerOrderId);
                    if (order != null)
                    {
                        invoice.ForEach(x => x.SalesAgentId = order.SalesAgentId);
                        await _transactionRepository.AddCustomerOrderDetailsList(invoice);

                        //Add Create SOA No
                        SOAResponse sOA = await _transactionRepository.GenerateSOA(invoice[0].CustomerOrderId);
                        await _transactionRepository.UpdateProcessInvoice(order.Id, sOA.GenSOA, userName, user.UserId);
                    }
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SaveInvoiceDetails.Post");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveInvoiceDetails.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Invoice/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateInvoiceDetails([FromBody] List<tbCustomerOrderDetails> invoice)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    //Delete exisiting
                    await _transactionRepository.DeleteCustomerOrderDetailsList(invoice[0].CustomerOrderId);
                    tbCustomerOrderMain order = await _transactionRepository.GetCustomerOrderById(invoice[0].CustomerOrderId);
                    if (order != null)
                    {
                        invoice.ForEach(x => x.SalesAgentId = order.SalesAgentId);
                        await _transactionRepository.AddCustomerOrderDetailsList(invoice);
                        await _transactionRepository.UpdateProcessInvoicePut(order.Id, userName, user.UserId);
                    }
                    await _customerServices.Logs(userName, user.UserId, "success", "", "UpdateInvoiceDetails.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveInvoiceDetails.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Invoice/Action/{id}/{trans}")]//transfer cancel
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ActionInvoiceDetails(Guid id, string trans)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    if (trans.ToLower() == "transfer")
                    {
                        await _transactionRepository.UpdateApproveInvoice(id, userName, user.UserId);
                    }
                    else if (trans.ToLower() == "cancel")
                    {
                        await _transactionRepository.UpdateCustomerOrderCancel(id, userName, user.UserId);
                    }
                    await _customerServices.Logs(userName, user.UserId, "success", "", "ActionInvoiceDetails.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "ActionInvoiceDetails.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Invoice/View/{statusId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetInvoicePage(int statusId)
        {
            try
            {
                return Accepted(await _transactionRepository.GetAllInvoiceReponse(statusId));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetInvoicePage.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Invoice/View/Order/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrderDetails(Guid id)
        {
            try
            {
                return Accepted(await _transactionRepository.GetOrderDetailsById(id));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetOrderDetails.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("ForDelivery/Action/{id}/{trans}")]//transfer cancel
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ActionForDeliveryDetails(Guid id, string trans)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    if (trans.ToLower() == "deliver")
                    {
                        await _transactionRepository.UpdateForDelivery(id, userName, user.UserId);
                    }
                    else if (trans.ToLower() == "cancel")
                    {
                        await _transactionRepository.UpdateCustomerOrderCancel(id, userName, user.UserId);
                    }
                    await _customerServices.Logs(userName, user.UserId, "success", "", "ActionForDeliveryDetails.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "ActionForDeliveryDetails.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("ForDelivery/View/{statusId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetForDeliveryPage(int statusId)
        {
            try
            {
                return Accepted(await _transactionRepository.GetAllForDeliveryReponse(statusId));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetForDeliveryPage.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Delivered/View/{statusId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDeliveredPage(int statusId)
        {
            try
            {
                return Accepted(await _transactionRepository.GetAllDeliveredReponse(statusId));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetDeliveredPage.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
        #endregion

        #region Delete
        [HttpDelete("Cancel/{orderId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    var customer = await _transactionRepository.DeleteCancelledCustomerOrder(orderId);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "CancelOrder.Delete");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "CancelOrder.Delete");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
        #endregion
    }
}