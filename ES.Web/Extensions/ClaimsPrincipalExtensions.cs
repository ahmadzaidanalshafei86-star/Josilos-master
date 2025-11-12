using ES.Web.Services;

namespace ES.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Checks if the user has a specific permission by querying the database.
        /// This is an alternative to User.HasClaim() for permission checking.
        /// Use this in your controllers: await User.HasPermissionAsync(permission)
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="permission">The permission to check</param>
        /// <param name="userClaimsService">The UserClaimsService instance</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        public static async Task<bool> HasPermissionAsync(this ClaimsPrincipal user, string permission, IUserClaimsService userClaimsService)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return false;

            return await userClaimsService.HasPermissionAsync(userId, permission);
        }
    }
}
