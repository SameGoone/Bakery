using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bakery.Core.Interfaces;
using Bakery.Models;

namespace Bakery.Infrastructure
{
    public class EFRoleRepository : IRoleRepository
    {
        private readonly BakeryContext _dbContext;

        public EFRoleRepository(BakeryContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Role> GetByIdAsync(int id)
        {
            return _dbContext.Roles
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<List<Role>> ListAsync()
        {
            return _dbContext.Roles
                .ToListAsync();
        }

        public Task AddAsync(Role user)
        {
            _dbContext.Roles.Add(user);
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(Role user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync();
        }
    }
}
