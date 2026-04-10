using Hotel.Application.Responses;
using MediatR;
using System.Collections.Generic;

namespace Hotel.Application.Pagamento.Queries.GetPagamentosByCheckinId
{
    public class GetPagamentosByCheckinIdQuery : IRequest<BaseCommandResponse>
    {
        public int CheckinId { get; set; }
        
        public GetPagamentosByCheckinIdQuery(int checkinId)
        {
            CheckinId = checkinId;
        }
    }
}