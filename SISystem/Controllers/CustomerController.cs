using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISystem.Data;
using SISystem.Models;
using SISystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace SISystem.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/SISystem/[controller]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISalesAgentRepository _salesAgentRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;

        public CustomerController(
            ICustomerRepository customerRepository,
            ISalesAgentRepository salesAgentRepository,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _customerRepository = customerRepository;
            _salesAgentRepository = salesAgentRepository;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        [HttpGet("Status/{status}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCustomerByStatus(string status)
        {
            try
            {
                if (status.ToLower() == "all")
                {
                    return Accepted(await _customerRepository.GetAllCustomerMultiple());
                }
                return Accepted(await _customerRepository.GetAllCustomerMultipleByStatus(status));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllCustomerByStatus.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCustomer()
        {
            try
            {
                return Accepted(await _customerRepository.GetAllCustomerMultiple());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllCustomer.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCustomer(Guid id)
        {
            try
            {
                return Accepted(await _customerRepository.GetAllCustomerById(id));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllCustomer.Id.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("Multiple")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveMultipleCustomer([FromBody] CustomerAgentRequest customer)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    Guid id = await _customerRepository.AddCustomer(customer.customer);
                    customer.customerSalesAgent.ForEach(x => {
                        x.CustomerId = id;
                        x.DateCreated = DateTime.Now;
                        x.IsActive = true;
                    });
                    await _salesAgentRepository.UpdateOverrideRemarks(customer.customerSalesAgent);
                    await _customerRepository.AddCustomerSalesAgentMultiple(customer.customerSalesAgent);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SaveMultipleCustomer.Post");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveMultipleCustomer.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpDelete("{customerId}/SalesAgent/{salesAgentId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCustomer(Guid customerId, Guid salesAgentId)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _customerRepository.DeleteSalesAgent(customerId, salesAgentId);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "DeleteCustomer.Delete");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "DeleteCustomer.Delete");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCustomerById(Guid id)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _customerRepository.DeleteCustomer(id);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "DeleteCustomerById.Delete");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "DeleteCustomerById.Delete");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateCustomer([FromBody] tbCustomer customer)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _customerRepository.UpdateCustomer(customer);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "UpdateCustomer.Update");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UpdateCustomer.Update");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Multiple")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateMultipleCustomer([FromBody] CustomerAgentRequest customer)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _customerRepository.UpdateCustomer(customer.customer);
                    await _customerRepository.UpdateSalesCustomer(customer.customerSalesAgent);
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UpdateMultipleCustomer.Update");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
    }
}