using Application.Contracts.Persistence.Common.BaseRepository;
using Application.Contracts.Services.AuthServices;
using Application.Exceptions;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Common.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Infrastructure.Tests.Repositories.Common.UnitOfWork
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        private ApplicationDbContext _context;
        private Infrastructure.Repositories.Common.UnitOfWork.UnitOfWork _unitOfWork;
        private Mock<IAuthenticatedUserService> _mockAuthenticatedUserService;
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

            _mockAuthenticatedUserService = new Mock<IAuthenticatedUserService>();
            _mockAuthenticatedUserService.Setup(x => x.GetUsernameFromClaims()).Returns(Username);

            _context = new ApplicationDbContext(options, _mockAuthenticatedUserService.Object);

            _unitOfWork = new Infrastructure.Repositories.Common.UnitOfWork.UnitOfWork(_context);
        }



        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Dispose();
            _context.Dispose();
        }

        [Test]
        public void UnitOfWork_WhenCalled_ShouldReturnCorrectType()
        {
            //Assert
            Assert.That(_unitOfWork, Is.InstanceOf<Infrastructure.Repositories.Common.UnitOfWork.UnitOfWork>());
        }

        // create test to check if Complete method throws exception
        [Test]
        public void Complete_WhenCalledWithException_ThrowsException()
        {
            // Arrange
            _context.Database.EnsureDeleted();
            _context.Dispose();

            // Act & Assert
            Assert.ThrowsAsync<ApiException>(() => _unitOfWork.Complete());
        }

        [Test]
        public void ProcedureRepository_WhenCalled_ReturnsIProcedureRepositoryImplementation()
        {
            // Act
            var result = _unitOfWork.ProcedureRepository;

            // Assert
            Assert.That(result, Is.InstanceOf<IProcedureRepository>());
        }
    }
}
