using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISystem.Data;
using SISystem.Models;
using SISystem.Services;

namespace SISystem.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/SISystem/[controller]")]
    public class PaymentController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;
        public PaymentController(ITransactionRepository transactionRepository,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _transactionRepository = transactionRepository;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        [HttpPut("Debt")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> MarkAsBadDebt(List<Guid> salesAgentID)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _transactionRepository.UpdateOrderMarkedAsDebt(salesAgentID);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "MarkAsBadDebt.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "MarkAsBadDebt.Update");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Partial")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PayPartialPayment([FromBody] PaymentPartialRequest request)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    var parentCust = await _transactionRepository.GetParentCustomer(request.ParentCustomerId, request.PaymentParentId);
                    if (parentCust != null)
                    {
                        request.PaymentForm.ForEach(x => {
                            x.PaymentParentId = request.PaymentParentId;
                        });
                        await _transactionRepository.AddPaymentForm(request.PaymentForm);
                        decimal sum = request.PaymentForm.Sum(x => x.Amount);
                        if ((parentCust.ReceivableAmount - parentCust.PartialAmount) <= sum) {
                            await _transactionRepository.UpdatePaymentStatus(request.CustomerOrderId, 8, userName);
                            parentCust.PartialAmount = 0.0m;
                            parentCust.Remarks = "Fully Paid";
                            await _customerServices.Logs(userName, user.UserId, "success", "", "PayPartialPayment.Put");
                        }
                        else
                        {
                            parentCust.PartialAmount = parentCust.PartialAmount + sum;
                            await _customerServices.Logs(userName, user.UserId, "success", "", "PayPartialPayment.Put");
                        }
                        await _transactionRepository.UpdateParentCustomer(parentCust);
                        return Accepted("Partial Update Payment Accepted");
                    }
                    return NotFound("No Partial Payment Found");
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "PayPartialPayment.Update");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpDelete("Void/{orderId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> VoidPayment(Guid orderId)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    var customer = await _transactionRepository.GetPaymentCustomerDetails(orderId);
                    await _transactionRepository.VoidPaymentStatus(customer, 5, userName);
                    await _transactionRepository.DeletePaymentStatus(orderId);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "VoidPayment.Delete");
                return Accepted();
            }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "VoidPayment.Delete");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SavePaymentDetails([FromBody] PaymentRequest request)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    request.PaymentParentDetails.UserId = user.UserId;
                    request.PaymentParentDetails.DateCreated = DateTime.Now;
                    var payment = await _transactionRepository.AddPaymentParentDetails(request.PaymentParentDetails);
                    request.PaymentForm.ForEach(x => {
                        x.PaymentParentId = payment.Id;
                    });
                    await _transactionRepository.AddPaymentForm(request.PaymentForm);

                    decimal formSum = request.PaymentForm.Sum(x => x.Amount);
                    foreach (var item in request.PaymentParentCustomer)
                    {
                        decimal subt = formSum - item.ReceivableAmount;
                        if (subt >= 0)
                        {
                            item.Remarks = "Fully Paid";
                            item.PaymentParentId = payment.Id;
                            await _transactionRepository.AddPaymentParentCustomer(item);
                            await _transactionRepository.UpdatePaymentStatus(item.CustomerOrderId, 8, userName);
                        }
                        else if (subt > -item.ReceivableAmount)
                        {
                            item.Remarks = "Partially Paid";
                            item.PaymentParentId = payment.Id;
                            item.PartialAmount = formSum;
                            await _transactionRepository.AddPaymentParentCustomer(item);
                            await _transactionRepository.UpdatePaymentStatus(item.CustomerOrderId, 7, userName);
                        }
                        formSum = subt;
                    }
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SavePaymentDetails.Post");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SavePaymentDetails.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllPaymentReceived()
        {
            try
            {
                return Accepted(await _transactionRepository.GetPaymentReceivedNew());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllPaymentReceived.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
       
        [HttpGet("Summary")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllPaymentReceivedSummary()
        {
            try
            {
                return Accepted(await _transactionRepository.GetPaymentReceivedSummary());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllPaymentReceivedSummary.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
    }
}