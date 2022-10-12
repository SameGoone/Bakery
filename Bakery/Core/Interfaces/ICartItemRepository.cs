using System.Collections.Generic;
using System.Threading.Tasks;
using Bakery.Models;

namespace Bakery.Core.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem> GetByIdAsync(int id);
        Task<List<CartItem>> ListAsync();
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
    }
}
