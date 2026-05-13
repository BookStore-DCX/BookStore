using BookStore.DTOs.Category;

namespace BookStore.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByNameAsync(string name);
    }
}