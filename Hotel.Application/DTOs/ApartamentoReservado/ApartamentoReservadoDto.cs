#nullable enable
using System;

namespace Hotel.Application.DTOs.ApartamentoReservado
{
    /// <summary>
    /// DTO para transferência de dados de Apartamento Reservado
    /// </summary>
    public class ApartamentoReservadoDto
    {
        public int Id { get; set; }
        public int ApartamentoId { get; set; }
        public int ReservaId { get; set; }
        public float ValorDiaria { get; set; }
        public float Total { get; set; }
        public string? UtilizadoresId { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public bool Ativo { get; set; }

        // Propriedades de navegação (opcional, para incluir dados relacionados)
        public string? NumeroApartamento { get; set; }
        public string? TipoApartamento { get; set; }
        public string? NomeCliente { get; set; }
        public string? EmailCliente { get; set; }
        public string? NomeUsuario { get; set; }
    }
}
