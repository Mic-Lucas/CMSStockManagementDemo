using DealershipStockManagement.Domain.Entities;

namespace DealershipStockManagement.Domain.Interfaces
{
    public interface IImageRepository
    {
        Task AddImagesAsync(IEnumerable<Image> images);
        Task UpdateImageAsync(Image image);
        Task<List<Image>> GetImagesAsync(int stockItemId);
        Task<Image?> GetImageAsync(int imageId);
        Task DeleteImagesAsync(IEnumerable<int> imageIds);
    }
}
