using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.TipoHospedagem
{
    public class BuscaDiariaRequest
    {
        [Required(ErrorMessage = "Tipo de apartamento é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Tipo de apartamento deve ser maior que zero")]
        public int TipoApartamento { get; set; }

        [Required(ErrorMessage = "Número de dias é obrigatório")]
        [Range(1, 4, ErrorMessage = "Número de dias deve estar entre 1 e 4")]
        public int Dias { get; set; }

        [Required(ErrorMessage = "Tipo de hospedagem é obrigatório")]
        public string TipoHospedagem { get; set; } = string.Empty;

        [Range(1, 4, ErrorMessage = "Número de horas deve estar entre 1 e 4")]
        public int? Hora { get; set; }

        /// <summary>
        /// Data específica para cálculo de valores especiais por dia da semana
        /// Se não informada, será usada a data atual
        /// </summary>
        public DateTime? DataReferencia { get; set; }
    }
}