using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NewLifeHRT.Infrastructure.Repositories
{
    public abstract class Repository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        protected readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected Repository(TContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = false)
        {

            if (noTracking)
            {
                return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
            }
            else
            {
                return await _dbSet.Where(predicate).ToListAsync();
            }
        }

        public virtual IQueryable<TEntity> Query()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> AllWithIncludeAsync(string[] includes)
        {
            IQueryable<TEntity> query = _dbSet.Include(includes.First());

            foreach (var include in includes.Skip(1))
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> GetWithIncludeAsync(object id, string[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes != null && includes.Any())
            {
                query = query.Include(includes[0]);
                foreach (var include in includes.Skip(1))
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(e =>
                EF.Property<object>(e, "Id").Equals(id));
        }

        public virtual async Task<IEnumerable<TEntity>> FindWithIncludeAsync(List<Expression<Func<TEntity, bool>>> predicates, string[] includes, bool noTracking = false)
        {
            IQueryable<TEntity> query = _dbSet.Include(includes.First());
            foreach (var include in includes.Skip(1))
            {
                query = query.Include(include);
            }

            foreach (var predicate in predicates)
            {
                query = query.Where(predicate);
            }

            if (noTracking)
            {
                query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public virtual async Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entities)
        {
            foreach(var entity in entities)
            {
                var entry = _context.Entry(entity);
                _dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                foreach (var entity in entities)
                {
                    var entry = _context.Entry(entity);
                    entry.State = EntityState.Unchanged;
                }

                throw e;
            }

            return entities;
        }
        public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null, bool noTracking = false)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (noTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }
        public async Task AddRangeAsync(List<TEntity> entites)
        {
            _dbSet.AddRange(entites);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<List<TEntity>> BulkDeleteAsync(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }

                _dbSet.Remove(entity);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                foreach (var entity in entities)
                {
                    var entry = _context.Entry(entity);
                    entry.State = EntityState.Unchanged;
                }

                throw; 
            }

            return entities;
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
