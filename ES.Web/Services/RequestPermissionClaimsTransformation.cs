using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ES.Web.Services
{
    /// <summary>
    /// Adds permission claims to the current principal for the duration of the request only.
    /// This prevents storing all permissions in the authentication cookie while allowing
    /// existing code that checks User.HasClaim("Permission", ...) to keep working.
    /// Permission lookups are cached inside IUserClaimsService to avoid repeated DB calls.
    /// </summary>
    public class RequestPermissionClaimsTransformation : IClaimsTransformation
    {
        private readonly IUserClaimsService _userClaimsService;

        public RequestPermissionClaimsTransformation(IUserClaimsService userClaimsService)
        {
            _userClaimsService = userClaimsService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity?.IsAuthenticated == true)
                return principal;

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return principal;

            var permissions = await _userClaimsService.GetUserPermissionsAsync(userId);

            if (permissions == null || permissions.Count == 0)
                return principal;

            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null)
                return principal;

            // Avoid duplicating permission claims
            var existing = new HashSet<string>(identity.Claims.Where(c => c.Type == "Permission").Select(c => c.Value));
            foreach (var p in permissions)
            {
                if (!existing.Contains(p))
                {
                    identity.AddClaim(new Claim("Permission", p));
                }
            }

            return principal;
        }
    }
}
