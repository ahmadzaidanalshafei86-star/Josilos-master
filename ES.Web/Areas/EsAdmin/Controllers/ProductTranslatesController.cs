using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class ProductTranslatesController : Controller
    {
        private readonly ProductTranslatesRepository _productTranslatesRepository;
        private readonly ProductsRepository _productsRepository;

        public ProductTranslatesController(ProductsRepository productsRepository,
            ProductTranslatesRepository productTranslatesRepository)
        {
            _productsRepository = productsRepository;
            _productTranslatesRepository = productTranslatesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.Products.Read)]
        public async Task<IActionResult> Index(int productId)
        {
            var product = await _productsRepository.GetProductWithTranslationsAsync(productId);

            if (product is null)
                return NotFound();

            ProductTranslatesViewModel model = new()
            {
                ProductId = productId,
                ProductTitle = product.Title,
                ProductDefaultLang = product.Language!.Code,
                CreatedDate = product.CreatedDate,
                PreEnteredTranslations = product.ProductTranslates?.ToList(),

            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Permissions.Products.Create)]
        public async Task<IActionResult> Create(int productId)
        {
            var model = await _productTranslatesRepository.InitializeProductTranslatesFormViewModelAsync(productId);
            model.ProductId = productId;

            var product = await _productsRepository.GetProductWithTabsAsync(productId);

            if (product is null)
                return NotFound();

            model.Title = product.Title;
            model.ShortDescription = product.ShortDescription;
            model.LongDescription = product.LongDescription;
            model.MetaDescription = product.MetaDescription;
            model.MetaKeywords = product.MetaKeywords;
            model.ProductTabs = product.ProductTabs.Select(tab => new ProductTabViewModel
            {
                Id = tab.Id,
                Title = tab.Title,
                Content = tab.Content,
                Order = tab.Order,
            }).OrderBy(tab => tab.Order)
              .ToList();


            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Products.Create)]
        public async Task<IActionResult> Create(ProductTranslateFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var productTranslate = new ProductTranslate
            {
                LanguageId = (int)model.LanguageId!,
                Title = model.Title,
                ProductId = model.ProductId,
                ShortDescription = model.ShortDescription,
                LongDescription = model.LongDescription,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords
            };

            if (model.ProductTabs.Count > 0)
            {
                foreach (var tab in model.ProductTabs)
                {
                    var tabTranslte = new ProductTabTranslation
                    {
                        ProductTabId = tab.Id,
                        ProductId = model.ProductId,
                        TranslatedTitle = tab.Title,
                        TranslatedContent = tab.Content,
                        Order = tab.Order,
                        LanguageId = (int)model.LanguageId!,
                    };
                    await _productTranslatesRepository.AddProductTabTranslateAsync(tabTranslte);
                }
            }

            await _productTranslatesRepository.AddProductTranslateAsync(productTranslate);

            await _productTranslatesRepository.SaveChangesAsync();
            return RedirectToAction("Index", new { productId = model.ProductId });
        }

        [HttpGet]
        [Authorize(Permissions.Products.Update)]
        public async Task<IActionResult> Edit(int productId, int translateid)
        {
            var productTranslate = await _productTranslatesRepository.GetProductTranslateByIdAsync(translateid);

            if (productTranslate is null)
                return NotFound();

            var model = new ProductTranslateFormViewModel
            {
                TranslationId = productTranslate.Id,
                Title = productTranslate.Title,
                ShortDescription = productTranslate.ShortDescription,
                LongDescription = productTranslate.LongDescription,
                MetaDescription = productTranslate.MetaDescription,
                MetaKeywords = productTranslate.MetaKeywords,
                ProductTabs = await _productTranslatesRepository
                    .GetTabTranslationsAsync(productId, productTranslate.LanguageId)
            };

            model = await _productTranslatesRepository.InitializeProductTranslatesFormViewModelAsync(productId, model);
            model.ProductId = productId;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Products.Update)]
        public async Task<IActionResult> Edit(ProductTranslateFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var productTranslate = await _productTranslatesRepository.GetProductTranslateByIdAsync(model.TranslationId);

            if (productTranslate is null)
                return NotFound();

            productTranslate.Title = model.Title;
            productTranslate.ShortDescription = model.ShortDescription;
            productTranslate.LongDescription = model.LongDescription;
            productTranslate.MetaDescription = model.MetaDescription;
            productTranslate.MetaKeywords = model.MetaKeywords;

            if (model.ProductTabs.Count > 0)
            {
                await _productTranslatesRepository.UpdatProductTabTranslationsAsync(
                    model.ProductId,
                    productTranslate.LanguageId,
                    model.ProductTabs
                );
            }



            await _productTranslatesRepository.SaveChangesAsync();
            return RedirectToAction("Index", new { productId = model.ProductId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int translationId, int productId, int languageId)
        {
            if (!User.HasClaim("Permission", Permissions.Products.Delete))
                return StatusCode(403);

            var success = await _productTranslatesRepository.DeleteTranslation(translationId, productId, languageId);

            if (!success)
                return StatusCode(404);

            return StatusCode(200);
        }

    }
}
