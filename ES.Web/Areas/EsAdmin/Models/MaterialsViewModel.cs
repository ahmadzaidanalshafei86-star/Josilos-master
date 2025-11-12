

namespace ES.Web.Areas.EsAdmin.Models
{
    public class MaterialsViewModel
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public string Name { get; set; } = null!;
     
        public DateTime CreatedDate { get; set; }
       
    }
}

