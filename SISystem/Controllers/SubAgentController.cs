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
    public class SubAgentController : Controller
    {
        private readonly ISalesAgentRepository _salesAgentRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;
        public SubAgentController(ISalesAgentRepository salesAgentRepository,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _salesAgentRepository = salesAgentRepository;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        #region SubAgent

        [HttpGet("SubAgent/AgentId/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubAgentByAgentId(Guid id)
        {
            try
            {
                return Accepted(await _salesAgentRepository.GetSubAgentsByAgentId(id));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetSubAgentByAgentId.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("SubAgent/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubAgent(Guid id)
        {
            try
            {
                return Accepted(await _salesAgentRepository.GetSubAgents(id));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetSubAgent.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("SubAgent/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubAgentAll()
        {
            try
            {
                return Accepted(await _salesAgentRepository.GetAllSubAgents());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetSubAgentAll.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("SubAgent/Status/{status}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubAgentAllActive(string status)
        {
            try
            {
                if (status.ToLower() == "all")
                {
                    return Accepted(await _salesAgentRepository.GetAllSubAgents());
                }
                return Accepted(await _salesAgentRepository.GetAllSubAgentsActive(status));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetSubAgentAllActive.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("SubAgent/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveSubSalesAgent([FromBody] tbSalesSubAgent agent)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _salesAgentRepository.SaveSubSalesAgent(agent);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "SaveSubSalesAgent.Post");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "SaveSubSalesAgent.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("SubAgent/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateSubSalesAgent([FromBody] tbSalesSubAgent agent)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _salesAgentRepository.UpdateSubSalesAgent(agent);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "UpdateSubSalesAgent.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UpdateSubSalesAgent.Update");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpDelete("SubAgent/{agentId}/{subAgentId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteSubSalesAgent(Guid agentId, Guid subAgentId)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _salesAgentRepository.DeleteSubSalesAgent(agentId, subAgentId);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "DeleteSubSalesAgent.Delete");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "DeleteSubSalesAgent.Delete");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
        #endregion

    }
}