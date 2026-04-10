using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
  public class HospedagemRepository : RepositoryBase<Hospedagem>, IHospedagemRepository
  {

    private readonly GhotelDbContext _context;
        public HospedagemRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Hospedagem>> GetHospedagemAsync()
    {
      return await _context.Hospedagems
                        .Include(p => p.Empresas)
                        .Include(p => p.Apartamentos)
                        //  .Include(p=> p.Hospedes).ThenInclude(c=>c.Clientes)
                        //   .Include(p=>p.Pagamentos).ThenInclude(c=>c.LancamentoCaixas)
                        .Include(p => p.Checkins)
                        .Include(m => m.MotivoViagens)
                        .Include(m => m.TipoHospedagens)
                        .AsNoTracking()
                        .ToListAsync();
    }
    public async Task<Hospedagem> GetByIdAsync(int Id)
    {
      return await _context.Hospedagems
                        .Include(p => p.Empresas)
                        .Include(p => p.Apartamentos)
                        .Include(p => p.Checkins)
                        .Include(m => m.MotivoViagens)
                        .Include(m => m.TipoHospedagens)
                        
                        .FirstOrDefaultAsync(p => p.Id == Id);
    }
     public async Task<Hospedagem> GetByIdAsyncAsNotrack(int Id)
    {
      return await _context.Hospedagems.AsNoTracking().FirstOrDefaultAsync(p => p.Id == Id);
    }
    public async Task<Hospedagem> GetByCheckinIdAsync(int Id)
    {
      return await _context.Hospedagems
                        //  .Include(p=>p.Empresas)
                         .Include(p=>p.Apartamentos)
                        /* .Include(p=> p.Checkins)
                         .Include(m=>m.MotivoViagens)
                         .Include(m=>m.TipoHospedagens) */
                        //.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.CheckinsId == Id);
    }
    public async Task<IPaginatedList<Hospedagem>> GetFilteredquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
    {
      var aux = await IPaginatedList<Hospedagem>.ToPagedList(
       _context.Hospedagems
                           .Include(p => p.Empresas)
                           .Include(p => p.Apartamentos)
                           //  .Include(p=> p.Hospedes).ThenInclude(c=>c.Clientes)
                           .Include(p => p.Checkins)
                           //  .Include(p=>p.Pagamentos).ThenInclude(c=>c.LancamentoCaixas)
                           .Include(m => m.MotivoViagens)
                           .Include(m => m.TipoHospedagens)
                           .AsNoTracking()
                           .Where(r => r.Id.ToString().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""
                           //  || r.Descricao = .ToString().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter)? paginationFilter.FieldFilter.ToLower() : ""
                           )

                           )
     //      .ToListAsync();
     , paginationFilter.PageNumber, paginationFilter.PageSize);

      return aux;
    }

    public IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
    {
      IQueryable<Hospedagem> query = Enumerable.Empty<Hospedagem>().AsQueryable();
      query = (from apart in _context.Hospedagems
                               .Include(p => p.Empresas)
                               .Include(p => p.Apartamentos)
                               //  .Include(p=> p.Hospedes).ThenInclude(c=>c.Clientes)
                               .Include(p => p.Checkins)
                               //  .Include(p=>p.Pagamentos).ThenInclude(c=>c.LancamentoCaixas)
                               .Include(m => m.MotivoViagens)
                               .Include(m => m.TipoHospedagens)
                               .AsNoTracking()
                               .Where(r => r.Id.ToString().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""))
               select apart);
      return query;
    }
    public async Task<int> AddAsync(Hospedagem checkins)
    {
      _context.Hospedagems.Add(checkins);
      await _context.SaveChangesAsync();

      return checkins.Id;
    }
  }
}