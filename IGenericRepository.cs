using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Webolar.Framework;

public interface IGenericRepository<TEntity> where TEntity : class
{
    IEnumerable<TEntity> Get(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = "");

    TEntity GetById(int entityId);
    TEntity GetById(string entityId);

    void Insert(TEntity entity);

    void Delete(int entityId);
    void Delete(TEntity entity);

    void Delete(string entityId);

    void Update(TEntity entity);
}