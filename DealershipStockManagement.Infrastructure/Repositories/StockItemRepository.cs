using DealershipStockManagement.Domain.Entities;
using DealershipStockManagement.Domain.Interfaces;
using DealershipStockManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DealershipStockManagement.Infrastructure.Repositories
{
    public class StockItemRepository : IStockItemRepository
    {
        private readonly DatabaseContext _db;
        public StockItemRepository(DatabaseContext db) => _db = db;

        public async Task<(IReadOnlyList<StockItem> Items, int Total)> GetPagedAsync(
            int page, int pageSize, string? search, string? sortBy, string? sortDir)
        {

            var q = _db.StockItems.Include(s => s.Images).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(s => s.Make.Contains(search)
                || s.Model.Contains(search)
                || (s.ModelYear.ToString() ?? "").Contains(search)
                || (s.RetailPrice.ToString() ?? "").Contains(search)
                || (s.VIN ?? "").Contains(search)
                || (s.RegNo ?? "").Contains(search));

            (string sb, string sd) key = ((sortBy ?? "DTUpdated").ToLower(), (sortDir ?? "desc").ToLower());
            q = key switch
            {
                ("make", "asc") => q.OrderBy(s => s.Make),
                ("make", "desc") => q.OrderByDescending(s => s.Make),
                ("modelyear", "asc") => q.OrderBy(s => s.ModelYear),
                ("modelyear", "desc") => q.OrderByDescending(s => s.ModelYear),
                ("retailprice", "asc") => q.OrderBy(s => s.RetailPrice),
                ("retailprice", "desc") => q.OrderByDescending(s => s.RetailPrice),
                ("vin", "asc") => q.OrderBy(s => s.VIN),
                ("vin", "desc") => q.OrderByDescending(s => s.VIN),
                ("model", "asc") => q.OrderBy(s => s.Model),
                ("model", "desc") => q.OrderByDescending(s => s.Model),
                ("regNo", "asc") => q.OrderBy(s => s.RegNo),
                ("regNo", "desc") => q.OrderByDescending(s => s.RegNo),
                _ => key.sd == "asc" ? q.OrderBy(s => s.DTUpdated) : q.OrderByDescending(s => s.DTUpdated)
            };

            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public Task<StockItem?> GetByIdAsync(int id) =>
            _db.StockItems
                .Include(s => s.Accessories)
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task AddAsync(StockItem stockItem)
        {
            _db.StockItems.Add(stockItem);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockItem stockItem)
        {
            _db.StockItems.Update(stockItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.StockItems.FindAsync(id);
            if (entity is null) return;
            _db.StockItems.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
