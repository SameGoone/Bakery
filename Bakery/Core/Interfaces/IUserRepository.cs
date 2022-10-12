using System.Collections.Generic;
using System.Threading.Tasks;
using Bakery.Models;

namespace Bakery.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<List<User>> ListAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
