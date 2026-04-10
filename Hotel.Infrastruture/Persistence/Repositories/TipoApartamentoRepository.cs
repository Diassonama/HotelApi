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
     public class TipoApartamentoRepository : RepositoryBase<TipoApartamento>, ITipoApartamentoRepository
    {
         private readonly GhotelDbContext _context;
        public TipoApartamentoRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TipoApartamento>> GetApartamentoAsync()
        {
                  return await _context.TipoApartamentos
                                 //   .Include(p=>p.Apartamentos)
                                    .ToListAsync();                        
        }
        public async Task<TipoApartamento> GetByIdAsync(int Id)
        {
                  return await _context.TipoApartamentos
                                    .Include(p=>p.Apartamentos)
                                    .FirstOrDefaultAsync(p=>p.Id == Id);
                                  
        }

        public async  Task<IPaginatedList<TipoApartamento>> GetFilteredApartamentoquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
               var  aux = await  IPaginatedList<TipoApartamento>.ToPagedList(
                _context.TipoApartamentos
                                    .Include(p=>p.Apartamentos)
                                    .Where(r=> r.Descricao.Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter)? paginationFilter.FieldFilter.ToLower() : "")                                     
                                    )

              ,paginationFilter.PageNumber,paginationFilter.PageSize );

            return aux;
        }

        public  IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
           IQueryable<TipoApartamento> query = Enumerable.Empty<TipoApartamento>().AsQueryable();
           query = (from apart in _context.TipoApartamentos
                                    .Include(p=>p.Apartamentos)
                                    .Where(r=> r.Descricao.Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter)? paginationFilter.FieldFilter.ToLower() : "") )
                                    select  apart);
            return   query;
        }
    } 
}