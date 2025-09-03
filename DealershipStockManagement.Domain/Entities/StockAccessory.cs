using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealershipStockManagement.Domain.Entities
{
    public class StockAccessory
    {
        public int Id { get; set; }
        public int StockItemId { get; set; }
        public StockItem StockItem { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
