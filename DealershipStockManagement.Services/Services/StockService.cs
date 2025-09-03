using DealershipStockManagement.Application.Dtos;
using DealershipStockManagement.Application.Services;
using DealershipStockManagement.Domain.Entities;
using DealershipStockManagement.Domain.Interfaces;
using System;
using System.Text.Json;

namespace DealershipStockManagement.Application.Services;

public class StockService : IStockService
{
    private readonly IStockItemRepository _stockRepo;
    private readonly IImageRepository _imageRepo;

    public StockService(IStockItemRepository stockRepo, IImageRepository imageRepo)
    {
        _stockRepo = stockRepo;
        _imageRepo = imageRepo;
    }

    public async Task<object> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, string? sortDir, Func<int, string?> imageUrlFactory)
    {
        var (items, total) = await _stockRepo.GetPagedAsync(page, pageSize, search, sortBy, sortDir);
        var list = items.Select(i =>
        {
            var p = i.Images.FirstOrDefault(x => x.IsPrimary);
            return new StockItemListDto
            {
                Id = i.Id,
                Make = i.Make,
                Model = i.Model,
                ModelYear = i.ModelYear,
                RetailPrice = i.RetailPrice,
                VIN = i.VIN ?? "",
                RegNo = i.RegNo ?? "", 
                Colour = i.Colour ?? "",
                PrimaryImageUrl = p != null ? imageUrlFactory(p.Id) : null
            };
        }).ToList();

        return new { total, page, pageSize, data = list };
    }

    public async Task<object?> GetByIdAsync(int id, Func<int, string?> imageUrlFactory)
    {
        var item = await _stockRepo.GetByIdAsync(id);
        if (item is null) return null;

        return new
        {
            item.Id,
            item.RegNo,
            item.Make,
            item.Model,
            item.ModelYear,
            item.KMS,
            item.Colour,
            item.VIN,
            item.RetailPrice,
            item.CostPrice,
            item.DTCreated,
            item.DTUpdated,
            Accessories = item.Accessories.Select(a => new { a.Id, a.Name, a.Description }),
            Images = item.Images.Select(img => new
            {
                img.Id,
                img.Name,
                img.IsPrimary,
                Url = imageUrlFactory(img.Id)
            })
        };
    }

    public async Task<int> CreateAsync(StockItemCreateDto dto)
    {
        if (dto.Images is { Length: > 3 }) throw new InvalidOperationException("Maximum 3 images allowed.");

        var stock = new StockItem
        {
            RegNo = dto.RegNo,
            Make = dto.Make,
            Model = dto.Model,
            ModelYear = dto.ModelYear,
            KMS = dto.KMS,
            Colour = dto.Colour,
            VIN = dto.VIN,
            RetailPrice = dto.RetailPrice,
            CostPrice = dto.CostPrice,
            DTCreated = DateTime.UtcNow,
            DTUpdated = DateTime.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(dto.AccessoriesJson))
        {
            var accessories = JsonSerializer.Deserialize<List<AccessoryDto>>(dto.AccessoriesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (accessories != null)
            {
                foreach (var a in accessories)
                    stock.Accessories.Add(new StockAccessory { Name = a.Name, Description = a.Description });
            }
        }

        await _stockRepo.AddAsync(stock);

        if (dto.Images != null && dto.Images.Any())
        {
            int idx = 0;
            var imgs = new List<Image>();
            foreach (var f in dto.Images.Take(3))
            {
                using var ms = new MemoryStream();
                await f.CopyToAsync(ms);
                imgs.Add(new Image
                {
                    StockItemId = stock.Id,
                    Name = f.FileName,
                    ImageBinary = ms.ToArray(),
                    IsPrimary = idx == 0
                });
                idx++;
            }
            await _imageRepo.AddImagesAsync(imgs);
        }

        return stock.Id;
    }

    public async Task UpdateAsync(int id, StockItemUpdateDto dto)
    {
        var stock = await _stockRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("StockItem not found.");

        stock.RegNo = dto.RegNo ?? stock.RegNo;
        stock.Make = dto.Make ?? stock.Make;
        stock.Model = dto.Model ?? stock.Model;
        stock.ModelYear = dto.ModelYear ?? stock.ModelYear;
        stock.KMS = dto.KMS ?? stock.KMS;
        stock.Colour = dto.Colour ?? stock.Colour;
        stock.VIN = dto.VIN ?? stock.VIN;
        stock.RetailPrice = dto.RetailPrice ?? stock.RetailPrice;
        stock.CostPrice = dto.CostPrice ?? stock.CostPrice;
        stock.DTUpdated = DateTime.UtcNow;

        // Clear accessories
        stock.Accessories.Clear();

        // Handle new images
        if (dto.Images != null && dto.Images.Any())
        {
            // Check if stock already has a primary image
            bool hasPrimary = stock.Images != null && stock.Images.Any(i => i.IsPrimary);

            var imgs = new List<Image>();
            foreach (var file in dto.Images.Take(3))
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                imgs.Add(new Image { StockItemId = stock.Id, Name = file.FileName, ImageBinary = ms.ToArray(), IsPrimary = hasPrimary && Array.IndexOf(dto.Images, file) == 0 });
            }
            await _imageRepo.AddImagesAsync(imgs);
        }

        if (!string.IsNullOrWhiteSpace(dto.DeleteImageIdsCsv))
        {
            var ids = dto.DeleteImageIdsCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, out var n) ? (int?)n : null)
                .Where(x => x.HasValue).Select(x => x!.Value).ToList();

            if (ids.Count > 0) await _imageRepo.DeleteImagesAsync(ids);

        }

        // After adding/deleting images, ensure there is a primary
        var allImages = await _imageRepo.GetImagesAsync(stock.Id);
        var primaryMissing = allImages.FirstOrDefault(i => i.IsPrimary) is null;
        if (primaryMissing && allImages.Any())
        {
            var primaryImage = allImages.First();
            primaryImage.IsPrimary = true;
            await _imageRepo.UpdateImageAsync(primaryImage);
        }

        if (!string.IsNullOrWhiteSpace(dto.AccessoriesJson))
        {
            var accessories = JsonSerializer.Deserialize<List<AccessoryDto>>(dto.AccessoriesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (accessories != null)
            {
                foreach (var a in accessories)
                    stock.Accessories.Add(new StockAccessory { Name = a.Name, Description = a.Description });
            }
        }

        await _stockRepo.UpdateAsync(stock);
    }

    public Task DeleteAsync(int id) => _stockRepo.DeleteAsync(id);

    public async Task<(byte[] Data, string FileName, string MimeType)?> GetImageAsync(int imageId)
    {
        var img = await _imageRepo.GetImageAsync(imageId);
        if (img is null) return null;
        var mime = GetImageMime(img.ImageBinary);
        return (img.ImageBinary, img.Name ?? "image", mime);
    }

    private static string GetImageMime(byte[] b)
    {
        if (b.Length > 2 && b[0] == 0xFF && b[1] == 0xD8) return "image/jpeg";
        if (b.Length > 8 && b[0] == 137 && b[1] == 80 && b[2] == 78 && b[3] == 71) return "image/png";
        return "application/octet-stream";
    }
}