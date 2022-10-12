using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bakery.Core.Interfaces;
using Bakery.Models;

namespace Bakery.Infrastructure
{
    public class EFCartItemRepository : ICartItemRepository
    {
        private readonly BakeryContext _dbContext;

        public EFCartItemRepository(BakeryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<CartItem> GetByIdAsync(int id)
        {
            return _dbContext.CartItems
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<List<CartItem>> ListAsync()
        {
            return _dbContext.CartItems
                .ToListAsync();
        }

        public Task AddAsync(CartItem cartItem)
        {
            _dbContext.CartItems.Add(cartItem);
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(CartItem cartItem)
        {
            _dbContext.Entry(cartItem).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync();
        }
    }
}
