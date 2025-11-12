namespace ES.Web.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedSuperAdmin(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManger)
        {
            ApplicationUser admin = new()
            {
                Email = "SuperAdmin@ESCMS.com",
                UserName = "SuperAdmin",
                IsActive = true,
                EmailConfirmed = true,
            };

            var user = await userManager.FindByNameAsync(admin.UserName);
            if (user is null)
            {
                await userManager.CreateAsync(admin, "Akm&mk2050ak");
                await userManager.AddToRoleAsync(admin, AppRoles.SuperAdmin);
            }

            await roleManger.SeedClaimsForSuperAdmin();
        }

        private static async Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var superAdminRole = await roleManager.FindByNameAsync(AppRoles.SuperAdmin);
            await roleManager.AddPermissionsClaims(superAdminRole, "Users");
            await roleManager.AddPermissionsClaims(superAdminRole, "Roles");
            await roleManager.AddPermissionsClaims(superAdminRole, "Categories");
            await roleManager.AddPermissionsClaims(superAdminRole, "Pages");
            await roleManager.AddPermissionsClaims(superAdminRole, "MenuManagment");
            await roleManager.AddPermissionsClaims(superAdminRole, "Documents");
            await roleManager.AddPermissionsClaims(superAdminRole, "MediaManagment");
            await roleManager.AddPermissionsClaims(superAdminRole, "Forms");
            await roleManager.AddPermissionsClaims(superAdminRole, "ProductCategories");
            await roleManager.AddPermissionsClaims(superAdminRole, "Brands");
            await roleManager.AddPermissionsClaims(superAdminRole, "ProductAttributes");
            await roleManager.AddPermissionsClaims(superAdminRole, "Products");
            await roleManager.AddPermissionsClaims(superAdminRole, "ProductLabels");
            await roleManager.AddPermissionsClaims(superAdminRole, "Careers");
            await roleManager.AddPermissionsClaims(superAdminRole, "SocialMediaLinks");
            await roleManager.AddPermissionsClaims(superAdminRole, "Materials");
            await roleManager.AddPermissionsClaims(superAdminRole, "Tenders");
            //TODO : when adding any module seed the cliams for this module to the super admin Role

            //await roleManager.AddPermissionsClaims(superAdminRole, "Pages");
        }

        public static async Task AddPermissionsClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermissionsList(module);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }

        }
    }
}
