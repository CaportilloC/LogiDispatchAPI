using Infrastructure.Helpers;
using NUnit.Framework;

namespace Infrastructure.Tests.Helpers
{
    [TestFixture]
    public class GeoUtilsTests
    {
        [Test]
        public void CalculateDistanceKm_ShouldReturnCorrectDistance_ForKnownCoordinates()
        {
            // Bogotá → Medellín
            double originLat = 4.7110;
            double originLon = -74.0721;
            double destLat = 6.2442;
            double destLon = -75.5812;

            var distance = GeoUtils.CalculateDistanceKm(originLat, originLon, destLat, destLon);

            Assert.That(distance, Is.InRange(230, 260)); // ~245 km
        }

        [Test]
        public void CalculateDistanceKm_ShouldReturnZero_WhenCoordinatesAreEqual()
        {
            double lat = 10.0;
            double lon = 20.0;

            var distance = GeoUtils.CalculateDistanceKm(lat, lon, lat, lon);

            Assert.That(distance, Is.EqualTo(0).Within(0.001));
        }

        [Test]
        public void CalculateDistanceKm_ShouldReturnMaxDistance_ForAntipodalPoints()
        {
            // Punto y su antípoda (máxima distancia posible ~20015 km)
            double originLat = 0.0;
            double originLon = 0.0;
            double destLat = -0.0;
            double destLon = 180.0;

            var distance = GeoUtils.CalculateDistanceKm(originLat, originLon, destLat, destLon);

            Assert.That(distance, Is.InRange(20000, 20040)); // ~20015 km
        }

        [Test]
        public void CalculateDistanceKm_ShouldWork_WithPoles()
        {
            // Polo Norte → Polo Sur (~20000 km)
            double originLat = 90.0;
            double originLon = 0.0;
            double destLat = -90.0;
            double destLon = 0.0;

            var distance = GeoUtils.CalculateDistanceKm(originLat, originLon, destLat, destLon);

            Assert.That(distance, Is.InRange(20000, 20040));
        }

        [Test]
        public void CalculateDistanceKm_ShouldReturnShortDistance_ForNearbyPoints()
        {
            // Dos puntos muy cercanos en París
            double originLat = 48.8566;
            double originLon = 2.3522;
            double destLat = 48.8570;
            double destLon = 2.3524;

            var distance = GeoUtils.CalculateDistanceKm(originLat, originLon, destLat, destLon);

            Assert.That(distance, Is.InRange(0.01, 0.05)); // ~40 metros
        }

        [Test]
        public void CalculateDistanceKm_ShouldThrow_ForOutOfRangeLatitude()
        {
            // Arrange
            double invalidLat = 100; // fuera del rango permitido
            double lon = 0;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                GeoUtils.CalculateDistanceKm(invalidLat, lon, 0, 0));
        }

        [Test]
        public void CalculateDistanceKm_ShouldThrow_ForOutOfRangeLongitude()
        {
            // Arrange
            double lat = 0;
            double invalidLon = 200; // inválido

            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
                GeoUtils.CalculateDistanceKm(lat, invalidLon, 0, 0));

            Assert.That(ex!.Message, Does.Contain("Longitude must be between -180 and 180"));
        }

        [Test]
        public void CalculateDistanceKm_ShouldHandleNegativeCoordinatesCorrectly()
        {
            // Latitudes y longitudes negativas (válidas)
            var distance = GeoUtils.CalculateDistanceKm(-45.0, -90.0, -45.0, -89.0);
            Assert.That(distance, Is.GreaterThan(0).And.LessThan(150)); // ~80km
        }
    }
}
