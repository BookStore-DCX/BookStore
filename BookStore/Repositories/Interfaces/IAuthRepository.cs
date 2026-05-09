using BookStore.Models;

namespace BookStore.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> ValidateUserAsync(string userName, string password);
        Task<bool> UserExistsAsync(string userName);
    }
}
