using Application.Features.Orders.Commands.Create;
using Application.Features.Orders.Commands.Delete;
using Application.Features.Orders.Commands.Restore;
using Application.Features.Orders.Commands.Update;
using Application.Features.Orders.Queries.GetAll;
using Application.Features.Orders.Queries.GetById;
using Application.Features.Orders.Queries.GetDeleted;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LogiDispatchAPI.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene todas las órdenes.
        /// </summary>
        /// <returns>Lista de órdenes</returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllOrdersQuery());
            return Ok(result);
        }

        /// <summary>
        /// Restaura una orden eliminada.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Orden restaurada</returns>
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var result = await _mediator.Send(new GetDeletedOrdersQuery());
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una orden por su ID.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Orden encontrada</returns>
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Registra una nueva orden de despacho.
        /// </summary>
        /// <param name="command">Datos de la orden</param>
        /// <returns>Resultado del registro</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Actualiza una orden existente.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <param name="command">Nuevos datos</param>
        /// <returns>Orden actualizada</returns>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Elimina (soft delete) una orden.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteOrderCommand { Id = id });
            return Ok(result);
        }

        /// <summary>
        /// Restaura una orden eliminada.
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Orden restaurada</returns>
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _mediator.Send(new RestoreOrderCommand { Id = id });
            return Ok(result);
        }

    }
}
