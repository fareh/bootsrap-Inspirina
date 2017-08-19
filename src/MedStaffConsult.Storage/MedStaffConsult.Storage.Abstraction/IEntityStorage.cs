using System.Collections.Generic;

namespace MedStaffConsult.Storage.Abstraction
{
    public interface IEntityStorage<T> where T : AbstractEntity
    {
        T Get(int uid);

        void Set(IEnumerable<T> entities);

        void Set(T document);

        List<T> GetAll();

        List<T> Get(IEnumerable<int> uids);

        void Remove(int uid);
    }
}