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
    public class ContaReceberRepository : RepositoryBase<ContaReceber>, IContaReceberRepository
    {
        private readonly GhotelDbContext _context;
        public ContaReceberRepository(GhotelDbContext context) : base(context) { _context = context; }

        public async Task<ContaReceber> GetByIdAsync(int id) =>
            await _context.ContasReceber.Include(c => c.Empresa).Include(c => c.Checkins).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<List<ContaReceber>> GetByEmpresaAsync(int empresaId) =>
            await _context.ContasReceber.Include(c => c.Empresa).Where(c => c.EmpresaId == empresaId).ToListAsync();

        public async Task<ContaReceber> GetByCheckinIdAsync(int checkinId) =>
            await _context.ContasReceber.Include(c => c.Empresa).FirstOrDefaultAsync(c => c.CheckinsId == checkinId);

        public async Task<List<ContaReceber>> GetRelatorioAsync(int? empresaId, DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.ContasReceber
                .Include(c => c.Empresa)
                .Include(c => c.Checkins)
                .AsQueryable();

            if (empresaId.HasValue)
                query = query.Where(c => c.EmpresaId == empresaId.Value);

            if (dataInicio.HasValue)
                query = query.Where(c => c.DataEmissao >= dataInicio.Value.Date);

            if (dataFim.HasValue)
            {
                var fim = dataFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.DataEmissao <= fim);
            }

            return await query
                .OrderBy(c => c.DataVencimento)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}