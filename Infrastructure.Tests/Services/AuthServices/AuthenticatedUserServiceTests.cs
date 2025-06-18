using Application.Contracts.Services.AuthServices;
using Application.Exceptions;
using Infrastructure.Services.AuthServices;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Infrastructure.Tests.Services.AuthServices
{
    [TestFixture]
    public class AuthenticatedUserServiceTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private AuthenticatedUserService _authenticatedUserService;

        [SetUp]
        public void SetUp()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _authenticatedUserService = new AuthenticatedUserService(_httpContextAccessorMock.Object);
        }

        [Test]
        public void AuthenticatedUserService_ImplementsIAuthenticatedUserService()
        {
            // Assert
            Assert.That(_authenticatedUserService, Is.InstanceOf<IAuthenticatedUserService>());
        }

        [Test]
        public void GetUsernameFromClaims_WhenCalled_ReturnsUserName()
        {
            // Arrange
            var claims = new List<Claim> { new("userName", "TestUser") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(principal);

            // Act
            var result = _authenticatedUserService.GetUsernameFromClaims();

            // Assert
            Assert.That(result, Is.EqualTo("TestUser"));
        }
    }
}
