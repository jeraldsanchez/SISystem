using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SISystem.Models;
using SISystem.Services;

namespace SISystem.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/SISystem/[controller]")]
    public class ReportController : Controller
    {
        private readonly IReportRepository _iReportRepository;
        private readonly ICustomerInformationServices _customerServices;
        private readonly IRegistrationRepository _registrationRepository;
        public ReportController(IReportRepository iReportRepository,
            ICustomerInformationServices customerServices,
            IRegistrationRepository registrationRepository)
        {
            _iReportRepository = iReportRepository;
            _customerServices = customerServices;
            _registrationRepository = registrationRepository;
        }

        [HttpGet("{customerOrderId}")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomerOrderDetailsListReport(Guid customerOrderId)
        {
            try
            {
                var userName = User.Identity.Name;
                RegistrationRegister user = await _registrationRepository.GetUsername(userName);
                if (user != null)
                {
                    var test = await _iReportRepository.GetReportSOA(customerOrderId);
                    await _customerServices.Logs(userName, user.UserId, "success", "", "GetCustomerOrderDetailsListReport.Get");
                    return Accepted(test);
                }
                return BadRequest(new { Message = "Account didn't exist" });
            }
            catch (Exception ex)
            {
                var userName = User.Identity.Name;
                await _customerServices.Logs(userName, null, "error", ex.GetBaseException().Message, "GetCustomerOrderDetailsListReport.Get");
                return BadRequest(new { Message = ex.ToString() });
            }
        }
    }
}