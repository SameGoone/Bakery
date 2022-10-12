using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bakery.Core.Interfaces;
using Bakery.Models;

namespace Bakery.Infrastructure
{
    public class EFProductRepository : IProductRepository
    {
        private readonly BakeryContext _dbContext;

        public EFProductRepository(BakeryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Product> GetByIdAsync(int id)
        {
            return _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<List<Product>> ListAsync()
        {
            return _dbContext.Products
                .ToListAsync();
        }

        public Task AddAsync(Product product)
        {
            _dbContext.Products.Add(product);
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync();
        }

        public Task RemoveAsync(Product product)
        {
            _dbContext.Products.Remove(product);
            return _dbContext.SaveChangesAsync();
        }
    }
}
