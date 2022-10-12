using System.Collections.Generic;
using System.Threading.Tasks;
using Bakery.Models;

namespace Bakery.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> ListAsync();
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task RemoveAsync(Product product);
    }
}
