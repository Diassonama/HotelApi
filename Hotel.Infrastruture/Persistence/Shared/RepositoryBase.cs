using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;
namespace Hotel.Infrastruture.Persistence.Shared
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class //where TContext: DbContext
    {
        private GhotelDbContext _context;
        //  private DbContext dbContext;

        public RepositoryBase(GhotelDbContext dbContext)
        {
            _context = dbContext;
            //   this._context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        }
        public void SetDbContext(DbContext dbContext)
        {
            _context = (GhotelDbContext)dbContext;
        }

        public async Task Update(T entity)
        {
            try
            {
              /*   // Verifica se a entidade já está sendo rastreada
                var trackedEntity = _context.ChangeTracker.Entries<T>()
                                            .FirstOrDefault(e => e.Entity.Equals(entity))?.Entity;
                if (trackedEntity != null)
                {
                    // Se a entidade já estiver sendo rastreada, atualiza o estado para Modified
                    _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
                }
                else
                {
                    _context.Attach(entity);
                    _context.Entry(entity).State = EntityState.Modified;
                } */
                  var registro = _context.Set<T>().Update(entity);
                 registro.State = EntityState.Modified;
                 _context.ChangeTracker.DetectChanges(); 
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task Delete(T entity)
        {
            try
            {
                //  var registro = await PegarPeloId(id);
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task Delete(int Id)
        {

            var entity = await _context.Set<T>().FindAsync(Id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();

        }

        public async Task Add(T entity)
        {
            try
            {
                await _context.AddAsync(entity);
                _context.ChangeTracker.DetectChanges();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);

            }
        }

        public async Task Add(List<T> entity)
        {
            try
            {
                await _context.AddRangeAsync(entity);
                _context.ChangeTracker.DetectChanges();
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<T> Get(int id)
        {
            try
            {
                var entity = await _context.Set<T>().FindAsync(id);
                if (entity is null) return null;  // throw new InvalidOperationException("Registro não encontrado");


                _context.Entry(entity).State = EntityState.Detached;
                return entity;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public async Task<T> Get(string id)
        {
            var registro = await _context.Set<T>().FindAsync(id);
            _context.Entry(registro).State = EntityState.Detached;
            return registro;
        }

        public IQueryable<T> GetAll()
        {
            try
            {
                return _context.Set<T>();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _context.Set<T>().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Exists(int id)
        {
            var entity = await Get(id);
            return entity != null;
        }

        public async Task<bool> Exists(string id)
        {
            var entity = await Get(id);
            return entity != null;
        }

        public async Task<IPaginatedList<T>> GetFilteredQuery(Domain.Interface.Shared.PaginationFilter paginationFilter, Expression<Func<T, bool>> additionalFilter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (additionalFilter != null)
                query = query.Where(additionalFilter);

            if (!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter))
                query = query.Where(e => EF.Functions.Like(e.ToString(), $"%{paginationFilter.FieldFilter.ToLower()}%"));

            return await IPaginatedList<T>.ToPagedList(query, paginationFilter.PageNumber, paginationFilter.PageSize);

        }

        public void Detach(T entity)
        {
            var entry = _context.Entry(entity);
            if (entry != null)
            {
                entry.State = EntityState.Detached;
            }
        }
        public void Attach(T entity)
        {
                _context.Set<T>().Attach(entity);
            
        }

        public IQueryable<T> GetFilteredGenAsync(Domain.Interface.Shared.PaginationFilter paginationFilter, Expression<Func<T, bool>> additionalFilter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (additionalFilter != null)
                query = query.Where(additionalFilter);

            if (!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter))
                query = query.Where(e => EF.Functions.Like(e.ToString(), $"%{paginationFilter.FieldFilter.ToLower()}%"));

            return query;
        }
    }
}