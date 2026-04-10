using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class ProdutoStockRepository : RepositoryBase<ProdutoStock>, IProdutoStockRepository
    {
         private readonly GhotelDbContext _context;
        public ProdutoStockRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

         public async Task<ProdutoStock> GetByProdutoIdAsync(int produtoId)
        {
            return await _context.ProdutoStocks
                .Include(ps => ps.Produto)
                .FirstOrDefaultAsync(ps => ps.ProdutoId == produtoId);
        }

        public async Task<List<ProdutoStock>> GetAllWithProdutoAsync()
        {
            return await _context.ProdutoStocks
                .Include(ps => ps.Produto)
                .Where(ps => ps.Produto.IsActive)
                .OrderBy(ps => ps.Produto.Nome)
                .ToListAsync();
        }

        public async Task<List<ProdutoStock>> GetEstoqueBaixoAsync()
        {
            return await _context.ProdutoStocks
                .Include(ps => ps.Produto)
                .Where(ps => ps.Quantidade <= ps.QuantidadeMinima && ps.Produto.IsActive)
                .OrderBy(ps => ps.Produto.Nome)
                .ToListAsync();
        }

        public async Task<List<ProdutoStock>> GetEstoqueAltoAsync()
        {
            return await _context.ProdutoStocks
                .Include(ps => ps.Produto)
                .Where(ps => ps.Quantidade >= ps.QuantidadeMaxima && ps.Produto.IsActive)
                .OrderBy(ps => ps.Produto.Nome)
                .ToListAsync();
        }

        public async Task<int> GetQuantidadeByProdutoIdAsync(int produtoId)
        {
            var stock = await _context.ProdutoStocks
                .FirstOrDefaultAsync(ps => ps.ProdutoId == produtoId);
            
            return stock?.Quantidade ?? 0;
        }

        public async Task<List<ProdutoStock>> GetByQuantidadeMinima(int quantidadeMinima)
        {
            return await _context.ProdutoStocks
                .Include(ps => ps.Produto)
                .Where(ps => ps.QuantidadeMinima >= quantidadeMinima && ps.Produto.IsActive)
                .OrderBy(ps => ps.Produto.Nome)
                .ToListAsync();
        }

        public async Task<bool> ExistsByProdutoIdAsync(int produtoId)
        {
            return await _context.ProdutoStocks
                .AnyAsync(ps => ps.ProdutoId == produtoId);
        }

        public  async Task<ProdutoStock> AddAsync(ProdutoStock entity)
        {
          
            await _context.ProdutoStocks.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            return entity;
        }

        public  async Task UpdateAsync(ProdutoStock entity)
        {
          
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public  async Task DeleteAsync(ProdutoStock entity)
        {
            // Hard delete para stock - remover completamente
            _context.ProdutoStocks.Remove(entity);
            await _context.SaveChangesAsync();
        }

        // Métodos auxiliares específicos para movimentação de estoque
        public async Task<ProdutoStock> AdicionarQuantidadeAsync(int produtoId, int quantidade, string? observacoes = null)
        {
            var stock = await GetByProdutoIdAsync(produtoId);
            
            if (stock == null)
            {
                // Criar novo stock se não existir
                stock = new ProdutoStock
                {
                    ProdutoId = produtoId,
                    Quantidade = quantidade,
                    QuantidadeMinima = 0,
                    QuantidadeMaxima = int.MaxValue,
                   
                    Observacoes = observacoes ?? "Criação inicial de estoque"
                };
                
                return await AddAsync(stock);
            }
            else
            {
                stock.Quantidade += quantidade;
               
                stock.Observacoes = observacoes ?? $"Adição de {quantidade} unidades";
                
                await UpdateAsync(stock);
                return stock;
            }
        }

        public async Task<ProdutoStock> RemoverQuantidadeAsync(int produtoId, int quantidade, string? observacoes = null)
        {
            var stock = await GetByProdutoIdAsync(produtoId);
            
            if (stock == null)
                throw new InvalidOperationException($"Stock não encontrado para o produto {produtoId}");
            
            if (stock.Quantidade < quantidade)
                throw new InvalidOperationException($"Quantidade insuficiente em estoque. Disponível: {stock.Quantidade}, Solicitado: {quantidade}");
            
            stock.Quantidade -= quantidade;
            stock.Observacoes = observacoes ?? $"Remoção de {quantidade} unidades";
            
            await UpdateAsync(stock);
            return stock;
        }

        public async Task<ProdutoStock> AjustarQuantidadeAsync(int produtoId, int novaQuantidade, string? observacoes = null)
        {
            var stock = await GetByProdutoIdAsync(produtoId);
            
            if (stock == null)
            {
                // Criar novo stock se não existir
                stock = new ProdutoStock
                {
                    ProdutoId = produtoId,
                    Quantidade = novaQuantidade,
                    QuantidadeMinima = 0,
                    QuantidadeMaxima = int.MaxValue,
                    Observacoes = observacoes ?? "Criação inicial com ajuste"
                };
                
                return await AddAsync(stock);
            }
            else
            {
                var quantidadeAnterior = stock.Quantidade;
                stock.Quantidade = novaQuantidade;
                stock.Observacoes = observacoes ?? $"Ajuste de {quantidadeAnterior} para {novaQuantidade} unidades";
                
                await UpdateAsync(stock);
                return stock;
            }
        }
    }
}
