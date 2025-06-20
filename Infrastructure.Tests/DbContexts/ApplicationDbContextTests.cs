using Application.Contracts.Services.AuthServices;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Infrastructure.Tests.DbContexts
{
    [TestFixture]
    public class ApplicationDbContextTests
    {
        //private ApplicationDbContext _context;
        private Mock<IAuthenticatedUserService> _authenticatedUserServiceMock;
        private string InMemoryDatabaseName;
        private string Username;

        [SetUp]
        public void SetUp()
        {
            Username = "TestUser";
            InMemoryDatabaseName = $"temp_proyect_${Guid.NewGuid().ToString().Replace("-", "")}";

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: InMemoryDatabaseName)
                .Options;

            _authenticatedUserServiceMock = new Mock<IAuthenticatedUserService>();
            _authenticatedUserServiceMock.Setup(m => m.GetUsernameFromClaims()).Returns(Username);

            //_context = new ApplicationDbContext(options, _authenticatedUserServiceMock.Object);
        }
    }
}

