namespace BookStore.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IReviewRepository Reviews { get; }

        IBookConditionRepository BookConditions { get; }
        IInventoryRepository Inventories { get; }

        Task<int> SaveChangesAsync();
    }

}
