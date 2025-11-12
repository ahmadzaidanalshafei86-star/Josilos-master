namespace ES.Web.Models
{
    public class SearchResultsViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
    }
}
