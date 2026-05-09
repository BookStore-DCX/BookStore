using BookStore.Models;

namespace BookStore.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
