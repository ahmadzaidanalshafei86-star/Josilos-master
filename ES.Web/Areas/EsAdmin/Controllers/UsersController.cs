using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Permissions.Users.Read)]
        public async Task<IActionResult> Index()
        {
            var usersList = await _userManager.Users
                .Where(u => u.UserName != "SuperAdmin")
                .ToListAsync();
            var users = new List<UsersViewModel>();

            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                users.Add(new UsersViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    Roles = roles
                });
            }
            return View(users);
        }


        [HttpGet]
        [Authorize(Permissions.Users.Create)]
        public async Task<IActionResult> Create()
        {

            UserFormViewModel model = new()
            {
                Roles = await GetRolesAsync()
            };

            return View("Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Users.Create)]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Roles = await GetRolesAsync();
                return View("Form", model);
            }

            // Create the new user
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                CreatedDate = DateTime.Now,
                EmailConfirmed = true,
                IsActive = true,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // Reload roles if user creation fails
                model.Roles = await GetRolesAsync();
                return View("Form", model);
            }
            await _userManager.AddToRoleAsync(user, model.Role);

            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        [Authorize(Permissions.Users.Update)]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            ResetPasswordViewModel viewModel = new()
            {
                Id = id,
            };

            return View("ResetPassword", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Users.Update)]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(viewModel.Id);

            if (user is null)
                return NotFound();

            var currentPasswordHash = user.PasswordHash;

            await _userManager.RemovePasswordAsync(user);

            var result = await _userManager.AddPasswordAsync(user, viewModel.Password);

            if (result.Succeeded)
            {
                //user.UpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                //user.UpdatedAt = DateTime.Now;

                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }
            user.PasswordHash = currentPasswordHash;
            await _userManager.UpdateAsync(user);

            return BadRequest(string.Join(",", result.Errors.Select(e => e.Description)));
        }

        [HttpGet]
        [Authorize(Permissions.Users.Update)]
        public async Task<IActionResult> Edit(string id)
        {

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            UserFormViewModel model = new()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault()!,
                Roles = await GetRolesAsync()
            };


            return View("form", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Users.Update)]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = await GetRolesAsync();
                return View("Form", model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            var CurrentRole = await _userManager.GetRolesAsync(user);

            if (CurrentRole.FirstOrDefault() != null)
            {
                await _userManager.RemoveFromRoleAsync(user, CurrentRole.FirstOrDefault());
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!User.HasClaim("Permission", Permissions.Users.Delete))
                return StatusCode(403);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.Id == User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
                return StatusCode(400);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return StatusCode(200);

            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> BulkToggleStatus([FromBody] List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return BadRequest("No users selected.");

            if (!User.HasClaim("Permission", Permissions.Users.Update))
                return StatusCode(403);

            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsActive = false; // Set to active
                    await _userManager.UpdateAsync(user); // Save changes
                }
            }

            return StatusCode(200);
        }


        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {

            if (!User.HasClaim("Permission", Permissions.Users.Update))
                return StatusCode(403);

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);

            if (!user.IsActive)
                await _userManager.UpdateSecurityStampAsync(user);// to log out the user the admin deleted the user 

            return StatusCode(200);
        }



        public async Task<IActionResult> AllowUserName(UserFormViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var isAllowed = user is null || user.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        private async Task<IEnumerable<SelectListItem>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles
                .Where(r => r.Name != "SuperAdmin")
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                })
                .ToListAsync();
            return roles;
        }

    }
}
