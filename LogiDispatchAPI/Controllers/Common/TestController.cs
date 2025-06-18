using MediatR;

namespace LogiDispatchAPI.Controllers.Common
{
    public class TestController(IMediator mediator) : V1Controller(mediator)
    {
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var response = await Mediator.Send(new GetAllQuery());
        //    return Ok(response);
        //}
    }
}
