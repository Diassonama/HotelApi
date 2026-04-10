namespace Hotel.Application.DTOs.TipoHospedagem
{
    /// <summary>
    /// Response com dados do tipo de hospedagem
    /// </summary>
    public class TipoHospedagemResponse
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public float Valor { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}