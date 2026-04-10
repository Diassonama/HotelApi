using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Pagamento.Queries.GetPagamentoById
{
    public class GetPagamentoByIdQuery : IRequest<BaseQueryResponse<Domain.Entities.Pagamento>>
    {
        public int Id { get; set; }
        
        public GetPagamentoByIdQuery(int id)
        {
            Id = id;
        }
    }
}