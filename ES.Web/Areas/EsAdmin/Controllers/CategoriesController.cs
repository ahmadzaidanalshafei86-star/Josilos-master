using ES.Core.Entities;
using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class CategoriesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IImageService _imageService;
        private readonly SlugService _slugService;
        private readonly ILanguageService _languageService;
        private readonly RowPermission _rowPermission;
        private readonly CategoriesRepository _categoriesRepository;
        private readonly LanguagesRepository _languagesRepository;
        private readonly GalleryImagesRepository _galleryImagesRepository;
        private readonly MenuItemsRepository _menuItemsRepository;

        public CategoriesController(LanguagesRepository languagesRepository,
            SlugService slugService,
            CategoriesRepository categoriesRepository,
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
            _categoriesRepository = categoriesRepository;
            _rowPermission = rowPermission;
            _roleManager = roleManager;
            _menuItemsRepository = menuItemsRepository;
        }

        [Authorize(Permissions.Categories.Read)]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoriesRepository.GetCategoriesWithParentInfoAsync();
            return View(categories);
        }

        [HttpGet]
        [Authorize(Permissions.Categories.Create)]
        public async Task<IActionResult> Create()
        {
            var model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Categories.Create)]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
                return View("Form", model);
            }

            Category category = new()
            {
                Name = model.Name,
                Slug = _slugService.GenerateUniqueSlug(model.Name, nameof(Category)),
                LongDescription = model.LongDescription,
                ShortDescription = model.ShortDescription,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords,
                CreatedDate = DateTime.Now,
                TypeOfSorting = model.TypeOfSorting,
                GalleryStyle = model.GalleryStyle,
                Order = model.Order,
                ThemeId = model.ThemeId,
                Link = model.Link,
                ParentCategoryId = model.ParentCategoryId,
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
                IsPublished = model.IsPublished

            };
            await _categoriesRepository.AddRelatedCategoriesAsync(category, model.RelatedCategoryIds);
            var categoryId = await _categoriesRepository.AddCategoryAsync(category);

            // Handle CoverImage Image
            if (model.CoverImage is not null)
            {
                var CoverImageName = $"{categoryId}_coverImage{Path.GetExtension(model.CoverImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.CoverImage, CoverImageName, "/images/Categories");
                if (isUploaded)
                {
                    category.CoverImageUrl = CoverImageName;
                    category.CoverImageAltName = model.CoverImage.FileName;

                    _categoriesRepository.UpdateCategory(category);
                }
                else
                {
                    ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                    model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
                    return View("Form", model);
                }
            }

            // Handle FeaturedImage Image
            if (model.FeaturedImage is not null)
            {
                var FeaturedImageName = $"{categoryId}_FeaturedImage{Path.GetExtension(model.FeaturedImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.FeaturedImage, FeaturedImageName, "/images/Categories");
                if (isUploaded)
                {
                    category.FeaturedImageUrl = FeaturedImageName;
                    category.FeaturedImageAltName = model.FeaturedImage.FileName;

                    _categoriesRepository.UpdateCategory(category);
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                    model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
                    return View("Form", model);
                }
            }

            if (model.GalleryImages is not null)
            {

                for (int i = 0; i < model.GalleryImages.Count; i++)
                {
                    GalleryImage galleryImg = new();
                    var galleryImageName = $"{categoryId}_{i}{Path.GetExtension(model.GalleryImages[i].FileName)}";
                    var (isUploaded, errorMessage) = await _imageService.UploadASync(model.GalleryImages[i], galleryImageName, "/images/Categories/GalleryImages");
                    if (isUploaded)
                    {
                        galleryImg.GalleryImageUrl = galleryImageName;
                        galleryImg.AltName = model.GalleryImages[i].FileName;
                        galleryImg.CategoryId = category.Id;
                        galleryImg.DisplayOrder = i;
                        await _galleryImagesRepository.AddGalleryImageAsync(galleryImg);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                        model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
                        return View("Form", model);
                    }
                }
            }
            return Json(new { success = true, id = categoryId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoriesRepository.GetCategoryWithAllDataAsync(id);

            if (category is null)
                return NotFound();

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                           .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.Categories,
                                           category.Id,
                                           CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");


            var model = new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                LongDescription = category.LongDescription,
                ShortDescription = category.ShortDescription,
                MetaDescription = category.MetaDescription,
                MetaKeywords = category.MetaKeywords,
                TypeOfSorting = category.TypeOfSorting,
                Order = category.Order,
                ThemeId = category.ThemeId,
                ParentCategoryId = category.ParentCategoryId,
                IsPublished = category.IsPublished,
                Link = category.Link,
                CoverImageUrl = category.CoverImageUrl,
                FeaturedImageUrl = category.FeaturedImageUrl,
                ExistingGalleryImages = category.GalleryImages?.Select(g => new GalleryImageViewModel
                {
                    GalleryImageUrl = g.GalleryImageUrl,
                    GalleryImageAltName = g.AltName,
                    DisplayOrder = g.DisplayOrder
                }).OrderBy(g => g.DisplayOrder)
                .ToList(),
                RelatedCategoryIds = category.RelatedCategories?.Select(c => c.Id).ToList(),
            };

            model = await _categoriesRepository.InitializeCategoryFormViewModelAsync(model);

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();

            var RowLevelPermission = await _rowPermission
                                            .HasRowLevelPermissionAsync(role.Id,
                                            TablesNames.Categories,
                                            id,
                                            CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            if (!ModelState.IsValid)
            {
                model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
                return View("Form", model);
            }

            var category = await _categoriesRepository.GetCategoryWithAllDataAsync(id);
            if (category == null)
                return NotFound();


            // Handle CoverImage Image
            if (model.CoverImage is not null)
            {
                // Delete old cover image if it exists
                if (!string.IsNullOrEmpty(category.CoverImageUrl))
                    _imageService.Delete($"/images/Categories/{category.CoverImageUrl}");

                var CoverImageName = $"{category.Id}_coverImage{Path.GetExtension(model.CoverImage.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.CoverImage, CoverImageName, "/images/Categories");

                if (isUploaded)
                {
                    category.CoverImageUrl = CoverImageName;
                    category.CoverImageAltName = model.CoverImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                    model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
                    return View("Form", model);
                }
            }
            else if (!string.IsNullOrEmpty(model.CoverImageUrl) && !model.KeepCoverImage)
            {
                _imageService.Delete($"/images/Categories/{category.CoverImageUrl}");
                category.CoverImageAltName = null;
                category.CoverImageUrl = null;
            }

            // Handle Featured Image
            if (model.FeaturedImage is not null)
            {
                // Delete old featured image if it exists
                if (!string.IsNullOrEmpty(category.FeaturedImageUrl))
                    _imageService.Delete($"/images/Categories/{category.FeaturedImageUrl}");

                var FeaturedImageName = $"{category.Id}_FeaturedImage{Path.GetExtension(model.FeaturedImage.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.FeaturedImage, FeaturedImageName, "/images/Categories");

                if (isUploaded)
                {
                    category.FeaturedImageUrl = FeaturedImageName;
                    category.FeaturedImageAltName = model.FeaturedImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                    model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
                    return View("Form", model);
                }
            }
            else if (!string.IsNullOrEmpty(model.FeaturedImageUrl) && !model.KeepFeaturedImage)
            {
                _imageService.Delete($"/images/Categories/{category.FeaturedImageUrl}");
                category.FeaturedImageAltName = null;
                category.FeaturedImageUrl = null;
            }

            //get the old gallery images
            var CategoryOldGalleryImages = await _galleryImagesRepository.GetGalleryImagesOfCategoryAsync(category.Id);


            if (CategoryOldGalleryImages.Any())
            {
                _galleryImagesRepository.DeleteRangeGalleryImages(CategoryOldGalleryImages); //delete them from the database
                //Delete all of them from the server if exits
                foreach (var gallerImage in CategoryOldGalleryImages)
                {
                    _imageService.Delete($"/images/Categories/GalleryImages/{gallerImage.GalleryImageUrl}");
                }
            }

            // Add the new Gallery Images
            if (model.GalleryImages is not null)
            {
                for (int i = 0; i < model.GalleryImages.Count; i++)
                {
                    GalleryImage galleryImg = new();
                    var galleryImageName = $"{category.Id}_{i}{Path.GetExtension(model.GalleryImages[i].FileName)}";
                    var (isUploaded, errorMessage) = await _imageService.UploadASync(model.GalleryImages[i], galleryImageName, "/images/Categories/GalleryImages");
                    if (isUploaded)
                    {
                        galleryImg.GalleryImageUrl = galleryImageName;
                        galleryImg.AltName = model.GalleryImages[i].FileName;
                        galleryImg.CategoryId = category.Id;
                        galleryImg.DisplayOrder = i;
                        await _galleryImagesRepository.AddGalleryImageAsync(galleryImg);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                        model = await _categoriesRepository.InitializeCategoryFormViewModelAsync();
                        return View("Form", model);
                    }
                }
            }

            if (category.Name != model.Name)
                category.Slug = _slugService.GenerateUniqueSlug(model.Name, nameof(Category), category.Id);

            category.Name = model.Name;
            category.LongDescription = model.LongDescription;
            category.ShortDescription = model.ShortDescription;
            category.MetaDescription = model.MetaDescription;
            category.MetaKeywords = model.MetaKeywords;
            category.TypeOfSorting = model.TypeOfSorting;
            category.GalleryStyle = model.GalleryStyle;
            category.Order = model.Order;
            category.ThemeId = model.ThemeId;
            category.ParentCategoryId = model.ParentCategoryId;
            category.LanguageId = await _languagesRepository.GetLanguageByCode("en-US");
            category.Link = model.Link;
            category.IsPublished = model.IsPublished;

            // Remove old Related categories and Add the new 
            category.RelatedCategories?.Clear();
            await _categoriesRepository.AddRelatedCategoriesAsync(category, model.RelatedCategoryIds);

            _categoriesRepository.UpdateCategory(category);

            //return Json(new { success = true });

            return Json(new { success = true, id = category.Id });

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var category = await _categoriesRepository.GetCategoryByIdAsync(id);

            if (category is null)
                return NotFound();

            // Prevent Deleting "Home Slider"
            if (category.Name == "Home Slider")
                return BadRequest("This category cannot be deleted.");

            //Check Authroization to delete this category
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                .HasRowLevelPermissionAsync(role.Id, TablesNames.Categories, id, CrudOperations.Delete);
            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);

            //Remove all row level permissions of this category
            var existingCategoryRowPermissions = await _rowPermission
                .GetRowLevelPermissionsToDeleteAsync(id, TablesNames.Categories);
            _rowPermission.RemoveRange(existingCategoryRowPermissions);

            //Remove all row level permissions of related pages
            var existingPageRowPermissions = await _rowPermission
                .GetRowLevelPermissionsToDeleteAsync(id, TablesNames.PagesOfCategory);
            _rowPermission.RemoveRange(existingPageRowPermissions);

            if (_categoriesRepository.IsParentCategory(id) || _categoriesRepository.IsRelatedToAnotherCategory(id))
                return StatusCode(400);

            if (!string.IsNullOrEmpty(category.FeaturedImageUrl))
                _imageService.Delete($"/images/Categories/{category.FeaturedImageUrl}");

            if (!string.IsNullOrEmpty(category.CoverImageUrl))
                _imageService.Delete($"/images/Categories/{category.CoverImageUrl}");

            //delete galleryImages
            var galleryImages = await _galleryImagesRepository.GetGalleryImagesOfCategoryAsync(id);
            if (galleryImages.Any())
            {
                foreach (var image in galleryImages)
                {
                    _imageService.Delete($"/images/Categories/GalleryImages/{image.GalleryImageUrl}");
                }
            }

            if (category.PagesRelatedToThis.Any())
            {
                foreach (var page in category.PagesRelatedToThis)
                {
                    if (!string.IsNullOrEmpty(page.FeatruedImageUrl))
                        _imageService.Delete($"/images/Pages/{page.FeatruedImageUrl}");
                    if (!string.IsNullOrEmpty(page.CoverImageUrl))
                        _imageService.Delete($"/images/Pages/{page.CoverImageUrl}");
                    if (page.PageFiles.Any())
                    {
                        foreach (var file in page.PageFiles)
                        {
                            _imageService.Delete($"/documents/Pages/{file.FileUrl}");
                        }
                    }
                    if (page.PageGalleryImages.Any())
                    {
                        foreach (var image in page.PageGalleryImages)
                        {
                            _imageService.Delete($"/images/Pages/GalleryImages/{image.GalleryImageUrl}");
                        }
                    }
                }
            }

            var result = await _categoriesRepository.DeleteCategoryAsync(id); //returns true if deleted successfully
            if (result)
                return StatusCode(200);

            return BadRequest();

        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            //Check Authroization to update this category 
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                          TablesNames.Categories,
                                          id,
                                          CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);

            var category = await _categoriesRepository.GetCategoryByIdAsync(id);

            if (category is null)
                return NotFound();

            category.IsPublished = !category.IsPublished;

            _categoriesRepository.UpdateCategory(category);

            return StatusCode(200);
        }
    }
}
