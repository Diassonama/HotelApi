using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.TipoHospedagem.Queries
{
    /// <summary>
    /// Query para buscar tipo de hospedagem por ID
    /// </summary>
    public class GetTipoHospedagemByIdQuery : IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }

        public GetTipoHospedagemByIdQuery(int id)
        {
            Id = id;
        }
    }
}