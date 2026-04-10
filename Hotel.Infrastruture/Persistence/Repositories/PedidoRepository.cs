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
    public class PedidoRepository : RepositoryBase<Pedido>, IPedidoRepository
    {
        private readonly GhotelDbContext _context;

        public PedidoRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<Pedido> GetByIdWithItemsAsync(int id)
        {
            return await _context.Pedidos
                .Include(p => p.ItemPedidos)
                .Include(p => p.Hospede)
                    .ThenInclude(h => h.Clientes)
                .Include(p => p.PontoVenda)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pedido>> GetByCheckinIdAsync(int checkinId)
        {
            return await _context.Pedidos
                .Include(p => p.ItemPedidos)
                .Include(p => p.Hospede)
                    .ThenInclude(h => h.Clientes)
                .Include(p => p.PontoVenda)
                .Where(p => p.IdCheckin == checkinId)
                .OrderByDescending(p => p.DataPedido)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}