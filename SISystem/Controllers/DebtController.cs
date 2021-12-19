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
    public class DebtController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;
        public DebtController(IPaymentRepository paymentRepository,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _paymentRepository = paymentRepository;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        [HttpPut("Action/{id}/{remarks}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AccountSummaryTransaction(Guid id, string remarks)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _paymentRepository.UpdateAccountSummaryCancel(id, remarks, userName, user.UserId);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "AccountSummaryTransaction.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllCustomerByStatus.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("AccountSummary/Cancelled/Revert/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AccountSummaryRevertCancel(Guid id)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _paymentRepository.AccountSummaryRevertCancel(id);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "AccountSummaryRevertCancel.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "AccountSummaryRevertCancel.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
    }
}