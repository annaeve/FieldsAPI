using Microsoft.AspNetCore.Mvc;
using FieldsAPI.Services;
using System;

namespace FieldsAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с полями.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly FieldService _fieldService;
        private readonly GeometryService _geometryService;

        public FieldsController(FieldService fieldService, GeometryService geometryService)
        {
            _fieldService = fieldService;
            _geometryService = geometryService;
        }

        /// <summary>
        /// Получение списка всех полей.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var fields = _fieldService.GetAllFields();
                return Ok(fields);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении списка полей: {ex.Message}");
            }
        }

        /// <summary>
        /// Получение размера поля по ID.
        /// </summary>
        [HttpGet("{id}/size")]
        public IActionResult GetSize(string id)
        {
            try
            {
                var size = _fieldService.GetSize(id);
                if (size == null)
                    return NotFound();

                return Ok(size);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении размера: {ex.Message}");
            }
        }

        /// <summary>
        /// Получение расстояния от точки до полигона поля.
        /// </summary>
        [HttpGet("{id}/distance")]
        public IActionResult GetDistance(string id, [FromQuery] double lat, [FromQuery] double lng)
        {
            try
            {
                var distance = _geometryService.GetDistanceToPoint(id, lat, lng);
                if (distance == null)
                    return NotFound();

                return Ok(distance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при вычислении расстояния: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверка принадлежности точки одному из полей.
        /// </summary>
        [HttpGet("contains")]
        public IActionResult CheckContains([FromQuery] double lat, [FromQuery] double lng)
        {
            try
            {
                var field = _geometryService.CheckPointInside(lat, lng);
                if (field == null)
                    return Ok(false);

                return Ok(new { id = field.Id, name = field.Name });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при проверке вхождения: {ex.Message}");
            }
        }
    }
}
