namespace ES.Core.SpViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public string Name { get; set; } = null!;
        public string? ParentCategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPublished { get; set; }

    }
}
