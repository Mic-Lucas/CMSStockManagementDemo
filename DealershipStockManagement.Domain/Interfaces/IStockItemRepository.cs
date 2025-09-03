using DealershipStockManagement.Domain.Entities;

namespace DealershipStockManagement.Domain.Interfaces
{
    public interface IStockItemRepository
    {
        Task<(IReadOnlyList<StockItem> Items, int Total)> GetPagedAsync(
            int page, int pageSize, string? search, string? sortBy, string? sortDir);

        Task<StockItem?> GetByIdAsync(int id);
        Task AddAsync(StockItem stockItem);
        Task UpdateAsync(StockItem stockItem);
        Task DeleteAsync(int id);
    }
}
