using Model.Entity;

namespace Service
{
    public class CityService
    {
        private static readonly List<City> cities =
        [
            new("北京", 39.904989, 116.405285),
            new("上海", 31.231706, 121.472644),
            new("广州", 23.125178, 113.280637),
            new("深圳", 22.547, 114.085947),
            new("天津", 39.125596, 117.190182),
            new("成都", 30.659462, 104.065735),
            new("重庆", 29.533155, 106.504962),
            new("杭州", 30.287459, 120.153576),
            new("南京", 32.041544, 118.767413),
            new("武汉", 30.584355, 114.298572),
            new("西安", 34.263161, 108.948024),
            new("苏州", 31.299379, 120.619585),
        ];

        public static string GetCity(double latitude, double longitude)
        {
            return cities.OrderBy(city => CalculateDistance(latitude, longitude, city.Latitude, city.Longitude)).First().Name;
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6378.137; // Radius of the Earth in kilometers
            double latDistance = DegreesToRadians(lat2 - lat1);
            double lonDistance = DegreesToRadians(lon2 - lon1);
            double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return distance;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180d;
        }
    }
}
