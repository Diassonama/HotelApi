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
    public class EmpresaRepository : RepositoryBase<Empresa>, IEmpresaRepository
    {
        private readonly GhotelDbContext _context;
        public EmpresaRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
          public IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            /* IQueryable<Empresa> query = Enumerable.Empty<Empresa>().AsQueryable();
            query = (from apart in _context.Empresas
                                    .Include(p => p.Clientes)
                                     .Include(p => p.Hospedagens)
                                     .Include(h => h.Reservas)
                                     .Include(h => h.FacturaEmpresas)
                             
                                     .Where(r => r.RazaoSocial.Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""))
                     select apart);
            return query; */

            IQueryable<Empresa> query = _context.Empresas
    .Include(p => p.Clientes)
    .Include(p => p.Hospedagens)
    .Include(h => h.Reservas)
    .Include(h => h.FacturaEmpresas);

if (!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter))
{
    string filtro = paginationFilter.FieldFilter.ToLower().Trim();
    query = query.Where(r => r.RazaoSocial.ToLower().Contains(filtro));
}

return query;
        }
    }
}