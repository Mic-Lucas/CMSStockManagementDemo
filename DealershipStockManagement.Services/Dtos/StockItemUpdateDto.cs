using Microsoft.AspNetCore.Http;

namespace DealershipStockManagement.Application.Dtos
{
    public class StockItemUpdateDto
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? ModelYear { get; set; }
        public int? KMS { get; set; }
        public string? Colour { get; set; }
        public string? VIN { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string? RegNo { get; set; }

        public IFormFile[]? Images { get; set; }
        public string? DeleteImageIdsCsv { get; set; }
        public string? AccessoriesJson { get; set; }
    }
}
