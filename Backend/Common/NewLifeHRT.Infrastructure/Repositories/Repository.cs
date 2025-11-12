using Microsoft.EntityFrameworkCore;
using NewLifeHRT.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await _dbSet.AddAsync(entity).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async Task<TEntity?> GetByIdAsync(object id)
        {
            ArgumentNullException.ThrowIfNull(id);

            // FindAsync(object) returns ValueTask<TEntity?>
            return await _dbSet.FindAsync(id).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool noTracking = false)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            IQueryable<TEntity> query = _dbSet;
            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.Where(predicate).ToListAsync().ConfigureAwait(false);
        }

        public virtual IQueryable<TEntity> Query() => _dbSet.AsQueryable();

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TEntity>> AllWithIncludeAsync(string[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includes is { Length: > 0 })
            {
                foreach (var navigation in includes.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    query = query.Include(navigation);
                }
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<TEntity> GetWithIncludeAsync(object id, string[] includes)
        {
            ArgumentNullException.ThrowIfNull(id);

            IQueryable<TEntity> query = _dbSet;

            if (includes is { Length: > 0 })
            {
                foreach (var navigation in includes.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    query = query.Include(navigation);
                }
            }

            // Uses EF.Property to avoid hard dependency on a compile-time key; mirrors your existing approach.
            return await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id)).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TEntity>> FindWithIncludeAsync(
            List<Expression<Func<TEntity, bool>>> predicates,
            string[] includes,
            bool noTracking = false)
        {
            ArgumentNullException.ThrowIfNull(predicates);

            IQueryable<TEntity> query = _dbSet;

            if (includes is { Length: > 0 })
            {
                foreach (var navigation in includes.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    query = query.Include(navigation);
                }
            }

            foreach (var predicate in predicates.Where(p => p is not null))
            {
                query = query.Where(predicate);
            }

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public virtual async Task<List<TEntity>> BulkUpdateAsync(List<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            foreach (var entity in entities)
            {
                if (entity is null) continue;

                var entry = _context.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                    entry = _context.Entry(entity);
                }
                entry.State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch
            {
                // Roll back state changes for all tracked entities in this batch
                foreach (var entity in entities.Where(e => e is not null))
                {
                    _context.Entry(entity).State = EntityState.Unchanged;
                }

                throw; // preserve stack trace
            }

            return entities;
        }

        public async Task<TEntity?> GetSingleAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
            bool noTracking = false)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            IQueryable<TEntity> query = _dbSet;

            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            if (include is not null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(predicate).ConfigureAwait(false);
        }

        public async Task AddRangeAsync(List<TEntity> entites)
        {
            ArgumentNullException.ThrowIfNull(entites);

            _dbSet.AddRange(entites.Where(e => e is not null));
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async Task<List<TEntity>> BulkDeleteAsync(List<TEntity> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            foreach (var entity in entities)
            {
                if (entity is null) continue;

                var entry = _context.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                _dbSet.Remove(entity);
            }

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch
            {
                // Restore state to prevent partial deletions staying as Modified/Deleted
                foreach (var entity in entities.Where(e => e is not null))
                {
                    _context.Entry(entity).State = EntityState.Unchanged;
                }

                throw; // preserve stack trace
            }

            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return await _dbSet.AnyAsync(predicate).ConfigureAwait(false);
        }
    }
}
