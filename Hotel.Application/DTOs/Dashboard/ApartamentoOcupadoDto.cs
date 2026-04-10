using System;

namespace Hotel.Application.DTOs.Dashboard
{
    public class ApartamentoOcupadoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Hospede { get; set; }
        public string Tipo { get; set; }
        public int Adultos { get; set; }
        public int QuantidadeCrianca { get; set; }
        public string DataAbertura { get; set; }
        public string PrevisaoFechamento { get; set; }
        public string Pais { get; set; }
        public string Empresa { get; set; }
        public string Checkout { get; set; }
        public int QuartoOcupado { get; set; }
        public int QuartoLivre { get; set; }
        public int TotalQuarto { get; set; }
        public decimal PerOcupado { get; set; }
        public decimal PerLivre { get; set; }
        public decimal ValorDiaria { get; set; }
    }
}
