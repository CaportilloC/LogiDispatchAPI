using Application.DTOs.Orders;
using Application.Features.Orders.Commands.Create;
using Application.Features.Orders.Commands.Delete;
using Application.Features.Orders.Commands.Restore;
using Application.Features.Orders.Commands.Update;
using Application.Features.Orders.Queries.GetAll;
using Application.Features.Orders.Queries.GetById;
using Application.Features.Orders.Queries.GetDeleted;
using Application.Wrappers;
using LogiDispatchAPI.Controllers.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Tests.Controllers.V1
{
    [TestFixture]
    public class OrdersControllerTests
    {
        private Mock<IMediator> _mediatorMock = null!;
        private OrdersController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new OrdersController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetAll_ShouldReturnOkResultWithData()
        {
            // Arrange
            var expectedResponse = new WrapperResponse<List<OrderResponse>>(new List<OrderResponse>
            {
                new() { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Status = "CREATED" },
                new() { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Status = "IN_PROGRESS" }
            });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllOrdersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void GetAll_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.GetAll());
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllOrdersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetDeleted_ShouldReturnOkResultWithData()
        {
            // Arrange
            var expectedResponse = new WrapperResponse<List<OrderResponse>>(new List<OrderResponse>
            {
                new() { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Status = "DELETED" }
            });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetDeletedOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetDeleted();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetDeletedOrdersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void GetDeleted_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetDeletedOrdersQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.GetDeleted());
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetDeletedOrdersQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetById_ShouldReturnOkResultWithData()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedResponse = new WrapperResponse<OrderResponse>(
                new OrderResponse { Id = orderId, CustomerId = Guid.NewGuid(), Status = "CREATED" });

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetOrderByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GetById(orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(It.Is<GetOrderByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void GetById_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetOrderByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.GetById(orderId));
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(It.Is<GetOrderByIdQuery>(q => q.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Register_ShouldReturnOkResult_WhenCommandIsValid()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                OriginLatitude = 1.1,
                OriginLongitude = 2.2,
                DestinationLatitude = 3.3,
                DestinationLongitude = 4.4,
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            var expectedResponse = new WrapperResponse<OrderResponse>(
                new OrderResponse { Id = Guid.NewGuid(), CustomerId = command.CustomerId, Status = "CREATED" });

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Register(command);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Register_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                OriginLatitude = 1.1,
                OriginLongitude = 2.2,
                DestinationLatitude = 3.3,
                DestinationLongitude = 4.4,
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.Register(command));
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Update_ShouldReturnOkResult_WhenCommandIsValid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var command = new UpdateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                OriginLatitude = 1.1,
                OriginLongitude = 2.2,
                DestinationLatitude = 3.3,
                DestinationLongitude = 4.4,
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 1 }
                }
            };

            var expectedResponse = new WrapperResponse<OrderResponse>(
                new OrderResponse { Id = orderId, CustomerId = command.CustomerId, Status = "UPDATED" });

            _mediatorMock
                .Setup(m => m.Send(It.Is<UpdateOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Update(orderId, command);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(It.Is<UpdateOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Update_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var command = new UpdateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                OriginLatitude = 1.1,
                OriginLongitude = 2.2,
                DestinationLatitude = 3.3,
                DestinationLongitude = 4.4,
                Items = new List<OrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 1 }
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<UpdateOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.Update(orderId, command));
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(It.Is<UpdateOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Delete_ShouldReturnOkResult_WhenSuccess()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedResponse = new WrapperResponse<bool>(true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Delete(orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(It.Is<DeleteOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Delete_ShouldReturnBadRequest_WhenFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var failedResponse = new WrapperResponse<bool>("Error al eliminar la orden") { Succeeded = false };

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResponse);

            // Act
            var result = await _controller.Delete(orderId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo(failedResponse));

            _mediatorMock.Verify(m => m.Send(It.Is<DeleteOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Delete_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.Delete(orderId));
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(It.Is<DeleteOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Restore_ShouldReturnOkResult_WhenSuccess()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var expectedResponse = new WrapperResponse<bool>(true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RestoreOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Restore(orderId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));

            _mediatorMock.Verify(m => m.Send(It.Is<RestoreOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Restore_ShouldReturnBadRequest_WhenFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var failedResponse = new WrapperResponse<bool>("Error al restaurar la orden") { Succeeded = false };

            _mediatorMock
                .Setup(m => m.Send(It.Is<RestoreOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(failedResponse);

            // Act
            var result = await _controller.Restore(orderId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo(failedResponse));

            _mediatorMock.Verify(m => m.Send(It.Is<RestoreOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Restore_ShouldThrow_WhenMediatorFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<RestoreOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Mediator failure"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _controller.Restore(orderId));
            Assert.That(ex!.Message, Is.EqualTo("Mediator failure"));

            _mediatorMock.Verify(m => m.Send(It.Is<RestoreOrderCommand>(c => c.Id == orderId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}