using DealershipStockManagement.Application.Dtos;

namespace DealershipStockManagement.Application.Services
{
    public interface IStockService
    {
        /// <summary>
        /// Returns an object: { total, page, pageSize, data = List<StockItemListDto> }
        /// imageUrlFactory is a function that, given an imageId, returns an absolute URL (constructed in API layer).
        /// </summary>
        Task<object> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, string? sortDir, Func<int, string?> imageUrlFactory);
        Task<object?> GetByIdAsync(int id, Func<int, string?> imageUrlFactory);

        Task<int> CreateAsync(StockItemCreateDto dto);
        Task UpdateAsync(int id, StockItemUpdateDto dto);
        Task DeleteAsync(int id);

        Task<(byte[] Data, string FileName, string MimeType)?> GetImageAsync(int imageId);
    }
}
