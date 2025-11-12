namespace ES.Core.Entities
{
    public class CareerApplication
    {
        public int Id { get; set; }
        public int CareerId { get; set; }
        public Career Career { get; set; } = null!;
        public int FormResponseId { get; set; }
        public FormResponse FormResponse { get; set; } = null!;
        public bool IsReviewed { get; set; } = false;
        public bool IsArchived { get; set; } = false;
    }
}
