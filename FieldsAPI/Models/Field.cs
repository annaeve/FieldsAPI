namespace FieldsAPI.Models
{
    /// <summary>
    /// Поле с идентификатором, названием, площадью и координатами.
    /// </summary>
    public class Field
    {
        public string Id {  get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public FieldLocation Locations { get; set; }

        public Field()
        {
            Locations = new FieldLocation();
        }
    }
}
