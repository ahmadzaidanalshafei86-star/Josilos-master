

using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class MaterialFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string Name { get; set; } = null!;

      
      

        [Required(ErrorMessage = Errors.RequiredField)]
        public TypeOfSorting TypeOfSorting { get; set; }
        public IEnumerable<SelectListItem>? SortingTypes { get; set; }

        public int Order { get; set; } = 0;

      

        public IEnumerable<SelectListItem>? Materials { get; set; }

      
      
    }
}

