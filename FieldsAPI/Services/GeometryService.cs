using FieldsAPI.Models;

namespace FieldsAPI.Services
{
    /// <summary>
    /// Сервис для геометрических вычислений над полями.
    /// </summary>
    public class GeometryService
    {
        private readonly FieldService _fieldService;

        public GeometryService(FieldService fieldService)
        {
            _fieldService = fieldService;
        }

        /// <summary>
        /// Вычисление расстояния от центра указанного поля до заданной точки.
        /// </summary>
        /// <param name="fieldId">Идентификатор поля.</param>
        /// <param name="lat">Широта точки.</param>
        /// <param name="lng">Долгота точки.</param>
        /// <returns>Расстояние в метрах или null, если поле не найдено.</returns>
        public double? GetDistanceToPoint(string fieldId, double lat, double lng)
        {
            try
            {
                var field = _fieldService.GetById(fieldId);

                if (field?.Locations?.Center == null)
                    return null;

                var centerLat = field.Locations.Center.Latitude;
                var centerLng = field.Locations.Center.Longitude;

                return CalculateHaversineDistance(centerLat, centerLng, lat, lng);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Проверка нахождения точки внутри любого из полей.
        /// </summary>
        /// <param name="lat">Широта точки.</param>
        /// <param name="lng">Долгота точки.</param>
        /// <returns>Поле, в котором находится точка, или null.</returns>
        public Field CheckPointInside(double lat, double lng)
        {
            try
            {
                var fields = _fieldService.GetAllFields();
                foreach (var field in fields)
                {
                    if (IsPointInsidePolygon(lat, lng, field.Locations.Polygon))
                        return field;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Проверка нахождения точки внутри полигона.
        /// </summary>
        private bool IsPointInsidePolygon(double lat, double lng, List<Coordinate> polygon)
        {
            if (polygon == null || polygon.Count < 3) return false;

            bool inside = false;
            int count = polygon.Count;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                double xi = polygon[i].Latitude;
                double yi = polygon[i].Longitude;
                double xj = polygon[j].Latitude;
                double yj = polygon[j].Longitude;

                bool intersect = ((yi > lng) != (yj > lng)) &&
                    (lat < (xj - xi) * (lng - yi) / ((yj - yi) + 1e-10) + xi);

                if (intersect)
                    inside = !inside;
            }

            return inside;
        }

        /// <summary>
        /// Вычисление расстояния между двумя точками с использованием формулы гаверсинусов.
        /// </summary>
        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Радиус Земли в метрах
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// Перевод градусов в радианы.
        /// </summary>
        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
