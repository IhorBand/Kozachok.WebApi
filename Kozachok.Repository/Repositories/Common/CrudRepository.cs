using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.DTO.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Linq;
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

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await this.GetAsync(id);

            if (entity == null)
                return;

            this.Set.Remove(entity);
        }

        public virtual async Task<TEntity> GetAsync(Guid id) => await this.Query().FirstOrDefaultAsync(x => x.Id == id);

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) => await this.Query().FirstOrDefaultAsync(predicate);

        public virtual IQueryable<TEntity> Query() => this.Set.AsQueryable();

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate) => this.Set.Where(predicate);

        public virtual async Task SaveAsync(TEntity entity)
        {
            if (entity.Id == Guid.Empty)
                await this.Set.AddAsync(entity);
            else
                this.Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task<bool> AnyAsync(Guid id) => await this.Query(e => e.Id == id).AnyAsync();

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate) => await this.Query(predicate).AnyAsync();

        public virtual async Task AddAsync(TEntity entity) => await this.Set.AddAsync(entity);

        public virtual Task UpdateAsync(TEntity entity)
        {
            Set.Update(entity);
            return Task.CompletedTask;
        }

        public virtual PagedResult<TEntity> Page(IQueryable<TEntity> query, int page, int pageSize = GlobalConstants.DefaultPageSize)
        {
            var totalItems = query.Count();
            return new PagedResult<TEntity>(
                page,
                (int)Math.Ceiling(totalItems / (decimal)pageSize),
                totalItems,
                pageSize,
                query.Skip((page - 1) * pageSize).Take(pageSize).ToList()
            );
        }

        public virtual PagedResult<TEntity> Page(Expression<Func<TEntity, bool>> predicate, int page, int pageSize = GlobalConstants.DefaultPageSize) => this.Page(this.Query(predicate), page, pageSize);

        public virtual void Dispose() => Context.Dispose();
    }
}
