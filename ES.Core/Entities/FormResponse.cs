
namespace ES.Core.Entities
{
    public class FormResponse
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        //Relations
        public Form Form { get; set; } = null!;
        public int FormId { get; set; }
        public ICollection<FormResponseDetail> ResponseDetails { get; set; } = new List<FormResponseDetail>();
    }
}
