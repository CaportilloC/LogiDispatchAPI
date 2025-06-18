using Application.Contracts.Services.AuthServices;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class InstanceRepositoryTests
    {
        private ApplicationDbContext _context;
        //private InstanceRepository _repository;
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

            //_repository = new InstanceRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetInstanceByNameAsync_WhenCalled_ReturnsInstanceEntity()
        {
            //// Arrange
            //var instanceEntity = new InstanceEntity { Name = "Test Name", Description = "Test Description" };
            //_context.InstanceEntity.Add(instanceEntity);
            //await _context.SaveChangesAsync();

            //// Act
            //var result = await _repository.GetInstanceByNameAsync("Test Name");

            // Assert
            //Assert.Multiple(() =>
            //{
            //    Assert.That(result, Is.Not.Null);
            //    Assert.That(result!.Name, Is.EqualTo("Test Name"));
            //});
        }

        [Test]
        public void PokemonService_ShouldBeDecoratedWithRegisterExternalServiceAttribute()
        {
            // Arrange
            //var attribute = typeof(InstanceRepository).GetCustomAttribute<RegisterServiceAttribute>();

            // Assert
            //Assert.That(attribute, Is.Not.Null);
        }
    }
}
