using System.Collections.Generic;
using System.Threading.Tasks;
using Bakery.Models;

namespace Bakery.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(int id);
        Task<List<Role>> ListAsync();
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
    }
}
