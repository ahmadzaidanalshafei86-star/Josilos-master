using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class ProductAttributesTranslatesController : Controller
    {
        private readonly ProductAttributesRepository _productAttributesRepository;
        private readonly ProductAttributesTranslatesRepository _productAttributesTranslatesRepository;


        public ProductAttributesTranslatesController(ProductAttributesRepository productAttributesRepository,
            ProductAttributesTranslatesRepository productAttributesTranslatesRepository)
        {
            _productAttributesRepository = productAttributesRepository;
            _productAttributesTranslatesRepository = productAttributesTranslatesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.ProductAttributes.Read)]
        public async Task<IActionResult> Index(int productAttributeId)
        {
            var productAttribute = await _productAttributesRepository.GetProductAttributeWithTranslationsAsync(productAttributeId);

            if (productAttribute == null)
                return NotFound();

            ProductAttributeTranslateModel model = new()
            {
                ProductAttributeId = productAttribute.Id,
                AttributeName = productAttribute.Name,
                AttributeDefaultLang = productAttribute.Language.Code,
                PreEnteredTranslations = productAttribute.Translations.ToList(),
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Permissions.ProductAttributes.Create)]
        public async Task<IActionResult> Create(int productAttributeId)
        {
            var model = await _productAttributesTranslatesRepository
                .InitializeTranslatesFormViewModelAsync(productAttributeId);

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.ProductAttributes.Create)]
        public async Task<IActionResult> Create(ProductAttributeTranslateFormModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            await _productAttributesTranslatesRepository.AddProductAttributeTranslateAsync(model);

            return RedirectToAction("Index", new { productAttributeId = model.AttributeId });
        }

        [HttpGet]
        [Authorize(Permissions.ProductAttributes.Update)]
        public async Task<IActionResult> Edit(int attributeId, int translationId)
        {
            var model = await _productAttributesTranslatesRepository.InitializeTranslatesEditFormViewModelAsync(attributeId, translationId);
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.ProductAttributes.Update)]
        public async Task<IActionResult> Edit(ProductAttributeTranslateFormModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            await _productAttributesTranslatesRepository.UpdateTranslationsAsync(model);

            return RedirectToAction("Index", new { productAttributeId = model.AttributeId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.ProductAttributes.Delete))
                return StatusCode(403);

            var result = await _productAttributesTranslatesRepository.DeleteTranslationAsync(id);

            if (result)
                return StatusCode(200);

            return BadRequest();

        }

    }
}
