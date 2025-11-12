namespace ES.Core.Entities
{
   public class SilosDeclerations
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;


        // Arabic
        public string SubjectAr { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string NotesAr { get; set; } = string.Empty;
    }
}
