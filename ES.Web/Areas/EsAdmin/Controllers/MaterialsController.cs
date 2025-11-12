


using ES.Core.Entities;
using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class MaterialsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IImageService _imageService;
        private readonly SlugService _slugService;
        private readonly ILanguageService _languageService;
        private readonly RowPermission _rowPermission;
        private readonly MaterialsRepository _materialsRepository;
        private readonly LanguagesRepository _languagesRepository;
        private readonly GalleryImagesRepository _galleryImagesRepository;
        private readonly MenuItemsRepository _menuItemsRepository;

        public MaterialsController(LanguagesRepository languagesRepository,
            SlugService slugService,
            MaterialsRepository materialsRepository,
            GalleryImagesRepository galleryImagesRepository,
            IImageService imageService,
            ILanguageService languageService,
            RowPermission rowPermission,
            RoleManager<IdentityRole> roleManager,
            MenuItemsRepository menuItemsRepository)
        {
            _galleryImagesRepository = galleryImagesRepository;
            _languagesRepository = languagesRepository;
            _imageService = imageService;
            _slugService = slugService;
            _languageService = languageService;
            _materialsRepository = materialsRepository;
            _rowPermission = rowPermission;
            _roleManager = roleManager;
            _menuItemsRepository = menuItemsRepository;
        }

        [Authorize(Permissions.Materials.Read)]
        public async Task<IActionResult> Index()
        {
            var materials = await _materialsRepository.GetMaterialsWithParentInfoAsync();
            return View(materials);
        }

        [HttpGet]
        [Authorize(Permissions.Materials.Create)]
        public async Task<IActionResult> Create()
        {
            var model = await _materialsRepository.InitializematerialFormViewModelAsync();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Materials.Create)]
        public async Task<IActionResult> Create(MaterialFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _materialsRepository.InitializematerialFormViewModelAsync();
                return View("Form", model);
            }

            Material material = new()
            {
                Name = model.Name,
                Slug = _slugService.GenerateUniqueSlug(model.Name, nameof(Material)),
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync())

            };
           
            var materialId = await _materialsRepository.AddmaterialAsync(material);

          
            return Json(new { success = true, id = materialId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var material = await _materialsRepository.GetmaterialWithAllDataAsync(id);

            if (material is null)
                return NotFound();

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                           .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.Materials,
                                           material.Id,
                                           CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");


            var model = new MaterialFormViewModel
            {
                Id = material.Id,
                Name = material.Name
            };

            model = await _materialsRepository.InitializematerialFormViewModelAsync(model);

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, MaterialFormViewModel model)
        {
            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();

            var RowLevelPermission = await _rowPermission
                                            .HasRowLevelPermissionAsync(role.Id,
                                            TablesNames.Materials,
                                            id,
                                            CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            if (!ModelState.IsValid)
            {
                model = await _materialsRepository.InitializematerialFormViewModelAsync();
                return View("Form", model);
            }

            var material = await _materialsRepository.GetmaterialWithAllDataAsync(id);
            if (material == null)
                return NotFound();


         

          

        
          

            if (material.Name != model.Name)
                material.Slug = _slugService.GenerateUniqueSlug(model.Name, nameof(Material), material.Id);

            material.Name = model.Name;

            // Remove old Related materials and Add the new 
          
            _materialsRepository.Updatematerial(material);

            return Json(new { success = true, id = material.Id });

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var material = await _materialsRepository.GetMaterialByIdAsync(id);

            if (material is null)
                return NotFound();

            // Prevent Deleting "Home Slider"
            if (material.Name == "Home Slider")
                return BadRequest("This material cannot be deleted.");

            //Check Authroization to delete this material
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                .HasRowLevelPermissionAsync(role.Id, TablesNames.Materials, id, CrudOperations.Delete);
            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);

            //Remove all row level permissions of this material
            var existingmaterialRowPermissions = await _rowPermission
                .GetRowLevelPermissionsToDeleteAsync(id, TablesNames.Materials);
            _rowPermission.RemoveRange(existingmaterialRowPermissions);

            //Remove all row level permissions of related pages
            var existingPageRowPermissions = await _rowPermission
                .GetRowLevelPermissionsToDeleteAsync(id, TablesNames.PagesOfMaterial);
            _rowPermission.RemoveRange(existingPageRowPermissions);

            //if (_materialsRepository.IsParentmaterial(id) || _materialsRepository.IsRelatedToAnothermaterial(id))
            //    return StatusCode(400);

          
        

          

            var result = await _materialsRepository.DeletematerialAsync(id); //returns true if deleted successfully
            if (result)
                return StatusCode(200);

            return BadRequest();

        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            //Check Authroization to update this material 
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                          TablesNames.Materials,
                                          id,
                                          CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);

            var material = await _materialsRepository.GetMaterialByIdAsync(id);

            if (material is null)
                return NotFound();

           

            _materialsRepository.Updatematerial(material);

            return StatusCode(200);
        }
    }
}

