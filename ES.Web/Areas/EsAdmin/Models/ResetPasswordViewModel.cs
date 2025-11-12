using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class ResetPasswordViewModel
    {
        public string? Id { get; set; }

        [DataType(DataType.Password),
          StringLength(20, ErrorMessage = Errors.MaxMinLength, MinimumLength = 8)]
        [RegularExpression(RegexPatterns.Password, ErrorMessage = Errors.WeakPassword)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password),
            Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch), Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}

