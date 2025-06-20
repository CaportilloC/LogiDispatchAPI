using Domain.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Services.OrderService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Tests.Services
{
    public class ShippingCalculatorServiceTests
    {
        private ApplicationDbContext _context = null!;
        private ShippingCalculatorService _service = null!;

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _context.ShippingCostRanges.Add(new ShippingCostRange
            {
                Id = 1,
                MinKm = 0,
                MaxKm = 1000,
                CostUSD = 25.00m,
                CreatedAt = DateTime.UtcNow
            });

            _context.SaveChanges();

            var logger = new LoggerFactory().CreateLogger<ShippingCalculatorService>();
            _service = new ShippingCalculatorService(_context, logger);
        }

        [Test]
        public async Task CalculateShippingAsync_ShouldReturnCorrectDistanceAndCost_WhenInRange()
        {
            // Cali → Bogotá
            double originLat = 3.4516;
            double originLon = -76.5320;
            double destLat = 4.7110;
            double destLon = -74.0721;

            var result = await _service.CalculateShippingAsync(originLat, originLon, destLat, destLon);

            Assert.That(result.DistanceKm, Is.GreaterThan(250).And.LessThan(400));
            Assert.That(result.ShippingCostUSD, Is.EqualTo(25.00m));
        }

        [Test]
        public void CalculateShippingAsync_ShouldThrow_WhenDistanceIsOutOfRange()
        {
            // Arrange: Mismo punto (0 km)
            double originLat = 4.7110;
            double originLon = -74.0721;
            double destLat = 4.7110;
            double destLon = -74.0721;

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CalculateShippingAsync(originLat, originLon, destLat, destLon));

            Assert.That(ex!.Message, Does.Contain("fuera del rango permitido"));
        }

        [Test]
        public void CalculateShippingAsync_ShouldThrow_WhenDistanceExceedsMax()
        {
            // Bogotá → Nueva York
            double originLat = 4.7110;
            double originLon = -74.0721;
            double destLat = 40.7128;
            double destLon = -74.0060;

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CalculateShippingAsync(originLat, originLon, destLat, destLon));

            Assert.That(ex!.Message, Does.Contain("fuera del rango permitido"));
        }

        [Test]
        public void CalculateShippingAsync_ShouldThrow_WhenNoShippingRangeFound()
        {
            // Arrange: Bogotá → Cali (distancia ≈ 300 km, válida)
            double originLat = 4.7110;
            double originLon = -74.0721;
            double destLat = 3.4516;
            double destLon = -76.5320;

            // Asegurar que no haya ShippingCostRanges
            _context.ShippingCostRanges.RemoveRange(_context.ShippingCostRanges);
            _context.SaveChanges();

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CalculateShippingAsync(originLat, originLon, destLat, destLon));

            Assert.That(ex!.Message, Does.Contain("No se encontró un rango de envío"));
        }

        [Test]
        public async Task CalculateShippingAsync_ShouldWork_WhenDistanceIsCloseToUpperLimit()
        {
            _context.ShippingCostRanges.Add(new ShippingCostRange
            {
                Id = 2,
                MinKm = 750,
                MaxKm = 1000,
                CostUSD = 50.00m,
                CreatedAt = DateTime.UtcNow
            });
            _context.SaveChanges();

            double originLat = 4.7110;   // Bogotá
            double originLon = -74.0721;
            double destLat = 8.9824;     // Ciudad de Panamá
            double destLon = -79.5199;

            var result = await _service.CalculateShippingAsync(originLat, originLon, destLat, destLon);

            Assert.That(result.DistanceKm, Is.InRange(765, 770));
            Assert.That(result.ShippingCostUSD, Is.EqualTo(50.00m));
        }

    }
}
