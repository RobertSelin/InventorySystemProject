using InventorySystem.API.Data;
using InventorySystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.API.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _set.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _set.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _set.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _set.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
