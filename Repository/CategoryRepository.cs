using AccessManagementSystem_API.Data;
using AccessManagementSystem_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AccessManagementSystem_API.Repository
{
    public class CategoryRepository : IGenericRepository<Category>
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync() => await _context.Categories.ToListAsync();
        //public async Task<IEnumerable<Category>> GetAllAsync() => await _context.Categories.Include(c => c.Products).ToListAsync();

        public async Task<Category> GetByIdAsync(int id) =>
            await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Category entity)
        {
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category entity)
        {
            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public IEnumerable<Category> GetAll(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null, bool tracked = false)
        {
            throw new NotImplementedException();
        }
    }

}
