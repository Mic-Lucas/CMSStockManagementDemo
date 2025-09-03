using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealershipStockManagement.Application.Dtos
{
    public class StockItemListDto
    {
        public int Id { get; set; }
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string VIN { get; set; } = null!;
        public string RegNo { get; set; } = null!;
        public string Colour { get; set; } = null!;
        public int? ModelYear { get; set; }
        public decimal? RetailPrice { get; set; }
        public string? PrimaryImageUrl { get; set; }
    }
}
