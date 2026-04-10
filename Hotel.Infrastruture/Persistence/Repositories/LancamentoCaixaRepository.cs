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
    public class LancamentoCaixaRepository : RepositoryBase<LancamentoCaixa> , ILancamentoCaixaRepository
    {
        private readonly GhotelDbContext _context;
        public LancamentoCaixaRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;

        }

         public async Task<LancamentoCaixa> GetByPagamentoIdAsync(int Id)
        {
            return await _context.LancamentoCaixas
            .Include(p => p.Pagamentos)
            .Include(p => p.TipoPagamentos)
            .Include(p => p.Utilizadores)

            .OrderByDescending(p => p.DateCreated)

            .FirstOrDefaultAsync(p => p.PagamentosId == Id);
                             

        }

        public async Task<List<LancamentoCaixa>> GetAllByDateAsync(DateTime date)
        {
            return await _context.LancamentoCaixas
                .Include(l => l.TipoPagamentos)
                .Include(l => l.Pagamentos)
                .Include(l => l.Utilizadores)
                .Where(l => l.DataHoraLancamento.Date == date.Date)
                .OrderBy(l => l.DataHoraLancamento)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<LancamentoCaixa>> GetMovimentoCaixaAsync(DateTime? dataInicio, DateTime? dataFim, string perfil, string usuario)
        {
            var query = _context.LancamentoCaixas
                .Include(l => l.TipoPagamentos)
                .Include(l => l.Utilizadores)
                .AsQueryable();

            // Filtro de operador baseado no perfil
            string operadorFiltro = (perfil?.ToUpper() == "ADMINISTRADOR" || perfil?.ToUpper() == "SUPERADMIN") 
                ? null 
                : usuario;

            if (!string.IsNullOrEmpty(operadorFiltro))
                query = query.Where(l => l.Utilizadores != null && l.Utilizadores.UserName == operadorFiltro);

            if (dataInicio.HasValue)
                query = query.Where(l => l.DataHoraLancamento.Date >= dataInicio.Value.Date);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataHoraLancamento.Date <= dataFim.Value.Date);

            return await query
                .OrderBy(l => l.DataHoraLancamento)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<LancamentoCaixa>> GetPagamentosFechamentoCaixaAsync(DateTime date, int? caixaId, string perfil, string usuario)
        {
            var query = _context.LancamentoCaixas
                .Include(l => l.TipoPagamentos)
                .Include(l => l.Pagamentos)
                .Include(l => l.Utilizadores)
                .Where(l => l.DataHoraLancamento.Date == date.Date)
                .AsQueryable();

            if (caixaId.HasValue)
                query = query.Where(l => l.CaixasId == caixaId.Value);

            string operadorFiltro = (perfil?.ToUpper() == "ADMINISTRADOR" || perfil?.ToUpper() == "SUPERADMIN")
                ? null
                : usuario;

            if (!string.IsNullOrWhiteSpace(operadorFiltro))
                query = query.Where(l => l.Utilizadores != null && l.Utilizadores.UserName == operadorFiltro);

            return await query
                .OrderBy(l => l.DataHoraLancamento)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}