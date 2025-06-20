using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LogiDispatchAPI.Controllers.Common
{
    [ApiController]
    public abstract class BaseController(IMediator mediator) : ControllerBase
    {
        protected IMediator Mediator = mediator;
    }
}
