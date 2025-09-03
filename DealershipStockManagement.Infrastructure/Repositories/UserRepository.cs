using DealershipStockManagement.Domain.Entities;
using DealershipStockManagement.Domain.Interfaces;
using DealershipStockManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DealershipStockManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _db;
        public UserRepository(DatabaseContext db) => _db = db;

        public Task<User?> GetByUsernameAsync(string username) =>
            _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }
}
