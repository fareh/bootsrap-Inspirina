using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MedStaffConsult.Storage.Abstraction
{
    public interface IEntityStorage<TEntity> where TEntity : AbstractEntity
    {
        Task<TEntity> Get(int uid);

        Task Set(IEnumerable<TEntity> entities);

        Task Set(TEntity document);

        Task<IEnumerable<TEntity>> GetAll();

        Task<IEnumerable<TEntity>> Get(IEnumerable<int> uids);

        Task Remove(int uid);

        Task Update(TEntity entity);

        Task Update(IEnumerable<TEntity> entities);

        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);
    }
}