using DealershipStockManagement.Domain.Entities;
using DealershipStockManagement.Domain.Interfaces;
using DealershipStockManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealershipStockManagement.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly DatabaseContext _db;
        public ImageRepository(DatabaseContext db) => _db = db;

        public async Task AddImagesAsync(IEnumerable<Image> images)
        {
            _db.Images.AddRange(images);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateImageAsync(Image image)
        {
            _db.Images.Update(image);
            await _db.SaveChangesAsync();
        }

        public Task<List<Image>> GetImagesAsync(int stockItemId) =>
            _db.Images.Where(i => i.StockItemId == stockItemId).ToListAsync();

        public Task<Image?> GetImageAsync(int imageId) =>
            _db.Images.FirstOrDefaultAsync(i => i.Id == imageId);

        public async Task DeleteImagesAsync(IEnumerable<int> imageIds)
        {
            var imgs = _db.Images.Where(i => imageIds.Contains(i.Id));
            _db.Images.RemoveRange(imgs);
            await _db.SaveChangesAsync();
        }
    }
}
