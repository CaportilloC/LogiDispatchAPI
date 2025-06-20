using Application.DTOs.Orders.Customers;
using Application.Features.Orders.Queries.GetAll;
using Application.Wrappers;
using LogiDispatchAPI.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Tests.Controllers.V1
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private Mock<IMediator> _mediatorMock = null!;
        private CustomersController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CustomersController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetAll_ShouldReturnOkResultWithData()
        {
            // Arrange
            var expectedResponse = new WrapperResponse<List<CustomerResponse>>(new List<CustomerResponse>
            {
                new() { Id = Guid.NewGuid(), Name = "Cliente 1" },
                new() { Id = Guid.NewGuid(), Name = "Cliente 2" }
            });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void GetAll_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.GetAll());
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}