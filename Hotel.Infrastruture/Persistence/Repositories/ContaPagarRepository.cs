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
    public class ContaPagarRepository : RepositoryBase<ContaPagar>, IContaPagarRepository
    {
        private readonly GhotelDbContext _context;
        public ContaPagarRepository(GhotelDbContext context) : base(context) { _context = context; }

        public async Task<ContaPagar> GetByIdAsync(int id) =>
            await _context.ContasPagar.Include(c => c.Empresa).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<List<ContaPagar>> GetByEmpresaAsync(int empresaId) =>
            await _context.ContasPagar.Include(c => c.Empresa).Where(c => c.EmpresaId == empresaId).ToListAsync();

        public async Task<List<ContaPagar>> GetPendentesAsync() =>
            await _context.ContasPagar.Where(c => c.Estado != EstadoConta.Paga && c.Estado != EstadoConta.Cancelada).ToListAsync();

        public async Task<List<ContaPagar>> GetRelatorioAsync(int? empresaId, DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.ContasPagar
                .Include(c => c.Empresa)
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