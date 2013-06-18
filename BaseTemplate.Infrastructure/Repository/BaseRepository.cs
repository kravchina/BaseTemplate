using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BaseTemplate.Infrastructure.EntityModel;

namespace BaseTemplate.Infrastructure.Repository
{
    public interface IBaseRepository<TEntityModel, TViewModel>
        where TEntityModel : BaseModel
        where TViewModel : class
    {
        TViewModel Find(object id);
        IEnumerable<TViewModel> Get(params Expression<Func<TEntityModel, object>>[] includeExpressions);
        void Insert(TEntityModel entity);
        void InsertManyRelation<T>(TEntityModel entity, Func<TEntityModel, ICollection<T>> includeExpression) where T : BaseModel;
        void Update(TEntityModel entity);
        void UpdateManyRelation<T>(TEntityModel entity, Expression<Func<TEntityModel, ICollection<T>>> includeExpression) where T : BaseModel;
        void DeleteById(object id);
        void SaveChanges();
    }

    public class BaseRepository<TEntityModel, TViewModel> : IBaseRepository<TEntityModel, TViewModel>
        where TEntityModel : BaseModel
        where TViewModel : class
    {
        protected readonly EfDbContext dbContext;

        protected BaseRepository(EfDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual TViewModel Find(object id)
        {
            var entity = dbContext.Set<TEntityModel>().Find(id);

            return Mapper.DynamicMap<TViewModel>(entity);
        }

        public virtual IEnumerable<TViewModel> Get(params Expression<Func<TEntityModel, object>>[] includeExpressions)
        {
            var querableEntities = dbContext.Set<TEntityModel>().AsQueryable();

            querableEntities = includeExpressions.Aggregate(querableEntities, (current, includeExpression) => current.Include(includeExpression));

            foreach (var entity in querableEntities)
            {
                yield return Mapper.DynamicMap<TViewModel>(entity);
            }
        }

        public virtual void Insert(TEntityModel entity)
        {
            dbContext.Set<TEntityModel>().Add(entity);
        }

        public virtual void InsertManyRelation<T>(TEntityModel entity, Func<TEntityModel, ICollection<T>> includeExpression) where T : BaseModel
        {
            var entityCollection = includeExpression(entity);

            var includeEntities = entityCollection.ToList();

            entityCollection.Clear();

            foreach (var item in includeEntities)
            {
                entityCollection.Add(dbContext.Set<T>().Find(item.Id));
            }
        }

        public virtual void Update(TEntityModel entity)
        {
            var dbEntity = dbContext.Set<TEntityModel>().Single(t => t.Id == entity.Id);

            dbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
        }

        public virtual void UpdateManyRelation<T>(TEntityModel entity, Expression<Func<TEntityModel, ICollection<T>>> includeExpression) where T : BaseModel
        {
            var dbEntity = dbContext
                    .Set<TEntityModel>()
                    .Include(includeExpression)
                    .Single(t => t.Id == entity.Id);

            var getProperty = includeExpression.Compile();

            var oldCollection = getProperty(dbEntity);

            var newCollection = getProperty(entity);

            // Remove deleted
            foreach (var oldEntity in oldCollection.ToList())
            {
                if (!newCollection.Any(t => t.Id == oldEntity.Id))
                {
                    oldCollection.Remove(oldEntity);
                }
            }

            // Add new
            foreach (var newEntity in newCollection)
            {
                if (!oldCollection.Any(t => t.Id == newEntity.Id))
                {
                    var newDbEntity = dbContext.Set(newEntity.GetType()).Find(newEntity.Id);

                    oldCollection.Add((T)newDbEntity);
                }
            }
        }

        public virtual void DeleteById(object id)
        {
            TEntityModel entity = dbContext.Set<TEntityModel>().Find(id);

            dbContext.Set<TEntityModel>().Remove(entity);
        }

        public virtual void SaveChanges()
        {
            dbContext.SaveChanges();
        }
    }
}
