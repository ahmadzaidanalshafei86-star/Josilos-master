using ES.Core.Entities;
using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class PagesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PagesRepository _pagesRepository;
        private readonly ILanguageService _languageService;
        private readonly IImageService _imageService;
        private readonly IFilesService _filesService;
        private readonly SlugService _slugService;
        private readonly LanguagesRepository _languagesRepository;
        private readonly GalleryImagesRepository _galleryImagesRepository;
        private readonly PageFilesRepository _pageFilesRepository;
        private readonly RowPermission _rowPermission;
        private readonly MenuItemsRepository _menuItemsRepository;
        public PagesController(PagesRepository pagesRepository,
            LanguagesRepository languagesRepository,
            GalleryImagesRepository galleryImagesRepository,
            ILanguageService languageService,
            IImageService imageService,
            IFilesService filesService,
            SlugService slugService,
            PageFilesRepository pageFilesRepository,
            CategoriesRepository categoriesRepository,
            DocumentsRepository documentsRepository,
            RowPermission rowPermission,
            RoleManager<IdentityRole> roleManager,
            MenuItemsRepository menuItemsRepository)
        {
            _pagesRepository = pagesRepository;
            _languagesRepository = languagesRepository;
            _galleryImagesRepository = galleryImagesRepository;
            _languageService = languageService;
            _imageService = imageService;
            _filesService = filesService;
            _slugService = slugService;
            _pageFilesRepository = pageFilesRepository;
            _rowPermission = rowPermission;
            _roleManager = roleManager;
            _menuItemsRepository = menuItemsRepository;
        }

        [Authorize(Permissions.Pages.Read)]
        public async Task<IActionResult> Index()
        {
            var pages = await _pagesRepository.GetAllPagesWithCategoryNameAsync();

            // Get distinct categories for the dropdown filter
            var categories = pages
                  .Select(p => new {
                      Id = p.CategoryId,
                      Name = p.CategoryName
                  })
                  .Distinct()
                  .OrderBy(c => c.Name)
                  .ToList();


            ViewBag.Categories = categories;

            return View(pages);
        }
        [HttpGet]
        [Authorize(Permissions.Pages.Create)]
        public async Task<IActionResult> Create()
        {
            var model = await _pagesRepository.InitializePageFormViewModelAsync();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Pages.Create)]
        public async Task<IActionResult> Create(PageFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _pagesRepository.InitializePageFormViewModelAsync();
                return View("Form", model);
            }

            Page page = new()
            {
                Title = model.Title,
                Slug = _slugService.GenerateUniqueSlug(model.Title, nameof(Page)),
                CategoryId = model.CategoryId,
                FormId = model.FormId,
                DateInput = model.DateInput,
                CreatedDate = DateTime.Now,
                LongDescription = model.LongDescription,
                ShortDescription = model.ShortDescription,
                VideoURL = model.VideoURL,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords,
                Order = model.Order,
                IsPublished = model.IsPublished,
                GalleryStyle = model.GalleryStyle,
                Count = model.Count,
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
            };

            //Handle LinkTo
            // in case LinkTo Category Is Selected
            if (model.LinkToType == "2")
            {
                page.LinkToType = "2";
                page.LinkTo = model.LinkTo!;
                page.LinkToUrl = $"/Categories/{model.LinkTo}";

            } // in case LinkTo File Is Selected
            else if (model.LinkToType == "3")
            {
                page.LinkToType = "3";
                page.LinkTo = model.LinkTo!;
                page.LinkToUrl = model.LinkTo!;
            } // in case LinkTo URL Is Selected
            else if (model.LinkToType == "4")
            {
                page.LinkToType = "4";
                page.LinkTo = model.LinkTo!;
                page.LinkToUrl = model.LinkTo!;
            }
            // in case LinkTo Page is selected
            else
            {
                page.LinkToType = "1";
                page.LinkTo = page.Slug;
                page.LinkToUrl = $"/Page/{page.Slug}";
            }


            var pageId = await _pagesRepository.AddPageAsync(page);

            // Handle CoverImage Image
            if (model.CoverImage is not null)
            {
                var CoverImageName = $"{pageId}_coverImage{Path.GetExtension(model.CoverImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.CoverImage, CoverImageName, "/images/Pages");
                if (isUploaded)
                {
                    page.CoverImageUrl = CoverImageName;
                    page.CoverImageAltName = model.CoverImage.FileName;
                    _pagesRepository.Updatepage(page);
                }
                else
                {
                    ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                    model = await _pagesRepository.InitializePageFormViewModelAsync();
                    return View("Form", model);
                }
            }

            // Handle FeaturedImage Image
            if (model.FeaturedImage is not null)
            {
                var FeaturedImageName = $"{pageId}_FeaturedImage{Path.GetExtension(model.FeaturedImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.FeaturedImage, FeaturedImageName, "/images/Pages");
                if (isUploaded)
                {
                    page.FeatruedImageUrl = FeaturedImageName;
                    page.FeaturedImageAltName = model.FeaturedImage.FileName;
                    _pagesRepository.Updatepage(page);
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                    model = await _pagesRepository.InitializePageFormViewModelAsync();
                    return View("Form", model);
                }
            }

            //Add related Categoris
            if (model.RelatedCategoryIds.Any())
                await _pagesRepository.AddRelatedCategoriesAsync(page, model.RelatedCategoryIds);

            //GalleryImages
            if (model.GalleryImages is not null)
            {

                for (int i = 0; i < model.GalleryImages.Count; i++)
                {
                    PageGalleryImage pageGalleryImg = new();
                    var galleryImageName = $"{pageId}_{i}{Path.GetExtension(model.GalleryImages[i].FileName)}";
                    var (isUploaded, errorMessage) = await _imageService.UploadASync(model.GalleryImages[i], galleryImageName, "/images/Pages/GalleryImages");
                    if (isUploaded)
                    {
                        pageGalleryImg.GalleryImageUrl = galleryImageName;
                        pageGalleryImg.AltName = model.GalleryImages[i].FileName;
                        pageGalleryImg.PageId = page.Id;
                        pageGalleryImg.DisplayOrder = i;
                        await _galleryImagesRepository.AddPageGalleryImageAsync(pageGalleryImg);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                        model = await _pagesRepository.InitializePageFormViewModelAsync();
                        return View("Form", model);
                    }
                }
            }

            //Files
            if (model.PageFiles is not null)
            {

                for (int i = 0; i < model.PageFiles.Count; i++)
                {
                    PageFile pageFile = new();
                    var pageFileName = $"{pageId}_{i}{Path.GetExtension(model.PageFiles[i].FileName)}";
                    var (isUploaded, errorMessage) = await _filesService.UploadASync(model.PageFiles[i], pageFileName, "/documents/Pages");
                    if (isUploaded)
                    {
                        pageFile.FileUrl = pageFileName;
                        pageFile.AltName = model.PageFiles[i].FileName;
                        pageFile.PageId = page.Id;
                        pageFile.DisplayOrder = i;
                        await _pageFilesRepository.AddFileAsync(pageFile);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                        model = await _pagesRepository.InitializePageFormViewModelAsync();
                        return View("Form", model);
                    }
                }
            }
            return Json(new { success = true, id = pageId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var page = await _pagesRepository.GetPageWithGalleryImagesAsync(id);
            if (page is null)
                return NotFound();

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.PagesOfCategory,
                                           page.CategoryId,
                                           CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            var model = new PageFormViewModel()
            {
                Id = page.Id,
                Title = page.Title,
                CategoryId = page.CategoryId,
                FormId = page.FormId,
                ShortDescription = page.ShortDescription,
                LongDescription = page.LongDescription,
                MetaDescription = page.MetaDescription,
                MetaKeywords = page.MetaKeywords,
                CoverImageUrl = page.CoverImageUrl,
                FeaturedImageUrl = page.FeatruedImageUrl,
                VideoURL = page.VideoURL,
                Order = page.Order,
                LinkToType = page.LinkToType,
                LinkTo = page.LinkTo,
                GalleryStyle = page.GalleryStyle,
                ExistingGalleryImages = page.PageGalleryImages?.Select(g => new GalleryImageViewModel
                {
                    GalleryImageUrl = g.GalleryImageUrl,
                    GalleryImageAltName = g.AltName,
                    DisplayOrder = g.DisplayOrder
                }).OrderBy(g => g.DisplayOrder)
                .ToList(),
                ExistingFiles = page.PageFiles?.Select(f => new FileViewModel
                {
                    FileUrl = f.FileUrl,
                    FileAltName = f.AltName,
                    DisplayOrder = f.DisplayOrder
                }).OrderBy(f => f.DisplayOrder)
                .ToList(),
                IsPublished = page.IsPublished,
                RelatedCategoryIds = page.RelatedCategories?.Select(c => c.CategoryId).ToList(),
                Count = page.Count,
            };

            model = await _pagesRepository.InitializePageFormViewModelAsync(model);
            model.DateInput = page.DateInput;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, PageFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _pagesRepository.InitializePageFormViewModelAsync(model);
                return View("Form", model);
            }

            var page = await _pagesRepository.GetPageByIdAsync(id);
            if (page is null)
                return NotFound();

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.PagesOfCategory,
                                           page.CategoryId,
                                           CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            // Handle CoverImage Image
            if (model.CoverImage is not null)
            {
                // Delete old cover image if it exists
                if (!string.IsNullOrEmpty(page.CoverImageUrl))
                    _imageService.Delete($"/images/Categories/{page.CoverImageUrl}");

                var CoverImageName = $"{page.Id}_coverImage{Path.GetExtension(model.CoverImage.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.CoverImage, CoverImageName, "/images/Pages");

                if (isUploaded)
                {
                    page.CoverImageUrl = CoverImageName;
                    page.CoverImageAltName = model.CoverImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                    model = await _pagesRepository.InitializePageFormViewModelAsync(model);
                    return View("Form", model);
                }
            }
            else if (!string.IsNullOrEmpty(model.CoverImageUrl) && !model.KeepCoverImage)
            {
                _imageService.Delete($"/images/Pages/{page.CoverImageUrl}");
                page.CoverImageAltName = null;
                page.CoverImageUrl = null;
            }

            // Handle Featured Image
            if (model.FeaturedImage is not null)
            {
                // Delete old featured image if it exists
                if (!string.IsNullOrEmpty(page.FeatruedImageUrl))
                    _imageService.Delete($"/images/Categories/{page.FeatruedImageUrl}");

                var FeaturedImageName = $"{page.Id}_FeaturedImage{Path.GetExtension(model.FeaturedImage.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.FeaturedImage, FeaturedImageName, "/images/Pages");

                if (isUploaded)
                {
                    page.FeatruedImageUrl = FeaturedImageName;
                    page.FeaturedImageAltName = model.FeaturedImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                    model = await _pagesRepository.InitializePageFormViewModelAsync(model);
                    return View("Form", model);
                }
            }
            else if (!string.IsNullOrEmpty(model.FeaturedImageUrl) && !model.KeepFeaturedImage)
            {
                _imageService.Delete($"/images/Pages/{page.FeatruedImageUrl}");
                page.FeaturedImageAltName = null;
                page.FeatruedImageUrl = null;
            }

            //get the page old gallery images
            var PageOldGalleryImages = await _galleryImagesRepository.GetGalleryImagesOfPageAsync(page.Id);

            if (PageOldGalleryImages.Any())
            {
                _galleryImagesRepository.DeleteRangePageGalleryImages(PageOldGalleryImages); //delete them from the database
                //Delete all of them from the server if exits
                foreach (var gallerImage in PageOldGalleryImages)
                {
                    _imageService.Delete($"/images/Pages/GalleryImages/{gallerImage.GalleryImageUrl}");
                }
            }

            // Add the new Gallery Images
            if (model.GalleryImages is not null)
            {
                for (int i = 0; i < model.GalleryImages.Count; i++)
                {
                    PageGalleryImage pageGalleryImg = new();
                    var galleryImageName = $"{page.Id}_{i}{Path.GetExtension(model.GalleryImages[i].FileName)}";
                    var (isUploaded, errorMessage) = await _imageService.UploadASync(model.GalleryImages[i], galleryImageName, "/images/Pages/GalleryImages");
                    if (isUploaded)
                    {
                        pageGalleryImg.GalleryImageUrl = galleryImageName;
                        pageGalleryImg.AltName = model.GalleryImages[i].FileName;
                        pageGalleryImg.PageId = page.Id;
                        pageGalleryImg.DisplayOrder = i;
                        await _galleryImagesRepository.AddPageGalleryImageAsync(pageGalleryImg);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                        model = await _pagesRepository.InitializePageFormViewModelAsync();
                        return View("Form", model);
                    }
                }
            }


            //get the page old files
            var PageOldFiles = await _pageFilesRepository.GetFilesOfPageAsync(page.Id);

            if (PageOldFiles.Any())
            {
                _pageFilesRepository.DeleteRangeFiles(PageOldFiles);//delete them from the database
                //Delete all of them from the server if exits
                foreach (var file in PageOldFiles)
                {
                    _filesService.Delete($"/documents/Pages/{file.FileUrl}");
                }
            }

            // Add the new files
            if (model.PageFiles is not null)
            {
                for (int i = 0; i < model.PageFiles.Count; i++)
                {
                    PageFile pageFile = new();
                    var pageFileName = $"{page.Id}_{i}{Path.GetExtension(model.PageFiles[i].FileName)}";
                    var (isUploaded, errorMessage) = await _filesService.UploadASync(model.PageFiles[i], pageFileName, "/documents/Pages");
                    if (isUploaded)
                    {
                        pageFile.FileUrl = pageFileName;
                        pageFile.AltName = model.PageFiles[i].FileName;
                        pageFile.PageId = page.Id;
                        pageFile.DisplayOrder = i;
                        await _pageFilesRepository.AddFileAsync(pageFile);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                        model = await _pagesRepository.InitializePageFormViewModelAsync();
                        return View("Form", model);
                    }
                }
            }


            page.Title = model.Title;
            page.Slug = _slugService.GenerateUniqueSlug(model.Title, nameof(Page), page.Id);
            page.CategoryId = model.CategoryId;
            page.FormId = model.FormId;
            page.DateInput = model.DateInput;
            page.LongDescription = model.LongDescription;
            page.ShortDescription = model.ShortDescription;
            page.VideoURL = model.VideoURL;
            page.MetaDescription = model.MetaDescription;
            page.MetaKeywords = model.MetaKeywords;
            page.Order = model.Order;
            page.IsPublished = model.IsPublished;
            page.GalleryStyle = model.GalleryStyle;
            page.Count = model.Count;

            //Handle LinkTo
            // in case LinkTo Category Is Selected
            if (model.LinkToType == "2")
            {
                page.LinkToType = "2";
                page.LinkTo = model.LinkTo!;
                page.LinkToUrl = $"/Categories/{model.LinkTo}";

            } // in case LinkTo File Is Selected
            else if (model.LinkToType == "3")
            {
                page.LinkToType = "3";
                page.LinkTo = model.LinkTo!;
                page.LinkToUrl = model.LinkTo!;
            } // in case LinkTo URL Is Selected
            else if (model.LinkToType == "4")
            {
                page.LinkToType = "4";
                page.LinkTo = model.LinkTo!;
                page.LinkToUrl = model.LinkTo!;
            }
            // in case LinkTo Page is selected
            else
            {
                page.LinkToType = "1";
                page.LinkTo = page.Slug;
                page.LinkToUrl = $"/Page/{page.Slug}";
            }

            // Remove old Related categories and Add the new 
            page.RelatedCategories?.Clear();
            //Add related Categoris
            if (model.RelatedCategoryIds.Any())
                await _pagesRepository.AddRelatedCategoriesAsync(page, model.RelatedCategoryIds);

            _pagesRepository.Updatepage(page);

            //  return Json(new { success = true });

            return Json(new { success = true, id = page.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var page = await _pagesRepository.GetPageByIdAsync(id);

            if (page is null)
                return NotFound();

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.PagesOfCategory,
                                           page.CategoryId,
                                           CrudOperations.Delete);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);

            if (!string.IsNullOrEmpty(page.FeatruedImageUrl))
                _imageService.Delete($"/images/Pages/{page.FeatruedImageUrl}");

            if (!string.IsNullOrEmpty(page.CoverImageUrl))
                _imageService.Delete($"/images/Pages/{page.CoverImageUrl}");

            //delete galleryImages
            var galleryImages = await _galleryImagesRepository.GetGalleryImagesOfPageAsync(id);
            if (galleryImages.Any())
            {
                foreach (var image in galleryImages)
                {
                    _imageService.Delete($"/images/Pages/GalleryImages/{image.GalleryImageUrl}");
                }
            }

            //delete files
            var files = await _pageFilesRepository.GetFilesOfPageAsync(id);
            if (files.Any())
            {
                foreach (var file in files)
                {
                    _filesService.Delete($"/documents/Pages/{file.FileUrl}");
                }
            }

            _pagesRepository.Deletepage(page);

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var page = await _pagesRepository.GetPageByIdAsync(id);

            if (page is null)
                return StatusCode(402);

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.PagesOfCategory,
                                           page.CategoryId,
                                           CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);

            page.IsPublished = !page.IsPublished;
            _pagesRepository.Updatepage(page);

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(int id, int order) // Changed parameter name from pageId to id
        {
            var page = await _pagesRepository.GetPageByIdAsync(id);

            if (page is null)
                return StatusCode(402);

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.PagesOfCategory,
                                           page.CategoryId,
                                           CrudOperations.Update);

            page.Order = order;
            _pagesRepository.Updatepage(page);

            return StatusCode(200);
        }
    }
}
