using System;
using System.Collections.Generic;
using System.Linq;
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
    public class IssuingCompanyController : Controller
    {
        private readonly IIssuingCompanyRepository _issuingCompanyRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;
        public IssuingCompanyController(
            IIssuingCompanyRepository issuingCompanyRepository,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _issuingCompanyRepository = issuingCompanyRepository;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        [HttpGet("Company/{id}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllIssuingCompanyById(Guid id)
        {
            try
            {
                return Accepted(await _issuingCompanyRepository.GetIssuingCompanyById(id));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllIssuingCompanyById.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Company/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllIssuingCompany()
        {
            try
            {
                return Accepted(await _issuingCompanyRepository.GetIssuingCompany());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllIssuingCompany.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Company/Status/{status}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllIssuingCompanyByStatus(string status)
        {
            try
            {
                if (status.ToLower() == "all")
                {
                    return Accepted(await _issuingCompanyRepository.GetIssuingCompany());
                }
                return Accepted(await _issuingCompanyRepository.GetIssuingCompanyByStatus(status));
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllIssuingCompanyByStatus.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPost("Company/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostIssuingCompany([FromBody] IssuingCompanyRequest req)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    var issuing = await _issuingCompanyRepository.GetIssuingCompanyChecker(req.TIN);
                    if (issuing != null)
                    {
                        return BadRequest(new { Message = "Duplicate TIN Number" });
                    }
                    tbIssuingCompany company = new tbIssuingCompany
                    {
                        CompanyTypeID = req.CompanyTypeID,
                        Address = req.Address,
                        CompanyName = req.CompanyName,
                        TIN = req.TIN,
                        RDO = req.RDO,
                        Status = "Active",
                        BIRRegDate = DateTime.Today,
                        RetireDate = DateTime.Today.AddYears(1),
                        ZipCode = req.ZipCode
                    };
                    await _issuingCompanyRepository.SaveIssuingCompany(company);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "PostIssuingCompany.Post");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "PostIssuingCompany.Post");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpGet("Company/type")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCompanyType()
        {
            try
            {
                return Accepted(await _issuingCompanyRepository.GetCompanyType());
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetAllCompanyType.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpPut("Company/")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateIssuingCompany([FromBody] tbIssuingCompany company)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _issuingCompanyRepository.UpdateIssuingCompany(company);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "UpdateIssuingCompany.Put");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "UpdateIssuingCompany.Put");
                return BadRequest(new { Message = ex.ToString() });
            }
        }

        [HttpDelete("Company/{guid}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteIssuingCompany(Guid guid)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    await _issuingCompanyRepository.DeleteIssuingCompany(guid);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "DeleteIssuingCompany.Delete");
                    return Accepted();
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "DeleteIssuingCompany.Delete");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
    }
}