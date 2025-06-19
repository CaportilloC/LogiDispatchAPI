using Application.Exceptions;
using Application.Extensions.Values;
using Application.Wrappers;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace LogiDispatchAPI.Middlewares
{
    public class ErrorHandlerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                await HandleError(context, error);
            }
        }

        private static async Task HandleError(HttpContext context, Exception error)
        {
            var response = context.Response;
            response.ContentType = MediaTypeNames.Application.Json;
            var responseModel = new WrapperResponse<string>() { Succeeded = false, Message = error.Message };

            switch (error)
            {
                case NullFileException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Errors = e.Message.ValueToOneItemList();
                    break;

                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ApiException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case ValidationException e:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    responseModel.Errors = e.Errors;
                    break;

                case KeyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ExternalServiceException e:
                    response.StatusCode = (int)HttpStatusCode.BadGateway;
                    responseModel.Errors = e.Message.ValueToOneItemList();
                    break;
                case ConflictException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;

            }
            var result = JsonSerializer.Serialize(responseModel);

            await response.WriteAsync(result);
        }
    }
}
