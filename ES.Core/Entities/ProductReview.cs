namespace ES.Core.Entities
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int FormResponseId { get; set; }
        public FormResponse FormResponse { get; set; } = null!;
        public bool Published { get; set; }
    }
}
