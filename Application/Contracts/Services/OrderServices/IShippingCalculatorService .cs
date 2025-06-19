namespace Application.Contracts.Services.OrderServices
{
    public interface IShippingCalculatorService
    {
        Task<(double DistanceKm, decimal ShippingCostUSD)> CalculateShippingAsync(
            double originLat, double originLon,
            double destLat, double destLon);
    }
}
