namespace ES.Web.Services
{
    public interface IUserClaimsService
    {
        Task<IList<Claim>> GetUserClaimsAsync(string userId);
        Task<IList<Claim>> GetRoleClaimsAsync(string roleId);
        Task<bool> HasPermissionAsync(string userId, string permission);
        Task<HashSet<string>> GetUserPermissionsAsync(string userId);
    }
}