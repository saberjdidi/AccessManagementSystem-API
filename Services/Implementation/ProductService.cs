using AccessManagementSystem_API.Models;
using AccessManagementSystem_API.Repository;

namespace AccessManagementSystem_API.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repository;

        public ProductService(IGenericRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task AddProductAsync(Product product) => await _repository.AddAsync(product);

        public async Task DeleteProductAsync(int id) => await _repository.DeleteAsync(id);

        public async Task<IEnumerable<Product>> GetAllProductsAsync() => _repository.GetAll(includeProperties: "Category");
        //public async Task<IEnumerable<Product>> GetAllProductsAsync() => await _repository.GetAllAsync();

        public async Task<Product> GetProductByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task UpdateProductAsync(Product product) => await _repository.UpdateAsync(product);
    }
}
