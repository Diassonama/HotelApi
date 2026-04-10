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

    public class ProdutoRepository : RepositoryBase<Produtos>, IProdutoRepository
    {
        private readonly GhotelDbContext _context;
        public ProdutoRepository(GhotelDbContext dbContext, GhotelDbContext context) : base(dbContext)
        {
            _context = context;
        }

        public async Task<List<Produtos>> GetAllWithStockAsync(string? searchTerm = null, string? categoria = null, bool? apenasAtivos = true)
        {
            var query = _context.Produtos
                .Include(p => p.ProdutoStocks)
                .AsQueryable();

            if (apenasAtivos.HasValue && apenasAtivos.Value)
            {
                query = query.Where(p => p.IsActive);
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(p => p.PontoDeVenda.Nome.Contains(categoria));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    p.Nome.Contains(searchTerm));
            }

            return await query
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Produtos> GetByIdWithStockAsync(int id)
        {
            return await _context.Produtos
                .Include(p => p.ProdutoStocks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> IsCodigoUniqueAsync(int codigo)
        {
            return !await _context.Produtos
                .AnyAsync(p => p.Id == codigo);
        }

        public async Task<List<Produtos>> GetByCategoriaAsync(string categoria)
        {
            return await _context.Produtos
                .Include(p => p.ProdutoStocks)
                .Where(p => p.PontoDeVenda.Nome == categoria && p.IsActive)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Produtos> GetByCodigoAsync(int codigo)
        {
            return await _context.Produtos
                // .Include(p => p.ProdutoStocks)
                .FirstOrDefaultAsync(p => p.Id == codigo);
        }

        public async Task<List<Produtos>> GetProdutosComEstoqueBaixoAsync()
        {
            return await _context.Produtos
                .Include(p => p.ProdutoStocks)
                .Where(p => p.IsActive &&
                            p.ProdutoStocks.Any(ps => ps.Quantidade <= ps.QuantidadeMinima))
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<List<Produtos>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return new List<Produtos>();

            return await _context.Produtos
                .Include(p => p.ProdutoStocks)
                .Where(p => p.IsActive &&
                           (p.Nome.Contains(searchTerm) ||


                            p.PontoDeVenda.Nome.Contains(searchTerm)))
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Produtos> AddAsync(Produtos entity)
        {

            await _context.Produtos.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(Produtos entity)
        {
            entity.LastModifiedDate = DateTime.UtcNow;

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Produtos entity)
        {
            // Soft delete - marcar como inativo ao invés de deletar
            entity.IsActive = false;
            entity.LastModifiedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }
        
        public async Task<List<Produtos>> GetAllWithDetailsAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.PontoDeVenda)
                .Include(p => p.TaxTableEntry)
                .Include(p => p.ProductTypes)
                .Include(p => p.TaxExemptionReason)
                .Include(p => p.ProdutoStocks)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<Produtos> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.PontoDeVenda)
                .Include(p => p.TaxTableEntry)
                .Include(p => p.ProductTypes)
                .Include(p => p.TaxExemptionReason)
                .Include(p => p.ProdutoStocks)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Produtos>> GetByCategoriaIdAsync(int categoriaId)
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.PontoDeVenda)
                .Where(p => p.CategoriaId == categoriaId && p.IsActive)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<List<Produtos>> GetByPontoVendasIdAsync(int pontoVendasId)
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.PontoDeVenda)
                .Where(p => p.PontoDeVendasId == pontoVendasId && p.IsActive)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<List<Produtos>> GetAvailableProductsAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.PontoDeVenda)
                .Where(p => p.EstaDisponivel && p.IsActive)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<List<Produtos>> GetExpiredProductsAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.PontoDeVenda)
                .Where(p => p.EstaExpirado && p.IsActive)
                .OrderBy(p => p.DataExpiracao)
                .ToListAsync();
        }

        public async Task<List<Produtos>> GetLowStockProductsAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.PontoDeVenda)
                .Where(p => p.EstoqueAbaixoDoMinimo && p.IsActive)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

    }
}