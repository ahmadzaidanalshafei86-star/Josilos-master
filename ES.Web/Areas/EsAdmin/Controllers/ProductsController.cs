using Microsoft.CodeAnalysis;
using ES.Core.Enums;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;
namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class ProductsController : Controller
    {
        private readonly ProductsRepository _productsRepository;
        private readonly GalleryImagesRepository _galleryImagesRepository;
        private readonly ILanguageService _languageService;
        private readonly LanguagesRepository _languagesRepository;
        private readonly SlugService _slugService;
        private readonly IImageService _imageService;


        public ProductsController(ProductsRepository productsRepository,
            SlugService slugService,
            LanguagesRepository languagesRepository,
            ILanguageService languageService,
            IImageService imageService,
            GalleryImagesRepository galleryImagesRepository)
        {
            _productsRepository = productsRepository;
            _slugService = slugService;
            _languagesRepository = languagesRepository;
            _languageService = languageService;
            _imageService = imageService;
            _galleryImagesRepository = galleryImagesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.Products.Read)]
        public async Task<IActionResult> Index()
        {
            var model = await _productsRepository.GetProductsAsync();
            return View(model);
        }

        [HttpGet]
        [Authorize(Permissions.Products.Create)]
        public async Task<IActionResult> Create()
        {
            var model = await _productsRepository.InitializeProductFormViewModelAsync();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Products.Create)]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _productsRepository.InitializeProductFormViewModelAsync();
                return View("Form", model);
            }

            var CurrentLang = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync());
            Product product = new()
            {
                Title = model.Title,
                Slug = _slugService.GenerateUniqueSlug(model.Title, nameof(Product)),
                BrandId = model.BrandId,
                ProductLabelId = model.LabelId,
                ProductCategories = model.CategoryIds
                     .Select(categoryId => new ProductCategory { CategoryId = categoryId })
                     .ToList(),
                LongDescription = model.LongDescription,
                ShortDescription = model.ShortDescription,
                VideoUrl = model.VideoUrl,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords,
                ProductType = model.ProductType == "simple" ? ProductType.Simple : ProductType.Variable,
                RegularPrice = model.RegularPrice,
                SalePrice = model.SalePrice,
                SaleStartDate = model.SaleStartDate,
                SaleEndDate = model.SaleEndDate,
                SKU = model.SKU,
                GTIN = model.GTIN,
                ManageStock = model.ManageStock,
                SoldIndividually = model.SoldIndividually,
                ExcludeSoldOutBadge = model.ExcludeFromSoldOutBadge,
                IsPublished = model.IsPublished,
                DescriptionTab = model.DescriptionTab,
                ReviewTab = model.ReviewTab,
                LanguageId = CurrentLang,
            };

            // Handle Stock Quantity
            if (model.ManageStock)
            {
                product.StockQuantity = model.Quantity;
                product.LowStockThreshold = model.LowStockThreshold ?? 2;
                product.AllowBackorders = model.AllowBackorders ?? BackorderStatus.DoNotAllow;
            }
            else
            {
                product.StockStatus = model.StockStatus;
            }

            var productId = await _productsRepository.AddProductAsync(product);

            //linked products
            if (model.LinkedProductIds?.Any() == true)
                await _productsRepository.AddLinkedProductsAsync(product.Id, model.LinkedProductIds);

            // Handle CoverImage Image
            if (model.CoverImage is not null)
            {
                var CoverImageName = $"{productId}_coverImage{Path.GetExtension(model.CoverImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.CoverImage, CoverImageName, "/images/Products");
                if (isUploaded)
                {
                    product.CoverImageUrl = CoverImageName;
                    product.CoverImageAltName = model.CoverImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                    model = await _productsRepository.InitializeProductFormViewModelAsync();
                    return View("Form", model);
                }
            }

            // Handle FeaturedImage Image
            if (model.FeaturedImage is not null)
            {
                var FeaturedImageName = $"{productId}_FeaturedImage{Path.GetExtension(model.FeaturedImage.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.FeaturedImage, FeaturedImageName, "/images/Products");
                if (isUploaded)
                {
                    product.FeaturedImageUrl = FeaturedImageName;
                    product.FeaturedImageAltName = model.FeaturedImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                    model = await _productsRepository.InitializeProductFormViewModelAsync();
                    return View("Form", model);
                }
            }

            //GalleryImages
            if (model.GalleryImages is not null)
            {
                for (int i = 0; i < model.GalleryImages.Count; i++)
                {
                    ProductGalleryImage productGalleryImage = new();
                    var galleryImageName = $"{productId}_{i}{Path.GetExtension(model.GalleryImages[i].FileName)}";
                    var (isUploaded, errorMessage) = await _imageService.UploadASync(model.GalleryImages[i], galleryImageName, "/images/Products/GalleryImages");
                    if (isUploaded)
                    {
                        productGalleryImage.GalleryImageUrl = galleryImageName;
                        productGalleryImage.AltName = model.GalleryImages[i].FileName;
                        productGalleryImage.ProductId = productId;
                        productGalleryImage.DisplayOrder = i;
                        await _galleryImagesRepository.AddProductGalleryImageAsync(productGalleryImage);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                        model = await _productsRepository.InitializeProductFormViewModelAsync();
                        return View("Form", model);
                    }
                }
            }

            // Product Attributes
            if (model.SelectedAttributes?.Any() == true)
            {
                foreach (var attribute in model.SelectedAttributes)
                {
                    var selectedValues = attribute.Values.Where(v => v.IsSelected).ToList();

                    for (int i = 0; i < selectedValues.Count; i++)
                    {
                        var value = selectedValues[i];

                        ProductAttributeMapping mapping = new()
                        {
                            ProductId = productId,
                            ProductAttributeId = attribute.AttributeId,
                            ProductAttributeValueId = value.ValueId,
                            Order = i + 1,
                            RegularPrice = value.RegualrPrice,
                            SalePrice = value.SalePrice,
                            SaleStartDate = value.SaleStartDate,
                            SaleEndDate = value.SaleEndDate,
                            SKU = value.SKU,
                            StockQuantity = value.StockQuantity,
                        };

                        if (value.Image is not null)
                        {
                            var ProductVariantImageName = $"{productId}_VariantImg_{Guid.NewGuid().ToString("N")[..8]}{Path.GetExtension(value.Image.FileName)}";

                            var (isUploaded, errorMessage) = await _imageService.UploadASync(value.Image, ProductVariantImageName, "/images/Products");
                            if (isUploaded)
                            {
                                mapping.ImageUrl = ProductVariantImageName;
                            }
                            else
                            {
                                ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                                model = await _productsRepository.InitializeProductFormViewModelAsync();
                                return View("Form", model);
                            }
                        }

                        await _productsRepository.AddProductAttributeMapping(mapping);
                    }
                }
            }

            // Tabs
            if (model.ProductTabs is not null)
            {
                foreach (var productTab in model.ProductTabs)
                {
                    ProductTab tab = new()
                    {
                        Title = productTab.Title,
                        Content = productTab.Content,
                        Order = productTab.Order,
                        ProductId = productId,
                        LanguageId = CurrentLang,
                    };
                    await _productsRepository.AddProductTab(tab);
                }

            }

            await _productsRepository.SaveChangesAsync();
            return StatusCode(210);
        }

        [HttpGet]
        [Authorize(Permissions.Products.Update)]
        public async Task<IActionResult> Edit(int productId)
        {
            var product = await _productsRepository.GetFullProductByIdAsync(productId);
            if (product is null)
                return NotFound();

            ProductFormViewModel model = new()
            {
                Id = product.Id,
                Title = product.Title,
                BrandId = product.BrandId,
                LabelId = product.ProductLabelId,
                CategoryIds = product.ProductCategories.Select(c => c.CategoryId).ToList(),
                ShortDescription = product.ShortDescription,
                LongDescription = product.LongDescription,
                CoverImageUrl = product.CoverImageUrl,
                FeaturedImageUrl = product.FeaturedImageUrl,
                VideoUrl = product.VideoUrl,
                MetaDescription = product.MetaDescription,
                MetaKeywords = product.MetaKeywords,
                IsPublished = product.IsPublished,
                ExistingGalleryImages = product.ProductGalleryImages?.Select(g => new GalleryImageViewModel
                {
                    GalleryImageUrl = g.GalleryImageUrl,
                    GalleryImageAltName = g.AltName,
                    DisplayOrder = g.DisplayOrder,
                })
                .OrderBy(g => g.DisplayOrder)
                .ToList(),
                RegularPrice = product.RegularPrice,
                SalePrice = product.SalePrice,
                SaleStartDate = product.SaleStartDate,
                SaleEndDate = product.SaleEndDate,
                SKU = product.SKU,
                GTIN = product.GTIN,
                ManageStock = product.ManageStock,
                StockStatus = product.StockStatus,
                Quantity = product.StockQuantity,
                LowStockThreshold = product.LowStockThreshold,
                AllowBackorders = product.AllowBackorders,
                SoldIndividually = product.SoldIndividually,
                ExcludeFromSoldOutBadge = product.ExcludeSoldOutBadge,
                LinkedProductIds = product.LinkedProducts.Select(lp => lp.LinkedProductId).ToList(),
                DescriptionTab = product.DescriptionTab,
                ReviewTab = product.ReviewTab,
                ProductTabs = product.ProductTabs.Select(pt => new ProductTabViewModel
                {
                    Id = pt.Id,
                    Title = pt.Title,
                    Content = pt.Content,
                    Order = pt.Order,
                })
                .OrderBy(pt => pt.Order)
                .ToList(),
                ProductType = product.ProductType == ProductType.Simple ? "simple" : "variable",
            };
            model = await _productsRepository.InitializeProductFormViewModelAsync(model, productId);

            return View("Form", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Products.Update)]
        public async Task<IActionResult> Edit(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _productsRepository.InitializeProductFormViewModelAsync(model, model.Id);
                return View("Form", model);
            }

            var product = await _productsRepository.GetFullProductByIdAsync(model.Id);
            if (product == null)
                return NotFound();

            // Handle CoverImage Image
            if (model.CoverImage is not null)
            {
                // Delete old cover image if it exists
                if (!string.IsNullOrEmpty(product.CoverImageUrl))
                    _imageService.Delete($"/images/Products/{product.CoverImageUrl}");

                var CoverImageName = $"{product.Id}_coverImage{Path.GetExtension(model.CoverImage.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.CoverImage, CoverImageName, "/images/Products");

                if (isUploaded)
                {
                    product.CoverImageUrl = CoverImageName;
                    product.CoverImageAltName = model.CoverImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                    model = await _productsRepository.InitializeProductFormViewModelAsync(model, product.Id);
                    return View("Form", model);
                }
            }
            else if (!string.IsNullOrEmpty(model.CoverImageUrl) && !model.KeepCoverImage)
            {
                _imageService.Delete($"/images/Products/{product.CoverImageUrl}");
                product.CoverImageAltName = null;
                product.CoverImageUrl = null;
            }

            // Handle Featured Image
            if (model.FeaturedImage is not null)
            {
                // Delete old featured image if it exists
                if (!string.IsNullOrEmpty(product.FeaturedImageUrl))
                    _imageService.Delete($"/images/Products/{product.FeaturedImageUrl}");

                var FeaturedImageName = $"{product.Id}_FeaturedImage{Path.GetExtension(model.FeaturedImage.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.FeaturedImage, FeaturedImageName, "/images/Products");

                if (isUploaded)
                {
                    product.FeaturedImageUrl = FeaturedImageName;
                    product.FeaturedImageAltName = model.FeaturedImage.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                    model = await _productsRepository.InitializeProductFormViewModelAsync(model, product.Id);
                    return View("Form", model);
                }
            }
            else if (!string.IsNullOrEmpty(model.FeaturedImageUrl) && !model.KeepFeaturedImage)
            {
                _imageService.Delete($"/images/Products/{product.FeaturedImageUrl}");
                product.FeaturedImageAltName = null;
                product.FeaturedImageUrl = null;
            }

            //get the product old gallery images
            var ProductOldGalleryImages = await _galleryImagesRepository.GetGalleryImagesOfProductAsync(product.Id);

            //delete old galleryImages
            if (ProductOldGalleryImages.Count > 0)
            {
                _galleryImagesRepository.DeleteRangeProductGalleryImages(ProductOldGalleryImages); //delete them from the database
                //Delete all of them from the server if exits
                foreach (var gallerImage in ProductOldGalleryImages)
                {
                    _imageService.Delete($"/images/Products/GalleryImages/{gallerImage.GalleryImageUrl}");
                }
            }
            // Add the new Gallery Images
            if (model.GalleryImages is not null)
            {
                for (int i = 0; i < model.GalleryImages.Count; i++)
                {
                    ProductGalleryImage ProductGalleryImg = new();
                    var galleryImageName = $"{product.Id}_{i}{Path.GetExtension(model.GalleryImages[i].FileName)}";
                    var (isUploaded, errorMessage) = await _imageService.UploadASync(model.GalleryImages[i], galleryImageName, "/images/Products/GalleryImages");
                    if (isUploaded)
                    {
                        ProductGalleryImg.GalleryImageUrl = galleryImageName;
                        ProductGalleryImg.AltName = model.GalleryImages[i].FileName;
                        ProductGalleryImg.ProductId = product.Id;
                        ProductGalleryImg.DisplayOrder = i;
                        await _galleryImagesRepository.AddProductGalleryImageAsync(ProductGalleryImg);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.FeaturedImage), errorMessage!);
                        model = await _productsRepository.InitializeProductFormViewModelAsync(model, product.Id);
                        return View("Form", model);
                    }
                }
            }

            if (product.Title != model.Title)
                product.Slug = _slugService.GenerateUniqueSlug(model.Title, nameof(Product), product.Id);

            product.Title = model.Title;
            product.ProductCategories = model.CategoryIds
                     .Select(categoryId => new ProductCategory { CategoryId = categoryId })
                     .ToList();
            product.BrandId = model.BrandId;
            product.ProductLabelId = model.LabelId;
            product.ShortDescription = model.ShortDescription;
            product.LongDescription = model.LongDescription;
            product.VideoUrl = model.VideoUrl;
            product.MetaDescription = model.MetaDescription;
            product.MetaKeywords = model.MetaKeywords;
            product.ProductType = model.ProductType == "simple" ? ProductType.Simple : ProductType.Variable;
            product.RegularPrice = model.RegularPrice;
            product.SalePrice = model.SalePrice;
            product.SaleStartDate = model.SaleStartDate;
            product.SaleEndDate = model.SaleEndDate;
            product.SKU = model.SKU;
            product.GTIN = model.GTIN;
            product.ManageStock = model.ManageStock;
            product.SoldIndividually = model.SoldIndividually;
            product.ExcludeSoldOutBadge = model.ExcludeFromSoldOutBadge;
            product.IsPublished = model.IsPublished;
            product.DescriptionTab = model.DescriptionTab;
            product.ReviewTab = model.ReviewTab;

            // Handle Stock Quantity
            if (model.ManageStock)
            {
                product.StockQuantity = model.Quantity;
                product.LowStockThreshold = model.LowStockThreshold ?? 2;
                product.AllowBackorders = model.AllowBackorders ?? BackorderStatus.DoNotAllow;
            }
            else
            {
                product.StockStatus = model.StockStatus;
            }

            // Handle Product Tabs
            if (model.ProductTabs is not null)
            {
                var existingTabs = product.ProductTabs.ToList();
                var updatedTabIds = model.ProductTabs.Where(t => t.Id > 0).Select(t => t.Id).ToList();

                // Remove tabs that are no longer in the model
                var tabsToRemove = existingTabs.Where(pt => !updatedTabIds.Contains(pt.Id)).ToList();
                foreach (var tab in tabsToRemove)
                {
                    _productsRepository.RemoveProductTab(tab);
                }

                foreach (var productTab in model.ProductTabs)
                {
                    if (productTab.Id > 0)
                    {
                        // Update existing tab
                        var existingTab = existingTabs.FirstOrDefault(pt => pt.Id == productTab.Id);
                        if (existingTab != null)
                        {
                            existingTab.Title = productTab.Title;
                            existingTab.Content = productTab.Content;
                            existingTab.Order = productTab.Order;
                        }
                    }
                    else
                    {
                        // Add new tab
                        ProductTab newTab = new()
                        {
                            Title = productTab.Title,
                            Content = productTab.Content,
                            Order = productTab.Order,
                            ProductId = product.Id,
                            LanguageId = product.LanguageId,
                        };
                        await _productsRepository.AddProductTab(newTab);
                    }
                }
            }

            // Handle Linked products
            await _productsRepository.UpdateLinkedProductsAsync(product.Id, model.LinkedProductIds);

            //handle attributes
            var existingMappings = await _productsRepository.GetProductAttributeMappingsByProductId(product.Id);

            // Remove attributes that are no longer selected
            var removedMappings = existingMappings
                .Where(m => !model.SelectedAttributes.Any(a =>
                    a.Values.Any(v => v.IsSelected && v.ValueId == m.ProductAttributeValueId)))
                .ToList();

            foreach (var mapping in removedMappings)
            {
                // Delete associated image if it exists
                if (!string.IsNullOrEmpty(mapping.ImageUrl))
                {
                    _imageService.Delete($"/images/Products/{mapping.ImageUrl}");
                }

                _productsRepository.RemoveProductAttributeMapping(mapping);
            }

            // Add or update attributes
            foreach (var attribute in model.SelectedAttributes)
            {
                foreach (var value in attribute.Values)
                {
                    var existingMapping = existingMappings
                        .FirstOrDefault(m => m.ProductAttributeValueId == value.ValueId);

                    if (value.IsSelected)
                    {
                        if (existingMapping == null)
                        {
                            // Add new mapping
                            existingMapping = new ProductAttributeMapping
                            {
                                ProductId = product.Id,
                                ProductAttributeId = attribute.AttributeId,
                                ProductAttributeValueId = value.ValueId
                            };
                            await _productsRepository.AddProductAttributeMapping(existingMapping);
                        }

                        // Update values
                        existingMapping.RegularPrice = value.RegualrPrice;
                        existingMapping.SalePrice = value.SalePrice;
                        existingMapping.SaleStartDate = value.SaleStartDate;
                        existingMapping.SaleEndDate = value.SaleEndDate;
                        existingMapping.SKU = value.SKU;
                        existingMapping.StockQuantity = value.StockQuantity;

                        // Handle Image Upload
                        if (value.Image is not null)
                        {
                            var productVariantImageName = $"{product.Id}_VariantImg_{Guid.NewGuid().ToString("N")[..8]}{Path.GetExtension(value.Image.FileName)}";

                            var (isUploaded, errorMessage) = await _imageService.UploadASync(value.Image, productVariantImageName, "/images/Products");
                            if (isUploaded)
                            {
                                // Delete old image if exists
                                if (!string.IsNullOrEmpty(existingMapping.ImageUrl))
                                {
                                    _imageService.Delete($"/images/Products/{existingMapping.ImageUrl}");
                                }

                                existingMapping.ImageUrl = productVariantImageName;
                            }
                            else
                            {
                                ModelState.AddModelError(nameof(model.CoverImage), errorMessage!);
                                model = await _productsRepository.InitializeProductFormViewModelAsync(model, product.Id);
                                return View("Form", model);
                            }
                        }
                        else if (!string.IsNullOrEmpty(value.ImageUrl) && !value.KeepImage)
                        {
                            _imageService.Delete($"/images/Products/{existingMapping.ImageUrl}");
                            existingMapping.ImageUrl = null;
                        }
                    }
                    else if (existingMapping != null)
                    {
                        // Remove image when value is unselected
                        if (!string.IsNullOrEmpty(existingMapping.ImageUrl))
                        {
                            _imageService.Delete($"/images/Products/{existingMapping.ImageUrl}");
                            existingMapping.ImageUrl = null;
                        }
                    }
                }
            }

            await _productsRepository.SaveChangesAsync();
            return StatusCode(210);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Products.Delete))
                return StatusCode(403);

            var product = await _productsRepository.GetProductByIdAsync(id);

            if (product is null)
                return NotFound();

            await _productsRepository.DeleteAsync(product);

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Products.Update))
                return StatusCode(403);

            var product = await _productsRepository.GetProductByIdAsync(id);

            if (product is null)
                return StatusCode(402);

            product.IsPublished = !product.IsPublished;

            await _productsRepository.SaveChangesAsync();

            return StatusCode(200);
        }


        // used in linked products
        [HttpGet]
        public async Task<JsonResult> GetProductsByCategory(int categoryId)
        {
            var products = categoryId == 0
         ? await _productsRepository.GetAllProductsAsync()
         : await _productsRepository.GetProductsByCategoryIdAsync(categoryId);

            var result = products.Select(p => new
            {
                id = p.Id,
                name = p.Title
            });

            return Json(result);
        }

        // used in linked products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productsRepository.GetAllProductsAsync();

            return Json(products);
        }


        [HttpGet]
        [Authorize(Permissions.Products.Read)]
        public async Task<IActionResult> ViewReviews(int productId, int page = 1, int pageSize = 6, string search = "",
           string sort = "", bool showPublished = false)
        {
            var (reviews, totalCount) = await _productsRepository.GetReviewsByProductIdAsync(productId, page, pageSize, search, sort,
                showPublished);
            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ProductId = productId;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.ShowPublished = showPublished;
            return View(reviews);
        }

        [HttpPost]
        [Authorize(Permissions.Products.Update)]
        public async Task<IActionResult> BulkAction(int productId, int[] selectedIds, string action)
        {
            if (selectedIds?.Length > 0)
            {
                switch (action)
                {
                    case "toggle-publish":
                        await _productsRepository.TogglePublishAsyncAsync(selectedIds);
                        break;
                }
            }
            return RedirectToAction("ViewReviews", new { productId });
        }
    }
}
