namespace ES.Web.Seeds
{
    public class DefaultRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
                await roleManager.CreateAsync(new IdentityRole(AppRoles.SuperAdmin));// SuperAdmin For Full Access
        }
    }
}
