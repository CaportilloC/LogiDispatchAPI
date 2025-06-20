using Application.Attributes.Services;
using Application.Contracts.Services.OrderServices;
using Infrastructure.DbContexts;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.OrderService
{
    [RegisterService(ServiceLifetime.Scoped)]
    public class ShippingCalculatorService : IShippingCalculatorService
    {
        private const double MinDistanceKm = 1;
        private const double MaxDistanceKm = 1000;

        private readonly ApplicationDbContext _context;
        private readonly ILogger<ShippingCalculatorService> _logger;

        public ShippingCalculatorService(
            ApplicationDbContext context,
            ILogger<ShippingCalculatorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(double DistanceKm, decimal ShippingCostUSD)> CalculateShippingAsync(
            double originLat, double originLon,
            double destLat, double destLon)
        {
            _logger.LogInformation("Iniciando cálculo de envío...");

            var distance = GeoUtils.CalculateDistanceKm(originLat, originLon, destLat, destLon);
            ValidateDistanceRange(distance);

            var range = await _context.ShippingCostRanges
                .Where(r => distance >= (double)r.MinKm && distance <= (double)r.MaxKm)
                .OrderBy(r => r.MaxKm - r.MinKm) // Selecciona el rango más específico
                .FirstOrDefaultAsync();

            if (range == null)
            {
                _logger.LogError("No se encontró un rango de envío para {Distance} km", distance);
                throw new InvalidOperationException("No se encontró un rango de envío para esta distancia.");
            }

            _logger.LogInformation("Cálculo exitoso. Distancia: {Distance} km, Costo: {Cost} USD", distance, range.CostUSD);
            return (distance, range.CostUSD);
        }

        private void ValidateDistanceRange(double distance)
        {
            if (distance < MinDistanceKm || distance > MaxDistanceKm)
            {
                _logger.LogWarning("Distancia fuera del rango permitido: {Distance} km", distance);
                throw new InvalidOperationException(
                    $"La distancia calculada ({distance} km) está fuera del rango permitido ({MinDistanceKm}–{MaxDistanceKm} km)."
                );
            }
        }
    }

}
