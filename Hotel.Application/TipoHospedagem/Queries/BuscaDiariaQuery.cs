using Hotel.Application.DTOs.TipoHospedagem;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.TipoHospedagem.Queries
{
    /// <summary>
    /// Query para buscar valor da diária baseado no tipo de hospedagem
    /// </summary>
    public class BuscaDiariaQuery : IRequest<BaseCommandResponse>
    {
        public int TipoApartamento { get; set; }
        public int numeroDeHospedes { get; set; }
        public string TipoHospedagem { get; set; } = string.Empty;
        public int? Hora { get; set; }
        public DateTime? DataReferencia { get; set; }

        public BuscaDiariaQuery(BuscaDiariaRequest request)
        {
            TipoApartamento = request.TipoApartamento;
            numeroDeHospedes = request.Dias;
            TipoHospedagem = request.TipoHospedagem?.ToUpper() ?? string.Empty;
            Hora = request.Hora;
            DataReferencia = request.DataReferencia ?? DateTime.Now;
        }

        public BuscaDiariaQuery(int tipoApartamento, int dias, string tipoHospedagem, int? hora = null, DateTime? dataReferencia = null)
        {
            TipoApartamento = tipoApartamento;
            numeroDeHospedes = dias;
            TipoHospedagem = tipoHospedagem?.ToUpper() ?? string.Empty;
            Hora = hora;
            DataReferencia = dataReferencia ?? DateTime.Now;
        }
    }
}