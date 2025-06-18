using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LogiDispatchAPI.Controllers.Common;

namespace InsertAlertsServicesAPI.Controllers.v1
{
    public class InsertAlertsServices(IMediator mediator) : V1Controller(mediator)
    {
        [HttpGet("TestApiKey")]
        //[ServiceFilter(typeof(ApiKeyAttribute))]
        public async Task<IActionResult> GetAlerts()
        {
            return Ok();
        }

        [HttpGet("Test")]
        [AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}
