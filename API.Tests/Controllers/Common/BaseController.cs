using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsertAlertsServicesAPI.Controllers.Common
{
    [ApiController]
    public abstract class BaseController(IMediator mediator) : ControllerBase
    {
        protected IMediator Mediator = mediator;
    }
}
