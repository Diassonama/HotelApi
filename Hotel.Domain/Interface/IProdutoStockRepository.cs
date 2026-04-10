using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IProdutoStockRepository: IRepositoryBase<ProdutoStock>
    {
         Task<ProdutoStock> GetByProdutoIdAsync(int produtoId);
        Task<List<ProdutoStock>> GetAllWithProdutoAsync();
        Task<List<ProdutoStock>> GetEstoqueBaixoAsync();
        Task<List<ProdutoStock>> GetEstoqueAltoAsync();
        Task<int> GetQuantidadeByProdutoIdAsync(int produtoId);
        Task<List<ProdutoStock>> GetByQuantidadeMinima(int quantidadeMinima);
        Task<bool> ExistsByProdutoIdAsync(int produtoId);
 
    }
}