using Microsoft.AspNetCore.Identity;
namespace ES.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
