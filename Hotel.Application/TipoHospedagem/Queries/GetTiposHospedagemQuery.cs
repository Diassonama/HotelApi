using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.TipoHospedagem.Queries
{
    /// <summary>
    /// Query para buscar todos os tipos de hospedagem
    /// </summary>
    public class GetTiposHospedagemQuery : IRequest<BaseCommandResponse>
    {
        public bool IncluirHospedagens { get; set; }

        public GetTiposHospedagemQuery(bool incluirHospedagens = false)
        {
            IncluirHospedagens = incluirHospedagens;
        }
    }
}