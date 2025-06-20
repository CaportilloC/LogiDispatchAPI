using Application.Contracts.Persistence.Common.BaseRepository;
using Application.Contracts.Persistence.Common.UnitOfWork;
using Application.DTOs.Orders.Products;
using Ardalis.Specification;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Services.ProductService;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.Tests.Services.ProductServiceTests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<ProductService>> _loggerMock;
        private Mock<IMapper> _mapperMock;
        private ProductService _productService;

        [SetUp]
        public void SetUp()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _mapperMock = new Mock<IMapper>();

            _productService = new ProductService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnMappedProducts_WhenProductsExist()
        {
            var fakeProducts = new List<Product>
            {
                new() { Id = Guid.NewGuid(), Name = "Producto 1" },
                new() { Id = Guid.NewGuid(), Name = "Producto 2" }
            };

            var mappedResponses = new List<ProductResponse>
            {
                new() { Id = fakeProducts[0].Id, Name = "Producto 1" },
                new() { Id = fakeProducts[1].Id, Name = "Producto 2" }
            };

            var repoMock = new Mock<IBaseRepository<Product>>();
            repoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Product>>(), default))
                    .ReturnsAsync(fakeProducts);

            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(repoMock.Object);
            _mapperMock.Setup(m => m.Map<List<ProductResponse>>(fakeProducts)).Returns(mappedResponses);

            var result = await _productService.GetAllAsync();

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
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProductsFound()
        {
            var repoMock = new Mock<IBaseRepository<Product>>();
            repoMock.Setup(r => r.ListAsync(It.IsAny<ISpecification<Product>>(), default))
                    .ReturnsAsync(new List<Product>());

            _unitOfWorkMock.Setup(u => u.Repository<Product>()).Returns(repoMock.Object);

            var result = await _productService.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No se encontraron productos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
