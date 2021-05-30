using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpecflowEventualConsistency.Domain;

namespace SpecflowEventualConsistency.Infrastructure
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : Entity
    {
        private readonly AppDbContext _appDbContext;

        public Repository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Add(TEntity entity)
        {
            await _appDbContext.Set<TEntity>()
                .AddAsync(entity)
                .ConfigureAwait(false);
        }

        public async Task AddRange(IEnumerable<TEntity> entities)
        {
            await _appDbContext.Set<TEntity>()
                .AddRangeAsync(entities)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> Get(Expression<Func<TEntity, bool>> expression)
        {
            return await _appDbContext.Set<TEntity>()
                .Where(expression)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}