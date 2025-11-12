using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductLabelViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField), MaxLength(30, ErrorMessage = Errors.MaxLength)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

    }
}
