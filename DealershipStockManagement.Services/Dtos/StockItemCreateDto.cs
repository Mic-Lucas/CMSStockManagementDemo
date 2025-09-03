using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealershipStockManagement.Application.Dtos
{
    public class StockItemCreateDto
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int? ModelYear { get; set; }
        public int? KMS { get; set; }
        public string? Colour { get; set; }
        public string? VIN { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string? RegNo { get; set; }

        // up to 3 files in form upload
        public IFormFile[]? Images { get; set; }
        public string? AccessoriesJson { get; set; } // JSON array of accessory DTOs
    }
}
