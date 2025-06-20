using Application.Features.Orders.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LogiDispatchAPI.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        /// <returns>Lista de clientes</returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(result);
        }
    }
}
