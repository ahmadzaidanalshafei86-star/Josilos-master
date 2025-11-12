using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }

        [MaxLength(20, ErrorMessage = Errors.MaxLength), Display(Name = "Username"),
            RegularExpression(RegexPatterns.Username, ErrorMessage = Errors.InvalidUsername)]
        [Remote("AllowUserName", null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
        public string UserName { get; set; } = null!;

        [MaxLength(200, ErrorMessage = Errors.MaxLength), EmailAddress]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password),
            StringLength(100, ErrorMessage = Errors.MaxMinLength, MinimumLength = 8),
            RegularExpression(RegexPatterns.Password, ErrorMessage = Errors.WeakPassword)]
        [RequiredIf("Id == null", ErrorMessage = Errors.RequiredField)]
        public string? Password { get; set; } = null!;

        [DataType(DataType.Password), Display(Name = "Confirm password"),
            Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch)]
        [RequiredIf("Id == null", ErrorMessage = Errors.RequiredField)]
        public string? ConfirmPassword { get; set; } = null!;

        [Display(Name = "Role")]
        public string Role { get; set; } = null!;

        public IEnumerable<SelectListItem>? Roles { get; set; }



    }
}
