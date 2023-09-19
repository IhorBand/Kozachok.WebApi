using Kozachok.Shared.DTO.Common;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Kozachok.Shared.Abstractions.Repositories.Common
{
    public interface ICrudRepository<TEntity> : IRepository where TEntity : Entity
    {
        Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task SaveAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Delete(TEntity existingEntity);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        void DeleteRange(IEnumerable<TEntity> existingEntities);
        Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        IQueryable<TEntity> Query();
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);
        Task<PagedResult<TEntity>> PageAsync(IQueryable<TEntity> query, int page, int pageSize = GlobalConstants.DefaultPageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<TEntity>> PageAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize = GlobalConstants.DefaultPageSize, CancellationToken cancellationToken = default);
    }
}
