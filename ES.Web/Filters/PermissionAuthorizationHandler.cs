using ES.Web.Services;

namespace ES.Web.Filters;
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserClaimsService _userClaimsService;

    public PermissionAuthorizationHandler(IUserClaimsService userClaimsService)
    {
        _userClaimsService = userClaimsService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User == null)
            return;

        // Get user ID from claims
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return;

        // Check if user has the required permission by querying the database
        var hasPermission = await _userClaimsService.HasPermissionAsync(userId, requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
            return;
        }
    }
}