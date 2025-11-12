namespace ES.Core.Entities
{
    public class PageCategory
    {
        public int PageId { get; set; }
        public Page Page { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
