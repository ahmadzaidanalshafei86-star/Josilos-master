using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Models
{
    public class CustomerRegistrationViewModel
    {
        [Required(ErrorMessage = "User Type is required")]
        public int UserType { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [MaxLength(300)]
        public string CompanyName { get; set; }

        public int CompanySector { get; set; }

        public string RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        public string Fax { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string Website { get; set; }

        public string OtherMaterials { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Commercial Register file is required")]
        public IFormFile CommercialRegisterFile { get; set; }

        public IFormFile RegistrationCertificateFile { get; set; }
        public IFormFile ProfessionsLicenseFile { get; set; }
        public IFormFile ClientListFile { get; set; }
        public IFormFile AchievementsFile { get; set; }

        public List<MaterialItem> AvailableMaterials { get; set; } = new();
        public List<int> SelectedMaterials { get; set; } = new();
    }

    public class MaterialItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
