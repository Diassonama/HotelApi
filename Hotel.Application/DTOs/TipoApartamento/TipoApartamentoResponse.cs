namespace Hotel.Application.DTOs.TipoApartamento
{
    /// <summary>
    /// Response com dados do tipo de apartamento
    /// </summary>
    public class TipoApartamentoResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal ValorDiariaSingle { get; set; }
        public decimal ValorDiariaDouble { get; set; }
        public decimal ValorDiariaTriple { get; set; }
        public decimal ValorDiariaQuadruple { get; set; }
        public decimal ValorUmaHora { get; set; }
        public decimal ValorDuasHora { get; set; }
        public decimal ValortresHora { get; set; }
        public decimal ValorQuatroHora { get; set; }
        public decimal ValorNoite { get; set; }
        public decimal? Domingo { get; set; }
        public decimal? Segunda { get; set; }
        public decimal? Terca { get; set; }
        public decimal? Quarta { get; set; }
        public decimal? Quinta { get; set; }
        public decimal? Sexta { get; set; }
        public decimal? Sabado { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}