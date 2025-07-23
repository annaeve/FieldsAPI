namespace FieldsAPI.Configuration
{
    /// <summary>
    /// Параметры конфигурации путей к KML-файлам с полигонами и центроидами.
    /// </summary>
    public class KmlPathsOptions
    {
        public string Fields { get; set; } = string.Empty;
        public string Centroids { get; set; } = string.Empty;
    }
}
