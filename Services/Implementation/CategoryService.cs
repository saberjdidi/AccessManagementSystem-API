using AccessManagementSystem_API.Models;
using AccessManagementSystem_API.Repository;

namespace AccessManagementSystem_API.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _repository;

        public CategoryService(IGenericRepository<Category> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync() => await _repository.GetAllAsync();

        public async Task<Category> GetCategoryByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task AddCategoryAsync(Category category) => await _repository.AddAsync(category);

        public async Task UpdateCategoryAsync(Category category) => await _repository.UpdateAsync(category);

        public async Task DeleteCategoryAsync(int id) => await _repository.DeleteAsync(id);
    }
}
