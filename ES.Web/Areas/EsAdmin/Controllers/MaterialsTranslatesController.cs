


using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class MaterialsTranslatesController : Controller
    {
        private readonly MaterialsRepository _materialsRepository;
        private readonly MaterialsTranslatesRepository _materialsTranslatesRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RowPermission _rowPermission;
        public MaterialsTranslatesController(MaterialsRepository materialsRepository,
            MaterialsTranslatesRepository materialsTranslatesRepository,
            RowPermission rowPermission,
            RoleManager<IdentityRole> roleManager)
        {
            _materialsRepository = materialsRepository;
            _materialsTranslatesRepository = materialsTranslatesRepository;
            _rowPermission = rowPermission;
            _roleManager = roleManager;
        }

        [Authorize(Permissions.Materials.Read)]
        public async Task<IActionResult> Index(int materialId)
        {
            var material = await _materialsRepository.GetMaterialByIdWithTranslationsAsync(materialId);

            MaterialTranslatesViewModel model = new()
            {
                MaterialId = materialId,
                MaterialName = material.Name,
                MaterialDefaultLang = material.Language.Code,
                PreEnteredTranslations = material.MaterialsTranslates?.ToList(),
            };
            return View(model);
        }


        [HttpGet]
        [Authorize(Permissions.Materials.Create)]
        public async Task<IActionResult> Create(int materialId)
        {
            var model = await _materialsTranslatesRepository.InitializeMaterialTranslatesFormViewModelAsync(materialId);
            model.MaterialId = materialId;

            var material = await _materialsRepository.GetMaterialByIdAsync(materialId);

            model.Name = material.Name;
            

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Materials.Create)]
        public async Task<IActionResult> Create(MaterialTranslationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _materialsTranslatesRepository.InitializeMaterialTranslatesFormViewModelAsync(model.MaterialId);
                return View("Form", model);
            }

            MaterialTranslate MaterialTranslate = new()
            {
                Name = model.Name,
                LanguageId = (int)model.LanguageId,
                MaterialId = model.MaterialId,
            };

            await _materialsTranslatesRepository.AddMaterialTranslateAsync(MaterialTranslate);

            return RedirectToAction("Index", new { materialId = model.MaterialId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int materialId, int Translateid)
        {
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();

            var RowLevelPermission = await _rowPermission
                                            .HasRowLevelPermissionAsync(role.Id,
                                            TablesNames.Materials,
                                           materialId,
                                            CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            var translate = await _materialsTranslatesRepository.GetMaterialTranslateByIdAsync(Translateid);

            MaterialTranslationFormViewModel model = new()
            {
                TranslationId = Translateid,
                Name = translate.Name,
            };

            model = await _materialsTranslatesRepository.InitializeMaterialTranslatesFormViewModelAsync(materialId, model);
            model.MaterialId = materialId;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(MaterialTranslationFormViewModel model, int materialId)
        {

            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();

            var RowLevelPermission = await _rowPermission.HasRowLevelPermissionAsync(role.Id, TablesNames.Materials, materialId, CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            if (!ModelState.IsValid)
            {
                model = await _materialsTranslatesRepository.InitializeMaterialTranslatesFormViewModelAsync(materialId, model);
                model.MaterialId = materialId;
                return View("Form", model);
            }

            var translate = await _materialsTranslatesRepository.GetMaterialTranslateByIdAsync(model.TranslationId);

            translate.Name = model.Name;


            _materialsTranslatesRepository.UpdateMaterial(translate);

            return RedirectToAction("Index", new { materialId = model.MaterialId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int translationId, int materialId)
        {
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();

            var RowLevelPermission = await _rowPermission.HasRowLevelPermissionAsync(role.Id, TablesNames.Materials,materialId, CrudOperations.Delete);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);

            var result = await _materialsTranslatesRepository.DeleteTranslationAsync(translationId); //returns true if deleted successfully
            if (result)
                return StatusCode(200);

            return BadRequest();

        }
    }
}
