using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsertAlertsServicesAPI.Controllers.Common
{
    [Route("api/v2/[controller]")]
    public abstract class V2Controller(IMediator mediator) : BaseController(mediator)
    {
    }
}
