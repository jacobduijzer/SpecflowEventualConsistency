using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpecflowEventualConsistency.Domain
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task Add(TEntity entity);
        Task AddRange(IEnumerable<TEntity> entities);
        Task Delete(Expression<Func<TEntity, bool>> expression);
        Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> expression);
    }
}