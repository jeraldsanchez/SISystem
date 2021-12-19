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
    public class SalesAgentController : Controller
    {
        private readonly ISalesAgentRepository _salesAgentRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;
        public SalesAgentController(
            ISalesAgentRepository salesAgentRepository,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _salesAgentRepository = salesAgentRepository;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        #region Agent
        [HttpGet("Agent/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllAgentsById(Guid id)
        {
            try
            {
                return Accepted(await _salesAgentRepository.GetSalesAgentById(id));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllAgentsById.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }


        [HttpGet("Agent/Status/{status}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllActiveAgents(string status)
        {
            try
            {
                if (status.ToLower() == "all")
                {
                    return Accepted(await _salesAgentRepository.GetAllSalesAgent());
                }
                return Accepted(await _salesAgentRepository.GetAllActiveSalesAgent(status));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllActiveAgents.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }


        [HttpGet("Agent/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllAgents()
        {
            try
            {
                return Accepted(await _salesAgentRepository.GetAllSalesAgent());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllAgents.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("SalesAgentCustomer/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSalesAgentCustomer(Guid id)
        {
            try
            {
                return Accepted(await _salesAgentRepository.GetCustomerSalesAgent(id));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllAgentsById.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("SalesAgentCustomer/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllSalesAgentCustomerBy()
        {
            try
            {
                return Accepted(await _salesAgentRepository.GetCustomerSalesAgents2());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllSalesAgentCustomerBy.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("Agent/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveAgent([FromBody] tbSalesAgent agent)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    var sales = await _salesAgentRepository.GetSalesAgentChecker(agent.MobileNo1);
                    if (sales != null)
                    {
                        return BadRequest(new { Message = "Duplicate Mobile Number" });
                    }
                    await _salesAgentRepository.SaveSalesAgent(agent);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SaveAgent.Post");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveAgent.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Agent/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAgent([FromBody] tbSalesAgent agent)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _salesAgentRepository.UpdateSalesAgent(agent);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "UpdateAgent.Update");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UpdateAgent.Update");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpDelete("Agent/{guid}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAgent(Guid guid)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _salesAgentRepository.DeleteSalesAgent(guid);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "DeleteAgent.Delete");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
        }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "DeleteAgent.Delete");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
        #endregion
    }
}