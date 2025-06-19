using Application.Contracts.Persistence.Common.BaseRepository;
using Application.Contracts.Persistence.Common.UnitOfWork;
using Application.Contracts.Services.OrderServices;
using Application.DTOs.Orders;
using Application.Specifications.Orders;
using Ardalis.Specification;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Services.OrderService;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.Services.OrderServiceTest
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<OrderService>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private OrderService _orderService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<OrderService>>();
            _mapperMock = new Mock<IMapper>();

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                shippingCalculatorService: null!,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnMappedOrders_WhenOrdersExist()
        {
            // Arrange
            var fakeOrders = new List<Order>
            {
                new Order { Id = Guid.NewGuid() },
                new Order { Id = Guid.NewGuid() }
            };

            var mappedResponses = new List<OrderResponse>
            {
                new OrderResponse { Id = fakeOrders[0].Id },
                new OrderResponse { Id = fakeOrders[1].Id }
            };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<Order>>(), default))
                .ReturnsAsync(fakeOrders);

            _unitOfWorkMock
                .Setup(u => u.Repository<Order>())
                .Returns(orderRepoMock.Object);

            _mapperMock
                .Setup(m => m.Map<List<OrderResponse>>(fakeOrders))
                .Returns(mappedResponses);

            // Act
            var result = await _orderService.GetAllAsync();

            // Assert
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
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoOrdersFound()
        {
            // Arrange
            var emptyOrders = new List<Order>();

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.ListAsync(It.IsAny<ISpecification<Order>>(), default))
                .ReturnsAsync(emptyOrders);

            _unitOfWorkMock
                .Setup(u => u.Repository<Order>())
                .Returns(orderRepoMock.Object);

            // No se necesita configurar el mapper porque no se ejecuta con lista vacía

            // Act
            var result = await _orderService.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No se encontraron órdenes")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnMappedOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var fakeOrder = new Order { Id = orderId };
            var mappedResponse = new OrderResponse { Id = orderId };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), default))
                .ReturnsAsync(fakeOrder);

            _unitOfWorkMock
                .Setup(u => u.Repository<Order>())
                .Returns(orderRepoMock.Object);

            _mapperMock
                .Setup(m => m.Map<OrderResponse>(fakeOrder))
                .Returns(mappedResponse);

            // Act
            var result = await _orderService.GetByIdAsync(orderId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(orderId));

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Se recuperó la orden")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Order>>(), default))
                .ReturnsAsync((Order?)null);

            _unitOfWorkMock
                .Setup(u => u.Repository<Order>())
                .Returns(orderRepoMock.Object);

            // Act
            var result = await _orderService.GetByIdAsync(orderId);

            // Assert
            Assert.That(result, Is.Null);

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("no encontrada")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldCreateOrder_WhenDataIsValid()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                OriginLatitude = 1.1m,
                OriginLongitude = 2.2m,
                DestinationLatitude = 3.3m,
                DestinationLongitude = 4.4m,
                Items = new List<OrderItemDto>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 2 }
        }
            };

            var fakeCustomer = new Customer { Id = request.CustomerId };
            var fakeProduct = new Product { Id = request.Items[0].ProductId };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            var customerRepoMock = new Mock<IBaseRepository<Customer>>();
            var productRepoMock = new Mock<IBaseRepository<Product>>();

            customerRepoMock
                .Setup(r => r.GetByIdAsync(request.CustomerId, default))
                .ReturnsAsync(fakeCustomer);

            productRepoMock
                .Setup(r => r.GetByIdAsync(request.Items[0].ProductId, default))
                .ReturnsAsync(fakeProduct);

            orderRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Order>(), default))
                .ReturnsAsync((Order o, CancellationToken _) => o);

            _unitOfWorkMock.Setup(u => u.Repository<Customer>()).Returns(customerRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(productRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Order>()).Returns(orderRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Complete()).Returns(Task.FromResult(1));

            var shippingServiceMock = new Mock<IShippingCalculatorService>();
            shippingServiceMock
                .Setup(s => s.CalculateShippingAsync(
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>()))
                .ReturnsAsync((50.0, 10.0m));

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                shippingServiceMock.Object,
                _loggerMock.Object
            );

            _mapperMock
                .Setup(m => m.Map<OrderResponse>(It.IsAny<Order>()))
                .Returns(new OrderResponse { Id = Guid.NewGuid() });

            // Act
            var result = await _orderService.CreateAsync(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            _loggerMock.Verify(log =>
                log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("creada exitosamente")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public void CreateAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerId = Guid.NewGuid(),
                OriginLatitude = 1.1m,
                OriginLongitude = 2.2m,
                DestinationLatitude = 3.3m,
                DestinationLongitude = 4.4m,
                Items = new List<OrderItemDto>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 2 }
        }
            };

            var fakeCustomer = new Customer { Id = request.CustomerId };

            var customerRepoMock = new Mock<IBaseRepository<Customer>>();
            var productRepoMock = new Mock<IBaseRepository<Product>>();
            var orderRepoMock = new Mock<IBaseRepository<Order>>();

            customerRepoMock
                .Setup(r => r.GetByIdAsync(request.CustomerId, default))
                .ReturnsAsync(fakeCustomer);

            productRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((Product?)null);

            _unitOfWorkMock.Setup(u => u.Repository<Customer>()).Returns(customerRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(productRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Order>()).Returns(orderRepoMock.Object);

            var shippingServiceMock = new Mock<IShippingCalculatorService>();
            shippingServiceMock
                .Setup(s => s.CalculateShippingAsync(
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>()))
                .ReturnsAsync((50.0, 10.0m));

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                shippingServiceMock.Object,
                _loggerMock.Object
            );

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _orderService.CreateAsync(request));
            Assert.That(ex!.Message, Does.Contain("Producto"));
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateOrder_WhenDataIsValid()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var request = new UpdateOrderRequest
            {
                Id = orderId,
                CustomerId = customerId,
                OriginLatitude = 1.1m,
                OriginLongitude = 2.2m,
                DestinationLatitude = 3.3m,
                DestinationLongitude = 4.4m,
                Items = new List<OrderItemDto>
        {
            new() { ProductId = productId, Quantity = 2 }
        }
            };

            var existingOrder = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                OrderItems = new List<OrderItem>(),
                DeletedAt = null
            };

            var customer = new Customer { Id = customerId };
            var product = new Product { Id = productId };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            var customerRepoMock = new Mock<IBaseRepository<Customer>>();
            var productRepoMock = new Mock<IBaseRepository<Product>>();

            orderRepoMock
                .Setup(r => r.GetByIdAsync(orderId, default))
                .ReturnsAsync(existingOrder);
            orderRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<Order>(), default))
                .Returns(Task.CompletedTask);

            customerRepoMock
                .Setup(r => r.GetByIdAsync(customerId, default))
                .ReturnsAsync(customer);

            productRepoMock
                .Setup(r => r.GetByIdAsync(productId, default))
                .ReturnsAsync(product);

            _unitOfWorkMock.Setup(u => u.Repository<Order>()).Returns(orderRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Customer>()).Returns(customerRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(productRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Complete()).ReturnsAsync(1);

            var shippingServiceMock = new Mock<IShippingCalculatorService>();
            shippingServiceMock
                .Setup(s => s.CalculateShippingAsync(
                    (double)request.OriginLatitude,
                    (double)request.OriginLongitude,
                    (double)request.DestinationLatitude,
                    (double)request.DestinationLongitude))
                .ReturnsAsync((50.0, 10.0m));

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                shippingServiceMock.Object,
                _loggerMock.Object
            );

            _mapperMock
                .Setup(m => m.Map<OrderResponse>(It.IsAny<Order>()))
                .Returns(new OrderResponse { Id = orderId });

            // Act
            var result = await _orderService.UpdateAsync(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(orderId));
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
            _loggerMock.Verify(log =>
                log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("actualizada correctamente")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenOrderDoesNotExist()
        {
            // Arrange
            var request = new UpdateOrderRequest
            {
                Id = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                OriginLatitude = 1.1m,
                OriginLongitude = 2.2m,
                DestinationLatitude = 3.3m,
                DestinationLongitude = 4.4m,
                Items = new List<OrderItemDto>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 1 }
        }
            };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.GetByIdAsync(request.Id, default))
                .ReturnsAsync((Order?)null); // ← Orden no existe

            _unitOfWorkMock.Setup(u => u.Repository<Order>()).Returns(orderRepoMock.Object);

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                shippingCalculatorService: Mock.Of<IShippingCalculatorService>(),
                _loggerMock.Object
            );

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _orderService.UpdateAsync(request));
            Assert.That(ex!.Message, Does.Contain("Orden no encontrada"));
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenDistanceOutOfRange()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var request = new UpdateOrderRequest
            {
                Id = orderId,
                CustomerId = customerId,
                OriginLatitude = 1.1m,
                OriginLongitude = 2.2m,
                DestinationLatitude = 3.3m,
                DestinationLongitude = 4.4m,
                Items = new List<OrderItemDto>
        {
            new() { ProductId = productId, Quantity = 1 }
        }
            };

            var existingOrder = new Order { Id = orderId };
            var customer = new Customer { Id = customerId };
            var product = new Product { Id = productId };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            var customerRepoMock = new Mock<IBaseRepository<Customer>>();
            var productRepoMock = new Mock<IBaseRepository<Product>>();

            orderRepoMock
                .Setup(r => r.GetByIdAsync(orderId, default))
                .ReturnsAsync(existingOrder);
            customerRepoMock
                .Setup(r => r.GetByIdAsync(customerId, default))
                .ReturnsAsync(customer);
            productRepoMock
                .Setup(r => r.GetByIdAsync(productId, default))
                .ReturnsAsync(product);

            _unitOfWorkMock.Setup(u => u.Repository<Order>()).Returns(orderRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Customer>()).Returns(customerRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(productRepoMock.Object);

            var shippingServiceMock = new Mock<IShippingCalculatorService>();
            shippingServiceMock
                .Setup(s => s.CalculateShippingAsync(
                    (double)request.OriginLatitude,
                    (double)request.OriginLongitude,
                    (double)request.DestinationLatitude,
                    (double)request.DestinationLongitude))
                .ReturnsAsync((0.5, 5.0m)); // ← Distancia inválida (menor a 1 km)

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                shippingServiceMock.Object,
                _loggerMock.Object
            );

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _orderService.UpdateAsync(request));
            Assert.That(ex!.Message, Does.Contain("distancia calculada está fuera del rango"));
        }

        [Test]
        public void UpdateAsync_ShouldThrow_WhenProductNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid(); // ← producto que no existe

            var request = new UpdateOrderRequest
            {
                Id = orderId,
                CustomerId = customerId,
                OriginLatitude = 1.1m,
                OriginLongitude = 2.2m,
                DestinationLatitude = 3.3m,
                DestinationLongitude = 4.4m,
                Items = new List<OrderItemDto>
        {
            new() { ProductId = productId, Quantity = 1 }
        }
            };

            var existingOrder = new Order { Id = orderId };
            var customer = new Customer { Id = customerId };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            var customerRepoMock = new Mock<IBaseRepository<Customer>>();
            var productRepoMock = new Mock<IBaseRepository<Product>>();

            orderRepoMock
                .Setup(r => r.GetByIdAsync(orderId, default))
                .ReturnsAsync(existingOrder);
            customerRepoMock
                .Setup(r => r.GetByIdAsync(customerId, default))
                .ReturnsAsync(customer);
            productRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((Product?)null); // ← Producto no encontrado

            _unitOfWorkMock.Setup(u => u.Repository<Order>()).Returns(orderRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Customer>()).Returns(customerRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(productRepoMock.Object);

            var shippingServiceMock = new Mock<IShippingCalculatorService>();
            shippingServiceMock
                .Setup(s => s.CalculateShippingAsync(
                    (double)request.OriginLatitude,
                    (double)request.OriginLongitude,
                    (double)request.DestinationLatitude,
                    (double)request.DestinationLongitude))
                .ReturnsAsync((50.0, 15.0m)); // ← Distancia válida

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                shippingServiceMock.Object,
                _loggerMock.Object
            );

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _orderService.UpdateAsync(request));
            Assert.That(ex!.Message, Does.Contain("no encontrado"));
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkOrderAsDeleted_WhenExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var existingOrder = new Order
            {
                Id = orderId,
                DeletedAt = null
            };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.GetByIdAsync(orderId, default))
                .ReturnsAsync(existingOrder);

            _unitOfWorkMock.Setup(u => u.Repository<Order>())
                           .Returns(orderRepoMock.Object);

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                Mock.Of<IShippingCalculatorService>(),
                _loggerMock.Object
            );

            // Act
            await _orderService.DeleteAsync(orderId);

            // Assert
            Assert.That(existingOrder.DeletedAt, Is.Not.Null);
            orderRepoMock.Verify(r => r.UpdateAsync(existingOrder, default), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.GetByIdAsync(orderId, default))
                .ReturnsAsync((Order?)null); // ← No existe

            _unitOfWorkMock.Setup(u => u.Repository<Order>())
                           .Returns(orderRepoMock.Object);

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                Mock.Of<IShippingCalculatorService>(),
                _loggerMock.Object
            );

            // Act
            var result = await _orderService.DeleteAsync(orderId);

            // Assert
            Assert.That(result, Is.False);
            orderRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Never);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Never);
        }

        [Test]
        public async Task RestoreAsync_ShouldRestoreDeletedOrder_WhenExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var deletedOrder = new Order
            {
                Id = orderId,
                DeletedAt = DateTime.UtcNow
            };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.GetByIdAsync(orderId, default))
                .ReturnsAsync(deletedOrder);

            _unitOfWorkMock.Setup(u => u.Repository<Order>())
                           .Returns(orderRepoMock.Object);

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                Mock.Of<IShippingCalculatorService>(),
                _loggerMock.Object
            );

            // Act
            await _orderService.RestoreAsync(orderId);

            // Assert
            Assert.That(deletedOrder.DeletedAt, Is.Null);
            orderRepoMock.Verify(r => r.UpdateAsync(deletedOrder, default), Times.Once);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Once);
        }

        [Test]
        public async Task RestoreAsync_ShouldReturnFalse_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.GetByIdAsync(orderId, default))
                .ReturnsAsync((Order?)null); // ← No existe

            _unitOfWorkMock.Setup(u => u.Repository<Order>())
                           .Returns(orderRepoMock.Object);

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                Mock.Of<IShippingCalculatorService>(),
                _loggerMock.Object
            );

            // Act
            var result = await _orderService.RestoreAsync(orderId);

            // Assert
            Assert.That(result, Is.False);
            orderRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Never);
            _unitOfWorkMock.Verify(u => u.Complete(), Times.Never);
        }

        [Test]
        public async Task GetDeletedAsync_ShouldReturnMappedDeletedOrders()
        {
            // Arrange
            var deletedOrders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), DeletedAt = DateTime.UtcNow },
                new Order { Id = Guid.NewGuid(), DeletedAt = DateTime.UtcNow }
            };

            var mappedResponse = new List<OrderResponse>
            {
                new() { Id = deletedOrders[0].Id },
                new() { Id = deletedOrders[1].Id }
            };

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.ListAsync(It.IsAny<OrdersDeletedSpecification>(), default))
                .ReturnsAsync(deletedOrders);

            _unitOfWorkMock.Setup(u => u.Repository<Order>())
                           .Returns(orderRepoMock.Object);

            _mapperMock.Setup(m => m.Map<List<OrderResponse>>(deletedOrders))
                       .Returns(mappedResponse);

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                Mock.Of<IShippingCalculatorService>(),
                _loggerMock.Object
            );

            // Act
            var result = await _orderService.GetDeletedAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(deletedOrders[0].Id));
        }

        [Test]
        public async Task GetDeletedAsync_ShouldReturnEmptyList_WhenNoDeletedOrdersExist()
        {
            // Arrange
            var deletedOrders = new List<Order>(); // Lista vacía

            var orderRepoMock = new Mock<IBaseRepository<Order>>();
            orderRepoMock
                .Setup(r => r.ListAsync(It.IsAny<OrdersDeletedSpecification>(), default))
                .ReturnsAsync(deletedOrders);

            _unitOfWorkMock.Setup(u => u.Repository<Order>())
                           .Returns(orderRepoMock.Object);

            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                Mock.Of<IShippingCalculatorService>(),
                _loggerMock.Object
            );

            // Act
            var result = await _orderService.GetDeletedAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            _mapperMock.Verify(m => m.Map<List<OrderResponse>>(It.IsAny<List<Order>>()), Times.Never);
        }
    }
}
