namespace BookStore.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthorRepository Authors { get; }

        IBookRepository Books { get; }

        IBookConditionRepository BookConditions { get; }

        ICategoryRepository Categories { get; }

        IInventoryRepository Inventories { get; }

        IPurchaseLogRepository PurchaseLogs { get; }

        IPublisherRepository Publishers { get; }

        IShoppingCartRepository ShoppingCarts { get; }

        IStateRepository States { get; }

        IUserRepository Users { get; }

        IReviewRepository Reviews { get; }

        Task<int> SaveChangesAsync();

    }
}