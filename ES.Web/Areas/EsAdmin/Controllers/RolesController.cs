using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RowPermission _rowPermission;
        private readonly CategoriesRepository _categoriesRepository;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, CategoriesRepository categoriesRepository, RowPermission rowPermission)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _categoriesRepository = categoriesRepository;
            _rowPermission = rowPermission;
        }

        [Authorize(Policy = Permissions.Roles.Read)]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .Where(r => r.Name != "SuperAdmin").ToListAsync();

            var rolesViewModel = roles.Select(role => new RolesViewModel
            {
                Id = role.Id,
                Name = role.Name,
            }).ToList();

            return View(rolesViewModel);
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Roles.Create)]
        public IActionResult Create(string? returnUrl, string? id)
        {
            if (returnUrl is not null)
                TempData["returnUrl"] = "returnUrl";

            if (id is not null)
                TempData["id"] = id;

            return View("Form");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Roles.Create)]
        public async Task<IActionResult> Create(RoleFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            if (await _roleManager.RoleExistsAsync(model.RoleName))
            {
                ModelState.AddModelError("RoleName", "Role Name already exists");
                return View("Form", model);
            }

            var role = new IdentityRole(model.RoleName.Trim());

            await _roleManager.CreateAsync(role);

            return RedirectToAction(nameof(ManagePermissions), new { id = role.Id });
        }

        [HttpGet]
        [Authorize(Policy = Permissions.Roles.Update)]
        public async Task<IActionResult> ManagePermissions(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
                return NotFound();

            //Handle row level authorization
            var existingCategoryRowPermissions = await _rowPermission.GetRowLevelPermissionsOfRoleAsync(id, TablesNames.Categories);
            var existingPagesRowPermissions = await _rowPermission.GetRowLevelPermissionsOfRoleAsync(id, TablesNames.PagesOfCategory);
            //Handle permission based authorization 
            var roleClaims = _roleManager.GetClaimsAsync(role).Result.Select(c => c.Value).ToList();
            var allClaims = Permissions.GenerateAllPermissions();
            var allPermissions = allClaims.Select(p => new CheckBoxViewModel { DisplayValue = p }).ToList();

            foreach (var permission in allPermissions)
            {
                if (roleClaims.Any(c => c == permission.DisplayValue))
                    permission.IsSelected = true;
            }

            var viewModel = new PermissionsFormViewModel
            {
                RoleId = id,
                RoleName = role.Name,
                RoleClaims = allPermissions,
                Categories = await _categoriesRepository.GetCategoriesNamesAsync(),
                CategoriesPermissions = new CategoryPermissionViewModel
                {
                    CanUpdate = existingCategoryRowPermissions.Any(p => p.PermissionType == CrudOperations.Update),
                    CanDelete = existingCategoryRowPermissions.Any(p => p.PermissionType == CrudOperations.Delete),
                    CategoryIds = existingCategoryRowPermissions.Select(p => p.RowId).ToList()
                },
                PagesPermissions = new CategoryPermissionViewModel
                {
                    CanUpdate = existingPagesRowPermissions.Any(p => p.PermissionType == CrudOperations.Update),
                    CanDelete = existingPagesRowPermissions.Any(p => p.PermissionType == CrudOperations.Delete),
                    CategoryIds = existingPagesRowPermissions.Select(p => p.RowId).ToList()
                }
            };
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Permissions.Roles.Update)]
        public async Task<IActionResult> ManagePermissions(PermissionsFormViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            if (role == null)
                return NotFound();

            //Handle row level authorization
            var existingCatefgoryRowPermissions = await _rowPermission.GetRowLevelPermissionsOfRoleAsync(model.RoleId, TablesNames.Categories);
            _rowPermission.RemoveRange(existingCatefgoryRowPermissions);

            var CategoryRowLevelPermissions = new List<RowLevelPermission>();

            if (model.CategoriesPermissions?.CategoryIds != null)
            {
                foreach (var categoryId in model.CategoriesPermissions.CategoryIds)
                {
                    if (model.CategoriesPermissions.CanUpdate)
                    {
                        CategoryRowLevelPermissions.Add(new RowLevelPermission
                        {
                            RoleId = model.RoleId,
                            TableName = TablesNames.Categories,
                            RowId = categoryId,
                            PermissionType = CrudOperations.Update
                        });
                    }

                    if (model.CategoriesPermissions.CanDelete)
                    {
                        CategoryRowLevelPermissions.Add(new RowLevelPermission
                        {
                            RoleId = model.RoleId,
                            TableName = TablesNames.Categories,
                            RowId = categoryId,
                            PermissionType = CrudOperations.Delete
                        });
                    }
                }
            }

            // Add new category row-level permissions if any exist
            if (CategoryRowLevelPermissions.Any())
                _rowPermission.AddRowLevelPermissionsRange(CategoryRowLevelPermissions);

            //Handle row level authorization
            var existingPagesRowPermissions = await _rowPermission.GetRowLevelPermissionsOfRoleAsync(model.RoleId, TablesNames.PagesOfCategory);
            _rowPermission.RemoveRange(existingPagesRowPermissions);

            var PageRowLevelPermissions = new List<RowLevelPermission>();

            if (model.PagesPermissions?.CategoryIds != null)
            {
                foreach (var categoryId in model.PagesPermissions.CategoryIds)
                {
                    if (model.PagesPermissions.CanUpdate)
                    {
                        PageRowLevelPermissions.Add(new RowLevelPermission
                        {
                            RoleId = model.RoleId,
                            TableName = TablesNames.PagesOfCategory,
                            RowId = categoryId,
                            PermissionType = CrudOperations.Update
                        });
                    }

                    if (model.PagesPermissions.CanDelete)
                    {
                        PageRowLevelPermissions.Add(new RowLevelPermission
                        {
                            RoleId = model.RoleId,
                            TableName = TablesNames.PagesOfCategory,
                            RowId = categoryId,
                            PermissionType = CrudOperations.Delete
                        });
                    }
                }
            }
            // Add new page row-level permissions if any exist
            if (PageRowLevelPermissions.Any())
                _rowPermission.AddRowLevelPermissionsRange(PageRowLevelPermissions);


            //Handle permission based authorization 
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var claim in roleClaims)
                await _roleManager.RemoveClaimAsync(role, claim);

            var selectedClaims = model.RoleClaims.Where(c => c.IsSelected).ToList();

            foreach (var claim in selectedClaims)
                await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.DisplayValue));



            if (TempData.ContainsKey("id") && TempData.ContainsKey("returnUrl"))
            {
                var id = TempData["id"]!.ToString();
                TempData.Remove("id");
                TempData.Remove("returnUrl");
                return RedirectToAction("Edit", "Users", new { id = id });
            }

            if (TempData.ContainsKey("returnUrl"))
            {
                TempData.Remove("returnUrl");
                return RedirectToAction("Create", "Users");
            }


            // Redirect back to the Create/Edit User form
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!User.HasClaim("Permission", Permissions.Roles.Delete))
                return StatusCode(403);

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var IsUsersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            if (IsUsersInRole.Any())
                return StatusCode(400);

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return StatusCode(200);

            return BadRequest(string.Join(",", result.Errors.Select(e => e.Description)));
        }
    }
}
