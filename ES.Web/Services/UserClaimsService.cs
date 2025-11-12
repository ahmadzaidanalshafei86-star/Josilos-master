using Microsoft.Extensions.Caching.Memory;

namespace ES.Web.Services
{
    public class UserClaimsService : IUserClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public UserClaimsService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMemoryCache cache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _cache = cache;
        }

        public async Task<IList<Claim>> GetUserClaimsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<Claim>();

            var claims = new List<Claim>();

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                // Get role claims
                var roleClaims = await GetRoleClaimsAsync(role);
                claims.AddRange(roleClaims);
            }

            return claims;
        }

        public async Task<IList<Claim>> GetRoleClaimsAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return new List<Claim>();

            return await _roleManager.GetClaimsAsync(role);
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {
            var cacheKey = $"user_permissions_{userId}";

            if (!_cache.TryGetValue(cacheKey, out HashSet<string> userPermissions))
            {
                userPermissions = await GetUserPermissionsAsync(userId);
                _cache.Set(cacheKey, userPermissions, _cacheExpiration);
            }

            return userPermissions.Contains(permission);
        }

        public async Task<HashSet<string>> GetUserPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new HashSet<string>();

            var permissions = new HashSet<string>();
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var claim in roleClaims.Where(c => c.Type == "Permission"))
                    {
                        permissions.Add(claim.Value);
                    }
                }
            }

            return permissions;
        }
    }
}