using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bakery.Core.Interfaces;
using Bakery.Models;

namespace Bakery.Infrastructure
{
    public class EFUserRepository : IUserRepository
    {
        private readonly BakeryContext _dbContext;

        public EFUserRepository(BakeryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User> GetByIdAsync(int id)
        {
            return _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<List<User>> ListAsync()
        {
            return _dbContext.Users
                .Include(u => u.Role)
                .ToListAsync();
        }

        public Task AddAsync(User user)
        {
            _dbContext.Users.Add(user);
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync();
        }
    }
}
