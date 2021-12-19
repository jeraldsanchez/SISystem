using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISystem.Data;
using SISystem.Services;

namespace SISystem.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/SISystem/[controller]")]
    public class BillingController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;
        public BillingController(ITransactionRepository transactionRepository,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _transactionRepository = transactionRepository;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        [HttpGet("{salesAgentID}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBillingBySalesAgentId(Guid salesAgentID)
        {
            try
            {
                var test = await _transactionRepository.GetBillingStatement(salesAgentID);
                return Accepted(test);
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllCustomerByStatus.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("Billing")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveBilling([FromBody] tbBilling billing)
        {
            try
            {
                return Accepted(await _transactionRepository.AddBilling(billing));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllCustomerByStatus.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("BillingDetails")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveBillingDetails([FromBody] tbBillingDetails billing)
        {
            try
            {
                return Accepted(await _transactionRepository.AddBillingDetails(billing)); //Update Order Status
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllCustomerByStatus.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
    }
}