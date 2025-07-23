using FieldsAPI.Models;

namespace FieldsAPI.Services
{
    /// <summary>
    /// Сервис для работы с полями, загруженными из KML-файлов.
    /// </summary>
    public class FieldService
    {
        private readonly List<Field> _fields;

        /// <summary>
        /// Инициализация нового экземпляра сервиса и загрузка данных полей.
        /// </summary>
        /// <param name="loader">Сервис загрузки KML-файлов.</param>
        public FieldService(KmlLoaderService loader)
        {
            _fields = loader.LoadFields();
        }

        /// <summary>
        /// Получение списка всех полей.
        /// </summary>
        /// <returns>Список полей.</returns>
        public List<Field> GetAllFields()
        {
            return _fields;
        }

        /// <summary>
        /// Нахождение поля по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор поля.</param>
        /// <returns>Поле или null, если не найдено.</returns>
        public Field GetById(string id)
        {
            foreach (var field in _fields)
            {
                if (field.Id == id)
                {
                    return field;
                }
            }

            return null;
        }

        /// <summary>
        /// Получение размера поля по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор поля.</param>
        /// <returns>Размер поля или null, если поле не найдено.</returns>
        public double? GetSize(string id)
        {
            var field = GetById(id);
            if (field != null)
            {
                return field.Size;
            }

            return null;
        }
    }
}
