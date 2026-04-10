namespace Hotel.Application.DTOs.TipoHospedagem
{
    public class BuscaDiariaResponse
    {
        public int TipoApartamento { get; set; }
        public string TipoHospedagem { get; set; } = string.Empty;
        public decimal ValorCalculado { get; set; }
        public int numeroDeHospedes { get; set; }
        public int? Hora { get; set; }
        public DateTime DataReferencia { get; set; }
        public string DiaSemana { get; set; } = string.Empty;
        public string DescricaoCalculo { get; set; } = string.Empty;
        public bool Encontrado { get; set; }
    }
}