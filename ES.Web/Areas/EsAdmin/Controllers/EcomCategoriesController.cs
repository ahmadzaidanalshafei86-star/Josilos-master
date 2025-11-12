using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class EcomCategoriesController : Controller
    {
        private readonly EcomCategoriesRepository _ecomCategoriesRepository;
        private readonly ProductsRepository _productsRepository;
        private readonly IImageService _imageService;
        private readonly SlugService _slugService;
        private readonly ILanguageService _languageService;
        private readonly LanguagesRepository _languagesRepository;
        private readonly MenuItemsRepository _menuItemsRepository;

        public EcomCategoriesController(EcomCategoriesRepository ecomCategoriesRepository,
            IImageService imageService,
            SlugService slugService,
            ILanguageService languageService,
            LanguagesRepository languagesRepository,
            MenuItemsRepository menuItemsRepository,
            ProductsRepository productsRepository)
        {
            _ecomCategoriesRepository = ecomCategoriesRepository;
            _imageService = imageService;
            _slugService = slugService;
            _languageService = languageService;
            _languagesRepository = languagesRepository;
            _menuItemsRepository = menuItemsRepository;
            _productsRepository = productsRepository;
        }

        [Authorize(Permissions.ProductCategories.Read)]
        public async Task<IActionResult> Index()
        {
            var categories = await _ecomCategoriesRepository.GetCategories();
            return View(categories);
        }

        [HttpGet]
        [Authorize(Permissions.ProductCategories.Create)]
        public async Task<IActionResult> Create()
        {
            EcomCategoryFormViewModel model = new();
            model.Categories = await _ecomCategoriesRepository.GetCategoriesNamesAsync();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.ProductCategories.Create)]
        public async Task<IActionResult> Create(EcomCategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _ecomCategoriesRepository.GetCategoriesNamesAsync();
                return View("Form", model);
            }

            EcomCategory category = new()
            {
                Name = model.Name,
                Slug = _slugService.GenerateUniqueSlug(model.Name, nameof(EcomCategory)),
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
                ParentCategoryId = model.ParentCategoryId,
                LongDescription = model.LongDescription,
                ShortDescription = model.ShortDescription,
                MetaDescription = model.MetaDescription, // Use ShortDescription if MetaDescription is null
                MetaKeywords = model.MetaKeywords,
                IsPublished = model.IsPublished
            };

            var categoryId = await _ecomCategoriesRepository.AddCategoryAsync(category);

            // Handle CoverImage Image
            if (model.CoverImage is not null)
            {
                var CoverImageName = $"{categoryId}_coverImage{Path.GetExtension(model.CoverImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.CoverImage, CoverImageName, "/images/EcomCategories");
                if (isUploaded)
                {
                    category.CoverImageUrl = CoverImageName;
                    category.CoverImageAltName = model.CoverImage.FileName;

                    await _ecomCategoriesRepository.UpdateCategory(category);
                }
                else
                {
                    ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                    return View("Form", model);
                }
            }

            // Handle FeaturedImage Image
            if (model.FeaturedImage is not null)
            {
                var FeaturedImageName = $"{categoryId}_FeaturedImage{Path.GetExtension(model.FeaturedImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.FeaturedImage, FeaturedImageName, "/images/EcomCategories");
                if (isUploaded)
                {
                    category.FeaturedImageUrl = FeaturedImageName;
                    category.FeaturedImageAltName = model.FeaturedImage.FileName;

                    await _ecomCategoriesRepository.UpdateCategory(category);
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                    return View("Form", model);
                }
            }

            return RedirectToAction("Edit", new { id = categoryId });
        }


        [HttpGet]
        [Authorize(Permissions.ProductCategories.Update)]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _ecomCategoriesRepository.GetCategoryByIdAsync(id);

            if (category is null)
                return NotFound();

            var model = new EcomCategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                LongDescription = category.LongDescription,
                ShortDescription = category.ShortDescription,
                CoverImageUrl = category.CoverImageUrl,
                FeaturedImageUrl = category.FeaturedImageUrl,
                MetaDescription = category.MetaDescription,
                MetaKeywords = category.MetaKeywords,
                ParentCategoryId = category.ParentCategoryId,
                IsPublished = category.IsPublished,
                Categories = await _ecomCategoriesRepository.GetCategoriesNamesAsync()
            };

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.ProductCategories.Update)]
        public async Task<IActionResult> Edit(EcomCategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _ecomCategoriesRepository.GetCategoriesNamesAsync();
                return View("Form", model);
            }

            var category = await _ecomCategoriesRepository.GetCategoryByIdAsync(model.Id);
            if (category is null)
                return NotFound();

            // Handle CoverImage Image
            if (model.CoverImage is not null)
            {
                // Delete old cover image if it exists
                if (!string.IsNullOrEmpty(category.CoverImageUrl))
                    _imageService.Delete($"/images/EcomCategories/{category.CoverImageUrl}");

                var CoverImageName = $"{category.Id}_coverImage{Path.GetExtension(model.CoverImage.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.CoverImage, CoverImageName, "/images/EcomCategories");

                if (isUploaded)
                {
                    category.CoverImageUrl = CoverImageName;
                    category.CoverImageAltName = model.CoverImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                    model.Categories = await _ecomCategoriesRepository.GetCategoriesNamesAsync();
                    return View("Form", model);
                }
            }
            else if (!string.IsNullOrEmpty(model.CoverImageUrl) && !model.KeepCoverImage)
            {
                _imageService.Delete($"/images/EcomCategories/{category.CoverImageUrl}");
                category.CoverImageAltName = null;
                category.CoverImageUrl = null;
            }

            // Handle Featured Image
            if (model.FeaturedImage is not null)
            {
                // Delete old featured image if it exists
                if (!string.IsNullOrEmpty(category.FeaturedImageUrl))
                    _imageService.Delete($"/images/EcomCategories/{category.FeaturedImageUrl}");

                var FeaturedImageName = $"{category.Id}_FeaturedImage{Path.GetExtension(model.FeaturedImage.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.FeaturedImage, FeaturedImageName, "/images/EcomCategories");

                if (isUploaded)
                {
                    category.FeaturedImageUrl = FeaturedImageName;
                    category.FeaturedImageAltName = model.FeaturedImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                    model.Categories = await _ecomCategoriesRepository.GetCategoriesNamesAsync();
                    return View("Form", model);
                }
            }
            else if (!string.IsNullOrEmpty(model.FeaturedImageUrl) && !model.KeepFeaturedImage)
            {
                _imageService.Delete($"/images/EcomCategories/{category.FeaturedImageUrl}");
                category.FeaturedImageAltName = null;
                category.FeaturedImageUrl = null;
            }

            if (category.Name != model.Name)
                category.Slug = _slugService.GenerateUniqueSlug(model.Name, nameof(EcomCategory), category.Id);

            category.Name = model.Name;
            category.LongDescription = model.LongDescription;
            category.ShortDescription = model.ShortDescription;
            category.MetaDescription = model.MetaDescription;
            category.MetaKeywords = model.MetaKeywords;
            category.ParentCategoryId = model.ParentCategoryId;
            category.IsPublished = model.IsPublished;

            await _ecomCategoriesRepository.UpdateCategory(category);

            return View("Form", model);

        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.ProductCategories.Delete))
                return StatusCode(403);

            var category = await _ecomCategoriesRepository.GetCategoryByIdAsync(id);

            if (category is null)
                return NotFound();

            if (_ecomCategoriesRepository.IsParentCategory(id))
                return StatusCode(400);

            // Get all products in this category
            var products = await _productsRepository.GetProductsByCategoryIdAsync(id);

            foreach (var product in products)
            {
                var otherCategories = await _productsRepository.GetProductCategoriesAsync(product.Id);

                if (otherCategories.Count == 1)
                {
                    await _productsRepository.DeleteAsync(product);
                }
                else
                {
                    await _productsRepository.RemoveProductFromCategoryAsync(product.Id, id);
                }
            }

            if (!string.IsNullOrEmpty(category.FeaturedImageUrl))
                _imageService.Delete($"/images/EcomCategories/{category.FeaturedImageUrl}");

            if (!string.IsNullOrEmpty(category.CoverImageUrl))
                _imageService.Delete($"/images/EcomCategories/{category.CoverImageUrl}");

            await _ecomCategoriesRepository.DeleteCategory(category);

            return StatusCode(200);

        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!User.HasClaim("Permission", Permissions.ProductCategories.Update))
                return StatusCode(403);

            var category = await _ecomCategoriesRepository.GetCategoryByIdAsync(id);

            if (category is null)
                return NotFound();

            category.IsPublished = !category.IsPublished;

            await _ecomCategoriesRepository.UpdateCategory(category);

            return StatusCode(200);
        }
    }
}
