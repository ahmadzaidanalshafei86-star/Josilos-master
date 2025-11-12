namespace ES.Core.Entities
{
    public class ProductAttributeMapping
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public int ProductAttributeId { get; set; }
        public int ProductAttributeValueId { get; set; }
        public int Order { get; set; } // Ensures attributes are ordered per product

        public Product Product { get; set; } = null!;
        public ProductAttribute ProductAttribute { get; set; } = null!;
        public ProductAttributeValue ProductAttributeValue { get; set; } = null!;

        public decimal? RegularPrice { get; set; } // Extra cost for this attribute
        public decimal? SalePrice { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public string? ImageUrl { get; set; } // Optional image per attribute
        public int? StockQuantity { get; set; } // Available stock for this attribute
        public string? SKU { get; set; }
    }
}
