namespace BookStore.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IShoppingCartRepository ShoppingCarts { get; }
        IPurchaseLogRepository PurchaseLogs { get; }
        Task<int> SaveChangesAsync();
    }
}
