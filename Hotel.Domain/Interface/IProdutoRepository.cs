using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IProdutoRepository : IRepositoryBase<Produtos>
    {
         Task<List<Produtos>> GetAllWithStockAsync(string? searchTerm = null, string? categoria = null, bool? apenasAtivos = true);
        Task<Produtos> GetByIdWithStockAsync(int id);
        Task<bool> IsCodigoUniqueAsync(int codigo);
        Task<List<Produtos>> GetByCategoriaAsync(string categoria);
        Task<Produtos> GetByCodigoAsync(int codigo);
        Task<List<Produtos>> GetProdutosComEstoqueBaixoAsync();
        Task<List<Produtos>> SearchAsync(string searchTerm);

        
    }
}