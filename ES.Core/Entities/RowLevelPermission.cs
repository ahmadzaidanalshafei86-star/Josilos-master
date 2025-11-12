using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class RowLevelPermission
    {
        public int Id { get; set; }
        [MaxLength(450)]
        public string RoleId { get; set; } = null!;
        public IdentityRole Role { get; set; } = null!;
        [MaxLength(255)]
        public string TableName { get; set; } = null!;
        public int RowId { get; set; }
        [MaxLength(50)]
        public string PermissionType { get; set; } = null!;
    }
}
