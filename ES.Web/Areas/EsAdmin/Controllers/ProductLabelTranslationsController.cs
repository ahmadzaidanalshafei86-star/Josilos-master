using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;


namespace ES.Web.Areas.EsAdmin.Controllers
{
    [Area("EsAdmin")]
    public class ProductLabelTranslationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductLabelTranslationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Permissions.Productlabels.Read)]
        public async Task<IActionResult> Index(int LabelId)
        {
            var ProductLabel = await _context.ProductLabels
                .Include(pa => pa.Language)
                .Include(pa => pa.ProductLabelTranslate)
                .ThenInclude(pat => pat.Language)
                .FirstOrDefaultAsync(pa => pa.Id == LabelId);

            if (ProductLabel is null)
                return NotFound();

            ProductLabelTranslateModel model = new()
            {
                ProductLabelId = ProductLabel.Id,
                ProductLabelName = ProductLabel.Name,
                ProductLabelDefaultLang = ProductLabel.Language.Code,
                CreatedAt = ProductLabel.CreatedAt,
                PreEnteredTranslations = ProductLabel.ProductLabelTranslate.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetTranslationForm(int productLabelId, int? translationId = null)
        {
            ProductLabelTranslateFormModel model = new();

            // in case of edit operation
            if (translationId.HasValue)
            {
                if (!User.HasClaim("Permission", Permissions.Productlabels.Update))
                    return StatusCode(403);

                var translation = await _context.ProductLabelTranslations.FindAsync(translationId);

                if (translation is null)
                    return NotFound();

                model = new ProductLabelTranslateFormModel
                {
                    TranslationId = translation.Id,
                    ProductLabelId = translation.ProductLabelId,
                    Name = translation.Name,
                    LanguageId = translation.LanguageId,
                };

                model.Languages = await GetLanguagesAsync(productLabelId);
            }
            // Initialize for create operation
            else
            {
                if (!User.HasClaim("Permission", Permissions.Productlabels.Create))
                    return StatusCode(403);

                model.Languages = await GetLanguagesAsync(productLabelId);
                model.ProductLabelId = productLabelId;
            }

            return PartialView("_TranslationForm", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTranslation(ProductLabelTranslateFormModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.TranslationId == 0) // Create new translation
                {
                    ProductLabelTranslate productLabelTranslate = new()
                    {
                        ProductLabelId = model.ProductLabelId,
                        Name = model.Name,
                        LanguageId = model.LanguageId,
                        CreatedAt = DateTime.Now,
                    };
                    await _context.ProductLabelTranslations.AddAsync(productLabelTranslate);
                    await _context.SaveChangesAsync();
                }
                else // Update existing translation
                {
                    var translation = await _context.ProductLabelTranslations.FindAsync(model.TranslationId);
                    if (translation is null)
                        return NotFound();

                    translation.Name = model.Name;
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }

            return PartialView("_TranslationForm", model); // Return form with validation errors
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Productlabels.Delete))
                return StatusCode(403);

            var translate = await _context.ProductLabelTranslations.FindAsync(id);

            if (translate == null)
                return StatusCode(404);

            _context.ProductLabelTranslations.Remove(translate);
            await _context.SaveChangesAsync();
            return StatusCode(200);

        }

        private async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int prdouctLabelId)
        {
            // Get all language IDs that already have translations for the given productLabel
            var translatedLanguageIds = await _context.ProductLabelTranslations
                .Where(ct => ct.ProductLabelId == prdouctLabelId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the productLabel and not showing it in the dropdown
            var productLabel = await _context.ProductLabels
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == prdouctLabelId);

            if (productLabel == null)
                throw new Exception(message: "product Label not found");

            return await _context.Languages
           .Where(l => l.Code != productLabel.Language.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }
    }
}
