using System.Collections.Generic;

namespace HelpDesk.API.DTOs
{
    /// <summary>
    /// Envoltura estándar de respuesta de la API.
    /// </summary>
    /// <typeparam name="T">Tipo de datos de la carga útil.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la solicitud fue exitosa.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje legible para humanos (ej. descripción de error o nota de éxito).
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Los datos reales devueltos por el endpoint. Nulo cuando no aplica.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Colección opcional de errores de validación o procesamiento.
        /// </summary>
        public IEnumerable<string>? Errors { get; set; }

        /// <summary>
        /// Método de fábrica para respuestas exitosas.
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T? data = default, string? message = null) =>
            new ApiResponse<T> { Success = true, Data = data, Message = message };

        /// <summary>
        /// Método de fábrica para respuestas de error.
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message, IEnumerable<string>? errors = null) =>
            new ApiResponse<T> { Success = false, Message = message, Errors = errors };
    }
}
