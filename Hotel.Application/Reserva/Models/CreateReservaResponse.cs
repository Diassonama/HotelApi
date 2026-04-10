using System;

namespace Hotel.Application.Reserva.Models
{
    /// <summary>
    /// Modelo de resposta para criação de reserva
    /// </summary>
    public class CreateReservaResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ReservaId { get; set; }
        public object Data { get; set; }
    }

    /// <summary>
    /// Modelo de resposta para erro
    /// </summary>
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string[] Errors { get; set; }
    }
}
