using Microsoft.Extensions.Options;

namespace ES.Web.Services
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly IUserClaimsService _userClaimsService;

        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IUserClaimsService userClaimsService)
            : base(userManager, roleManager, optionsAccessor)
        {
            _userClaimsService = userClaimsService;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Remove any permission claims if they exist to avoid persisting them into the auth cookie
            // (permissions will be added per-request via IClaimsTransformation instead)
            var permissionClaims = identity.Claims
                .Where(c => c.Type == "Permission")
                .ToList();

            foreach (var claim in permissionClaims)
            {
                identity.RemoveClaim(claim);
            }

            return identity;
        }
    }
}