namespace Infrastructure.Helpers
{
    public static class GeoUtils
    {
        private const double EarthRadiusKm = 6371;

        public static double CalculateDistanceKm(double originLat, double originLon, double destLat, double destLon)
        {
            ValidateCoordinates(originLat, originLon);
            ValidateCoordinates(destLat, destLon);

            double latDeltaRad = ToRadians(destLat - originLat);
            double lonDeltaRad = ToRadians(destLon - originLon);

            double originLatRad = ToRadians(originLat);
            double destLatRad = ToRadians(destLat);

            double a = Math.Pow(Math.Sin(latDeltaRad / 2), 2) +
                       Math.Cos(originLatRad) * Math.Cos(destLatRad) *
                       Math.Pow(Math.Sin(lonDeltaRad / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static void ValidateCoordinates(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");
        }
    }
}
