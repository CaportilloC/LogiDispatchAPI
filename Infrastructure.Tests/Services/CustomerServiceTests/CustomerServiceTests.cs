using Application.Contracts.Persistence.Common.BaseRepository;
using Application.Contracts.Persistence.Common.UnitOfWork;
using Application.DTOs.Orders.Customers;
using Ardalis.Specification;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Services.CustomerService;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.Services.CustomerServiceTests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<CustomerService>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private CustomerService _customerService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _mapperMock = new Mock<IMapper>();

            _customerService = new CustomerService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnMappedCustomers_WhenCustomersExist()
        {
            var fakeCustomers = new List<Customer>
            {
                new() { Id = Guid.NewGuid(), Name = "Cliente 1" },
                new() { Id = Guid.NewGuid(), Name = "Cliente 2" }
            };

            var mappedResponses = new List<CustomerResponse>
            {
                new() { Id = fakeCustomers[0].Id, Name = "Cliente 1" },
                new() { Id = fakeCustomers[1].Id, Name = "Cliente 2" }
            };

            var repoMock = new Mock<IBaseRepository<Customer>>();
            repoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Customer>>(), default))
                    .ReturnsAsync(fakeCustomers);

            _unitOfWorkMock.Setup(u => u.Repository<Customer>()).Returns(repoMock.Object);
            _mapperMock.Setup(m => m.Map<List<CustomerResponse>>(fakeCustomers)).Returns(mappedResponses);

            var result = await _customerService.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(mappedResponses));

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Se recuperaron")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoCustomersFound()
        {
            var repoMock = new Mock<IBaseRepository<Customer>>();
            repoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Customer>>(), default))
                    .ReturnsAsync(new List<Customer>());

            _unitOfWorkMock.Setup(u => u.Repository<Customer>()).Returns(repoMock.Object);

            var result = await _customerService.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No se encontraron clientes")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
