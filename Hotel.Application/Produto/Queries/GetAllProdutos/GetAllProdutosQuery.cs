using Hotel.Application.Responses;
using MediatR;
using System.Collections.Generic;

namespace Hotel.Application.Produto.Queries.GetAllProdutos
{
    public class GetAllProdutosQuery : IRequest<BaseQueryResponse<List<Domain.Entities.Produtos>>>
    {
        // Propriedades opcionais para filtros
        public int? CategoriaId { get; set; }
        public int? PontoDeVendasId { get; set; }
        public bool? ApenasAtivos { get; set; } = true;
        public bool? ApenasDisponiveis { get; set; }
        public string Nome { get; set; }
    }
}