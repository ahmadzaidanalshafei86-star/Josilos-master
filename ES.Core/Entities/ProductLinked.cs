namespace ES.Core.Entities
{
    public class ProductLinked
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int LinkedProductId { get; set; }
        public Product LinkedProduct { get; set; } = null!;
    }
}
