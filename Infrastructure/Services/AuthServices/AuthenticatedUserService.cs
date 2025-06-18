using Application.Contracts.Services.AuthServices;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services.AuthServices
{
    public class AuthenticatedUserService(IHttpContextAccessor httpContextAccessor) : IAuthenticatedUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string GetUsernameFromClaims()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue("userName");
            //if (string.IsNullOrEmpty(userName?.Trim() ?? string.Empty)) throw new ApiException("Not authorized");

            return userName!;
        }
    }
}
