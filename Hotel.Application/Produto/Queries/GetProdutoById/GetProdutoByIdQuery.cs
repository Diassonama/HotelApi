using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Produto.Queries.GetProdutoById
{
    public class GetProdutoByIdQuery : IRequest<BaseQueryResponse<Domain.Entities.Produtos>>
    {
        public int Id { get; set; }
        
        public GetProdutoByIdQuery(int id)
        {
            Id = id;
        }
    }
}