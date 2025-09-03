using DealershipStockManagement.Application.Dtos;

namespace DealershipStockManagement.Application.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto dto);
        Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
    }
}
