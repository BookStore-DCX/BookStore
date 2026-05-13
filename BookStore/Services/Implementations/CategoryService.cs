using AutoMapper;
using BookStore.DTOs.Category;
using BookStore.Exceptions;
using BookStore.Repositories.Interfaces;
using BookStore.Services.Interfaces;

namespace BookStore.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _uow.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByNameAsync(string name)
        {
            var category = await _uow.Categories.GetByCategoryNameAsync(name)
                ?? throw new NotFoundException(
                    $"Category with name '{name}' not found"
                );
            return _mapper.Map<CategoryDto>(category);
        }
    }
}