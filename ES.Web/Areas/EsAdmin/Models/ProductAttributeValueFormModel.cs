namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductAttributeValueFormModel
    {
        public int ValueId { get; set; }
        public string ValueName { get; set; } = null!;

        public bool IsSelected { get; set; }

        public decimal? RegualrPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public string? SKU { get; set; }
        public int? StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public bool KeepImage { get; set; } = true;
    }
}
