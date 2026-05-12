namespace BookStore.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IPublisherRepository Publishers { get; }
        ICategoryRepository Categories { get; }
        IStateRepository States { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        IPurchaseLogRepository PurchaseLogs { get; }
        IReviewRepository Reviews { get; }
        IBookConditionRepository BookConditions { get; }
        IInventoryRepository Inventories { get; }
        IBookRepository Books { get; }
        IAuthorRepository Authors { get; }

        Task<int> SaveChangesAsync();
    }
}