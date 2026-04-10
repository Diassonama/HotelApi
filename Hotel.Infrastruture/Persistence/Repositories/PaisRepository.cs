using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class PaisRepository : RepositoryBase<Pais>, IPaisRepository
    {
        private readonly GhotelDbContext _context;
        public PaisRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
         public IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            IQueryable<Pais> query = Enumerable.Empty<Pais>().AsQueryable();
            query = (from pais in _context.Paises
                                     .Include(c=>c.Clientes)
                                     .Where(r => r.Nome.Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""))
                     select pais);
            return query;
        }
    }
}