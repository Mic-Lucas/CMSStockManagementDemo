using DealershipStockManagement.Domain.Entities;

namespace DealershipStockManagement.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
    }
}
