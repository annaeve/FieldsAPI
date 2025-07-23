namespace FieldsAPI.Models
{
    /// <summary>
    /// Центр и полигон (границы) поля.
    /// </summary>
    public class FieldLocation
    {
        public Coordinate Center { get; set; }
        public List<Coordinate> Polygon { get; set; }

        public FieldLocation()
        {
            Polygon = new List<Coordinate>();
        }
    }
}
