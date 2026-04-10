

using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class PagamentoRepository : RepositoryBase<Pagamento>, IPagamentoRepository
    {
       // private readonly ICaixaRepository _caixa;
      private readonly GhotelDbContext _context;
        public PagamentoRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Pagamento>> GetPagamentosAsync()
        {
            return await _context.Pagamentos
                            //  .Include(p => p.Hospedagens)
                              .Include(h => h.LancamentoCaixas)
                              .Include(h => h.Utilizadores)
                              .AsNoTracking()
                              .ToListAsync();
        }

        public async Task<List<Pagamento>> GetAllByCheckinIdAsync(int checkinId)
        {
            return await _context.Pagamentos
                .Include(p => p.Hospedes)
                    .ThenInclude(h => h.Clientes)
                .Include(p => p.Utilizadores)
                .Include(p => p.LancamentoCaixas)
                    .ThenInclude(l => l.TipoPagamentos)
                .Include(p => p.LancamentoCaixas)
                    .ThenInclude(l => l.Utilizadores)
                .Where(p => p.OrigemId == checkinId)
                .OrderBy(p => p.DateCreated)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pagamento> GetByIdAsync(int Id)
        {
            return await _context.Pagamentos
                             // .Include(p => p.Hospedagens)
                              .Include(h => h.LancamentoCaixas)
                              .Include(h => h.Utilizadores)
                            //  .AsNoTracking()
                              .FirstOrDefaultAsync(p => p.Id == Id);
        }
         public async Task<Pagamento> GetByCheckinIdAsync(int Id)
        {
            return await _context.Pagamentos
                             // .Include(p => p.Hospedagens)
                              .Include(h => h.LancamentoCaixas)
                              .Include(h => h.Utilizadores)
                            //  .AsNoTracking()
                              .FirstOrDefaultAsync(p => p.OrigemId == Id);
        }
           //    CRIA GetByCheckinIdAsync top 1 ordenado por data desc

        public async Task<Pagamento> GetByCheckinIdTop1Async(int Id)
        {
            return await _context.Pagamentos
                             // .Include(p => p.Hospedagens)
                              .Include(h => h.LancamentoCaixas)
                              .Include(h => h.Utilizadores)
                              .Where(p => p.OrigemId == Id)
                              .OrderByDescending(p => p.DateCreated)
                              .AsNoTracking()
                              .FirstOrDefaultAsync();
        }

        
         public async Task<PagamentoValorTotalResponse> GetValorTotalByCheckinIdAsync(int checkinId)
        {
            try
            {
                // ✅ VERIFICAR SE O CHECK-IN EXISTE
                var checkinExiste = await _context.Checkins.AnyAsync(c => c.Id == checkinId);
                if (!checkinExiste)
                {
                    return new PagamentoValorTotalResponse(checkinId, 0, 0, null);
                }

                // ✅ BUSCAR TODOS OS PAGAMENTOS DO CHECK-IN
                var pagamentos = await _context.Pagamentos
                    .Where(p => p.OrigemId == checkinId && p.IsActive)
                    .Select(p => new { 
                        Valor = p.Valor, 
                        DataCriacao = p.DateCreated 
                    })
                    .ToListAsync();

                // ✅ CALCULAR VALORES
                var valorTotalPago = pagamentos.Sum(p => p.Valor);
                var totalPagamentos = pagamentos.Count;
                var ultimoPagamento = pagamentos.Any() 
                    ? pagamentos.Max(p => p.DataCriacao) 
                    : (DateTime?)null;

                return new PagamentoValorTotalResponse(
                    checkinId, 
                    valorTotalPago, 
                    totalPagamentos, 
                    ultimoPagamento
                );
            }
            catch (Exception)
            {
                // ✅ EM CASO DE ERRO, RETORNAR VALORES PADRÃO
                return new PagamentoValorTotalResponse(checkinId, 0, 0, null);
            }
        }
        public async Task<Pagamento> GetByIdHospedagemAsync(int Id)
        {
            return await _context.Pagamentos
                             // .Include(p => p.Hospedagens)
                              .Include(h => h.LancamentoCaixas)
                              .Include(h => h.Utilizadores)
                              //  .AsNoTracking()
                              .FirstOrDefaultAsync(p => p.OrigemId == Id);
        }
        public async Task<IPaginatedList<Pagamento>> GetFilteredApartamentoquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            var aux = await IPaginatedList<Pagamento>.ToPagedList(
             _context.Pagamentos
                               //  .Include(p => p.Hospedagens)
                                 .Include(h => h.Utilizadores)
                                 .Include(h => h.LancamentoCaixas)
                                 .AsNoTracking()
                                 .Where(r => r.OrigemId.ToString().Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : "")
                                 )
           //      .ToListAsync();
           , paginationFilter.PageNumber, paginationFilter.PageSize);

            return aux;
        }

        public IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            IQueryable<Pagamento> query = Enumerable.Empty<Pagamento>().AsQueryable();
            query = (from apart in _context.Pagamentos
                                   //  .Include(p => p.Hospedagens)
                                     .Include(h => h.LancamentoCaixas)
                                     .Include(h => h.Utilizadores)
                                     .AsNoTracking()
                                     .Where(r => r.OrigemId.ToString().Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""))
                     select apart);
            return query;
        }
        public async Task<Pagamento> AddAsync(Pagamento pagamento)
        {
            await _context.Pagamentos.AddAsync(pagamento);
            await _context.SaveChangesAsync();
            return pagamento;
        }
       
    }
}