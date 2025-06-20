using Application.DTOs.Orders.Products;
using Application.Features.Orders.Queries.GetAll;
using Application.Wrappers;
using LogiDispatchAPI.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Tests.Controllers.V1
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IMediator> _mediatorMock = null!;
        private ProductsController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductsController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetAll_ShouldReturnOkResultWithData()
        {
            // Arrange
            var expectedResponse = new WrapperResponse<List<ProductResponse>>(new List<ProductResponse>
            {
                new() { Id = Guid.NewGuid(), Name = "Producto A" },
                new() { Id = Guid.NewGuid(), Name = "Producto B" }
            });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void GetAll_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.GetAll());
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
