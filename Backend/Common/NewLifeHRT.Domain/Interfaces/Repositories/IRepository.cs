using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Domain.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(object id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = false);
        IQueryable<TEntity> Query();
        Task SaveChangesAsync();
        Task RemoveRangeAsync(IEnumerable<TEntity> entities);
        Task<IEnumerable<TEntity>> AllWithIncludeAsync(string[] includes);
        Task<TEntity> GetWithIncludeAsync(object id, string[] includes);
        Task<IEnumerable<TEntity>> FindWithIncludeAsync(List<Expression<Func<TEntity, bool>>> predicates, string[] includes, bool noTracking = false);
        Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entity);
        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate,Func<IQueryable<TEntity>, IQueryable<TEntity>> include = null,bool noTracking = false);
        Task AddRangeAsync(List<TEntity> entites);
        Task<List<TEntity>> BulkDeleteAsync(List<TEntity> entities);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);




    }
}
