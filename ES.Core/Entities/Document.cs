namespace ES.Core.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string OriginalName { get; set; } = null!;
        public string GenratedName { get; set; } = null!;
        public string FullUrl { get; set; } = null!;
        public DateTime UploadedOn { get; set; }

    }
}
