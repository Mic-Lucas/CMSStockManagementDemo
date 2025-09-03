using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DealershipStockManagement.Domain.Entities
{
    public class StockItem
    {
        public int Id { get; set; }
        public string? RegNo { get; set; }
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int? ModelYear { get; set; }
        public int? KMS { get; set; }
        public string? Colour { get; set; }
        public string? VIN { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public DateTime DTCreated { get; set; } = DateTime.UtcNow;
        public DateTime DTUpdated { get; set; } = DateTime.UtcNow;

        public ICollection<StockAccessory> Accessories { get; set; } = new List<StockAccessory>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
