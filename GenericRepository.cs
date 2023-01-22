using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Webolar.Framework
{
    public class GenericRepository<TEntity,TContext> : IGenericRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DbSet<TEntity> _dbSet;
        public TContext _context;

        public GenericRepository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public  bool Modify<Entity>(Entity record, Expression<Func<Entity, bool>> condition) where Entity : class
        {
            bool result = false;
            bool exists = false;
     
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        DbSet<Entity> EntityTable = _context.Set<Entity>();
                        var query = EntityTable.Where(condition);
                        foreach (var rec in query)
                        {
                            exists = true;

                            PropertyInfo[] properties = EntityCloner.GetProperties(typeof(Entity));
                            foreach (PropertyInfo property in properties)
                            {
                                property.SetValue(rec, property.GetValue(record));
                            }
                        }
                        if (exists)
                        {
                             _context.SaveChanges();
                            dbTransaction.Commit();
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        dbTransaction.Rollback();
                        log.Error(string.Format("Cannot modify a record in the database."), ex);
                    }
                }
        
            return result;
        }
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetById(int entityId)
        {
            return _dbSet.Find(entityId);
        }

        public virtual TEntity GetById(string entityId)
        {
            return _dbSet.Find(entityId);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Delete(int entityId)
        {
            var entity = _dbSet.Find(entityId);
            _dbSet.Remove(entity);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void Delete(string entityId)
        {
            var entity = _dbSet.Find(entityId);
            _dbSet.Remove(entity);
        }
    }
}
