using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hotel.Domain.Interface.Shared
{
  public interface IRepositoryBase<T> where T : class
  {
    Task Delete(int Id);

    IQueryable<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> Get(int id);
    Task<T> Get(string id);
    Task Add(T entity);
    Task Add(List<T> entity);
    Task Update(T entity);
    Task Delete(T entity);
    //  Task<IReadOnlyList<T>> GetAll();
    //  Task<T> Add(T entity);
    Task<bool> Exists(int id);
    Task<bool> Exists(string id);
    void Detach(T entity);
    void Attach(T entity);

    Task<IPaginatedList<T>> GetFilteredQuery(PaginationFilter paginationFilter, Expression<Func<T, bool>> additionalFilter = null);
    IQueryable<T> GetFilteredGenAsync(PaginationFilter paginationFilter, Expression<Func<T, bool>> additionalFilter = null);

    // void SetDbContext(DbContext dbContext);


    /* Task<T> Get(int id);
     Task<IReadOnlyList<T>> GetAll();
     Task<T> Add(T entity);
     Task<bool> Exists(int id);
     Task Update(T entity);
     Task Delete(T entity); */
  }
}