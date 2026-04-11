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
    public class HistoricoRepository : RepositoryBase<Historico>, IHistoricoRepository
    {
        private readonly GhotelDbContext _context;

        public HistoricoRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<Historico>> GetAllByCheckinIdAsync(int checkinId)
        {
            return await _context.Historicos
                .Include(h => h.Utilizadores)
                .Include(h => h.Checkins)
                .Where(h => h.CheckinsId == checkinId)
                .OrderBy(h => h.DateCreated)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Historico>> GetAllByDateAsync(DateTime date)
        {
            return await _context.Historicos
                .Include(h => h.Utilizadores)
                .Include(h => h.Checkins)
                .Where(h => h.DataHora.Date == date.Date)
                .OrderBy(h => h.DataHora)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Historico>> GetHistoricoFechamentoCaixaAsync(DateTime date, int? caixaId)
        {
            // Janela do dia útil: 07h do dia D até 08h do dia D+1
            var inicioTurno = date.Date.AddHours(7);
            var fimTurno    = date.Date.AddDays(1).AddHours(8);

            var query = _context.Historicos
                .Include(h => h.Utilizadores)
                .Include(h => h.Checkins)
                .Where(h => h.DataHora >= inicioTurno && h.DataHora < fimTurno)
                .AsQueryable();

            if (caixaId.HasValue)
                query = query.Where(h => h.CaixaAberto == caixaId.Value);

            return await query
                .OrderBy(h => h.DataHora)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}