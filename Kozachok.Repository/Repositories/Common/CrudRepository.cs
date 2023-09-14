using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.DTO.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kozachok.Shared;

namespace Kozachok.Repository.Repositories.Common
{
    public abstract class CrudRepository<TEntity> : ICrudRepository<TEntity> where TEntity : Entity
    {
        protected DbContext Context;
        protected DbSet<TEntity> Set;

        public CrudRepository(DbContext context)
        {
            this.Context = context;
            this.Set = this.Context.Set<TEntity>();
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await this.GetAsync(id, cancellationToken);

            if (entity == null)
                return;

            this.Set.Remove(entity);
        }

        public virtual async Task DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            foreach (var id in ids)
            {
                await DeleteAsync(id, cancellationToken);
            }
        }

        public virtual async Task<TEntity> GetAsync(
            Guid id, 
            CancellationToken cancellationToken = default) => await this.Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public virtual async Task<TEntity> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default) => await this.Query().FirstOrDefaultAsync(predicate, cancellationToken);

        public virtual IQueryable<TEntity> Query() => this.Set.AsQueryable();

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate) => this.Set.Where(predicate);

        public virtual async Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity.Id == Guid.Empty)
                await this.Set.AddAsync(entity, cancellationToken);
            else
                this.Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default) => await this.Query(e => e.Id == id).AnyAsync(cancellationToken);

        public virtual async Task<bool> AnyAsync(
            Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default) => await this.Query(predicate).AnyAsync(cancellationToken);

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => await this.Set.AddAsync(entity, cancellationToken);

        public virtual void Update(TEntity entity)
        {
            Set.Update(entity);
        }

        public virtual async Task<PagedResult<TEntity>> PageAsync(
            IQueryable<TEntity> query, 
            int page, 
            int pageSize = GlobalConstants.DefaultPageSize, 
            CancellationToken cancellationToken = default)
        {
            var totalItems = await query.CountAsync(cancellationToken);
            return new PagedResult<TEntity>(
                page,
                (int)Math.Ceiling(totalItems / (decimal)pageSize),
                totalItems,
                pageSize,
                await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken)
            );
        }

        public virtual async Task<PagedResult<TEntity>> PageAsync(
            Expression<Func<TEntity, bool>> predicate, 
            int page, 
            int pageSize = GlobalConstants.DefaultPageSize, 
            CancellationToken cancellationToken = default) => await this.PageAsync(this.Query(predicate), page, pageSize, cancellationToken);

        public virtual void Dispose() => Context.Dispose();
    }
}
