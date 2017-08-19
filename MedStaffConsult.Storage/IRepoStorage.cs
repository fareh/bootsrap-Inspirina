namespace MedStaffConsult.Storage.Abstraction
{
    public interface IRepoStorage<T> : IEntityStorage<T> where T : AbstractEntity
    {
    }
}
