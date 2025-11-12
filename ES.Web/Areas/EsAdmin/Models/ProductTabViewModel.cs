using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductTabViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public int Order { get; set; } = 0;

    }
}
