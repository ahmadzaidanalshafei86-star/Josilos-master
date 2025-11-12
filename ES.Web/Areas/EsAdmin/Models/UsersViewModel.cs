namespace ES.Web.Areas.EsAdmin.Models
{
    public class UsersViewModel
    {
        public string Id { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public IEnumerable<string> Roles { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
