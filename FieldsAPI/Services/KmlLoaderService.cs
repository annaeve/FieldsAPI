using FieldsAPI.Configuration;
using FieldsAPI.Models;
using Microsoft.Extensions.Options;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Globalization;

namespace FieldsAPI.Services
{
    /// <summary>
    /// Загрузка и парсинг KML-файлов с координатами полей и центроидов.
    /// </summary>
    public class KmlLoaderService
    {
        private readonly string _fieldsPath;
        private readonly string _centroidsPath;

        public KmlLoaderService(IOptions<KmlPathsOptions> options)
        {
            _fieldsPath = options.Value.Fields;
            _centroidsPath = options.Value.Centroids;
        }

        /// <summary>
        /// Загружает список полей с геометрией и центрами.
        /// </summary>
        public List<Field> LoadFields()
        {
            try
            {
                var fields = ParseFieldsKml(_fieldsPath);
                var centroids = ParseCentroidsKml(_centroidsPath);

                foreach (var field in fields)
                {
                    if (centroids.TryGetValue(field.Id, out var center))
                    {
                        field.Locations.Center = center;
                    }
                }

                return fields;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ошибка при загрузке данных из KML", ex);
            }
        }

        /// <summary>
        /// Разбирает KML-файл с полигонами полей.
        /// </summary>
        private List<Field> ParseFieldsKml(string path)
        {
            var result = new List<Field>();

            try
            {
                var parser = new Parser();
                using var stream = File.OpenRead(path);
                parser.Parse(stream);

                var document = parser.Root as Kml;
                var placemarks = document?.Flatten().OfType<Placemark>() ?? Enumerable.Empty<Placemark>();

                foreach (var placemark in placemarks)
                {
                    var field = new Field
                    {
                        Id = GetSimpleData(placemark, "fid"),
                        Name = placemark.Name,
                        Size = double.TryParse(GetSimpleData(placemark, "size"), NumberStyles.Any, CultureInfo.InvariantCulture, out var size)
                            ? size : 0
                    };

                    if (placemark.Geometry is Polygon polygon)
                    {
                        var coordinates = polygon.OuterBoundary?.LinearRing?.Coordinates;
                        if (coordinates != null)
                        {
                            foreach (var coord in coordinates)
                            {
                                field.Locations.Polygon.Add(new Coordinate(coord.Latitude, coord.Longitude));
                            }
                        }
                    }

                    result.Add(field);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при разборе KML с полигонами: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Разбирает KML-файл с центроидами полей.
        /// </summary>
        private Dictionary<string, Coordinate> ParseCentroidsKml(string path)
        {
            var centroids = new Dictionary<string, Coordinate>();

            try
            {
                var parser = new Parser();
                using var stream = File.OpenRead(path);
                parser.Parse(stream);

                var document = parser.Root as Kml;
                var placemarks = document?.Flatten().OfType<Placemark>() ?? Enumerable.Empty<Placemark>();

                foreach (var placemark in placemarks)
                {
                    var id = GetSimpleData(placemark, "fid");
                    if (placemark.Geometry is Point point)
                    {
                        var coord = point.Coordinate;
                        centroids[id] = new Coordinate(coord.Latitude, coord.Longitude);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при разборе KML с центроидами: {ex.Message}");
            }

            return centroids;
        }

        /// <summary>
        /// Получает значение поля по его имени из расширенных данных Placemark.
        /// </summary>
        private string GetSimpleData(Placemark placemark, string fieldName)
        {
            if (placemark.ExtendedData != null)
            {
                foreach (var data in placemark.ExtendedData.Data)
                {
                    if (data.Name == fieldName)
                        return data.Value;
                }

                foreach (var schema in placemark.ExtendedData.SchemaData)
                {
                    foreach (var sd in schema.SimpleData)
                    {
                        if (sd.Name == fieldName)
                            return sd.Text;
                    }
                }
            }

            return string.Empty;
        }
    }
}