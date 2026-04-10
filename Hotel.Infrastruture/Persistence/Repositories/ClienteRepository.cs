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
    public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
    {
         private readonly GhotelDbContext _context;
        public ClienteRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _context.Clientes
                              .Include(p => p.Hospedes)
                              .Include(p => p.Empresa)
                              .Include(p => p.Lavandarias)
                            //  .Include(h => h.Reservas)
                              .Include(h => h.Paises)
                              .AsNoTracking()
                              .ToListAsync();
        }
        public async Task<Cliente> GetByIdAsync(int Id)
        {
            return await _context.Clientes
                              .Include(p => p.Hospedes)
                              .Include(p => p.Empresa)
                              .Include(p => p.Lavandarias)
                            //  .Include(h => h.Reservas)
                              .Include(h => h.Paises)
                           //   .AsNoTracking()
                              .FirstOrDefaultAsync(p => p.Id == Id);

        }
        public async Task<IPaginatedList<Cliente>> GetFilteredApartamentoquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            var aux = await IPaginatedList<Cliente>.ToPagedList(
             _context.Clientes
                                .Include(p => p.Hospedes)
                                 .Include(p => p.Lavandarias)
                               //  .Include(h => h.Reservas)
                                 .Include(h => h.Paises)
                                 .AsNoTracking()
                                 .Where(r => r.Nome.Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : "")

                                 )
           //      .ToListAsync();
           , paginationFilter.PageNumber, paginationFilter.PageSize);

            return aux;
        }

        public IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            IQueryable<Cliente> query = Enumerable.Empty<Cliente>().AsQueryable();
            query = (from apart in _context.Clientes
                                    .Include(p => p.Hospedes)
                                     .Include(p => p.Lavandarias)
                                  //   .Include(h => h.Reservas)
                                     .Include(h => h.Paises)
                                     .AsNoTracking()
                                     .Where(r => r.Nome.Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""))
                     select apart);
            return query;
        }
    }
}