using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class ProductsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EcomCategoriesRepository _ecomcategoriesRepository;
        private readonly IImageService _imageService;
        private readonly GalleryImagesRepository _galleryImagesRepository;

        public ProductsRepository(ApplicationDbContext context,
           EcomCategoriesRepository ecomcategoriesRepository,
            IImageService imageService, GalleryImagesRepository galleryImagesRepository)
        {
            _context = context;
            _ecomcategoriesRepository = ecomcategoriesRepository;
            _imageService = imageService;
            _galleryImagesRepository = galleryImagesRepository;
        }
        public async Task<Product?> GetProductWithTranslationsAsync(int productId)
        {
            var product = await _context.Products
                 .Include(p => p.Language)
                 .Include(p => p.ProductTranslates!)
                     .ThenInclude(pt => pt.Language)
                 .SingleOrDefaultAsync(p => p.Id == productId);

            return product;
        }
        public async Task<Product?> GetProductWithTabsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.ProductTabs)
                .SingleOrDefaultAsync(p => p.Id == productId);
        }
        public async Task<IEnumerable<ProductViewModel>> GetProductsAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Select(p => new ProductViewModel()
                {
                    Id = p.Id,
                    Slug = p.Slug,
                    Title = p.Title,
                    FeaturedImageUrl = p.FeaturedImageUrl,
                    FeaturedImageAltName = p.FeaturedImageAltName,
                    CategoryNames = p.ProductCategories.Select(pc => pc.Category.Name).ToList(),
                    IsPublished = p.IsPublished,
                    CreatedDate = p.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<ProductFormViewModel> InitializeProductFormViewModelAsync(ProductFormViewModel? model = null, int? productId = null)
        {
            model ??= new ProductFormViewModel(); // Initialize model if null

            // Load Categories
            model.Categories = await _ecomcategoriesRepository.GetCategoriesNamesAsync();

            // Load Brands
            model.Brands = await _context.Brands
                .AsNoTracking()
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToListAsync();

            //Load Labels
            model.Labels = await _context.ProductLabels
                .AsNoTracking()
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                })
                .ToListAsync();

            // Load Available Products for linking
            model.AvailableProducts = await _context.Products
                .AsNoTracking()
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Title
                })
                .ToListAsync();

            // Load Available Attributes
            var availableAttributes = await _context.ProductAttributes
                .AsNoTracking()
                .Select(a => new ProductAttributeViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Values = a.Values
                        .Select(v => new ProductAttributeValueViewModel
                        {
                            Id = v.Id,
                            Value = v.Value,
                            Order = v.Order
                        })
                        .OrderBy(v => v.Order)
                        .ToList()
                })
                .ToListAsync();

            model.AvailableAttributes = availableAttributes;

            // If we are editing an existing product, load its saved attributes
            if (productId.HasValue)
            {
                var selectedAttributes = await _context.ProductAttributeMappings
                    .Where(m => m.ProductId == productId)
                    .Include(m => m.ProductAttribute)
                    .Include(m => m.ProductAttributeValue)
                    .ToListAsync();

                model.SelectedAttributes = availableAttributes
                    .Select(a => new ProductAttributeMappingViewModel
                    {
                        AttributeId = a.Id,
                        AttributeName = a.Name,
                        Values = a.Values
                            .Select(v =>
                            {
                                var savedValue = selectedAttributes
                                    .FirstOrDefault(m => m.ProductAttributeValueId == v.Id);

                                return new ProductAttributeValueFormModel
                                {
                                    ValueId = v.Id,
                                    ValueName = v.Value,
                                    IsSelected = savedValue != null,
                                    RegualrPrice = savedValue?.RegularPrice,
                                    SalePrice = savedValue?.SalePrice,
                                    SaleStartDate = savedValue?.SaleStartDate,
                                    SaleEndDate = savedValue?.SaleEndDate,
                                    SKU = savedValue?.SKU,
                                    StockQuantity = savedValue?.StockQuantity,
                                    ImageUrl = savedValue?.ImageUrl,
                                    KeepImage = !string.IsNullOrEmpty(savedValue?.ImageUrl)
                                };
                            })
                            .ToList()
                    })
                    .ToList();
            }
            else
            {
                // If creating a new product, initialize SelectedAttributes with empty selections
                model.SelectedAttributes = availableAttributes
                    .Select(a => new ProductAttributeMappingViewModel
                    {
                        AttributeId = a.Id,
                        AttributeName = a.Name,
                        Values = a.Values
                            .Select(v => new ProductAttributeValueFormModel
                            {
                                ValueId = v.Id,
                                ValueName = v.Value,
                                IsSelected = false
                            })
                            .ToList()
                    })
                    .ToList();
            }

            return model;
        }

        public async Task<int> AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }
        public async Task<Product?> GetFullProductByIdAsync(int id)
        {
            return await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductGalleryImages)
                .Include(p => p.LinkedProducts)
                .Include(p => p.ProductTabs)
                .Include(p => p.ProductAttributes)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            if (!string.IsNullOrEmpty(product.FeaturedImageUrl))
                _imageService.Delete($"/images/Products/{product.FeaturedImageUrl}");

            if (!string.IsNullOrEmpty(product.CoverImageUrl))
                _imageService.Delete($"/images/Products/{product.CoverImageUrl}");

            // Delete gallery images
            var galleryImages = await _galleryImagesRepository.GetGalleryImagesOfProductAsync(product.Id);
            if (galleryImages.Any())
            {
                foreach (var image in galleryImages)
                    _imageService.Delete($"/images/Products/GalleryImages/{image.GalleryImageUrl}");
            }

            // Remove links where this product is the primary product
            var linkedProducts = await _context.ProductLinks
                .Where(pl => pl.ProductId == product.Id || pl.LinkedProductId == product.Id)
                .ToListAsync();

            if (linkedProducts.Any())
                _context.ProductLinks.RemoveRange(linkedProducts);

            //Remove Attribute images
            var ProductAttributes = await _context.ProductAttributeMappings
                .Where(pm => pm.ProductId == product.Id)
                .ToListAsync();

            foreach (var attribute in ProductAttributes)
                if (attribute.ImageUrl is not null)
                    _imageService.Delete($"/images/Products/{attribute.ImageUrl}");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId))
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task AddLinkedProductsAsync(int productId, List<int> linkedProductIds)
        {
            if (linkedProductIds == null || linkedProductIds.Count == 0)
                return;

            var existingLinks = await _context.ProductLinks
                .Where(pl => pl.ProductId == productId && linkedProductIds.Contains(pl.LinkedProductId))
                .Select(pl => pl.LinkedProductId)
                .ToListAsync();

            var newLinks = linkedProductIds
                .Except(existingLinks) // Avoid inserting duplicates
                .Select(linkedProductId => new ProductLinked
                {
                    ProductId = productId,
                    LinkedProductId = linkedProductId
                })
                .ToList();

            if (newLinks.Count > 0)
            {
                await _context.ProductLinks.AddRangeAsync(newLinks);
            }
        }
        public async Task UpdateLinkedProductsAsync(int productId, List<int> linkedProductIds)
        {
            var existingLinks = await _context.ProductLinks
                .Where(pl => pl.ProductId == productId)
                .ToListAsync();

            var existingLinkedIds = existingLinks.Select(pl => pl.LinkedProductId).ToList();

            var newLinkedIds = linkedProductIds.Except(existingLinkedIds).ToList();
            var removedLinkedIds = existingLinkedIds.Except(linkedProductIds).ToList();

            if (removedLinkedIds.Any())
            {
                var linksToRemove = existingLinks.Where(pl => removedLinkedIds.Contains(pl.LinkedProductId)).ToList();
                _context.ProductLinks.RemoveRange(linksToRemove);
            }

            if (newLinkedIds.Any())
            {
                var newLinks = newLinkedIds.Select(linkedId => new ProductLinked
                {
                    ProductId = productId,
                    LinkedProductId = linkedId
                }).ToList();

                await _context.ProductLinks.AddRangeAsync(newLinks);
            }
        }

        // used in deleting category
        public async Task<List<EcomCategory>> GetProductCategoriesAsync(int productId)
        {
            return await _context.ProductCategories
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.Category)
                .ToListAsync();
        }
        public async Task RemoveProductFromCategoryAsync(int productId, int categoryId)
        {
            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(pc => pc.ProductId == productId && pc.CategoryId == categoryId);

            if (productCategory != null)
            {
                _context.ProductCategories.Remove(productCategory);
                await _context.SaveChangesAsync();
            }
        }

        //Used in product Attribute mapping
        public async Task AddProductAttributeMapping(ProductAttributeMapping mapping)
        {
            await _context.ProductAttributeMappings.AddAsync(mapping);
        }
        public async Task<List<ProductAttributeMapping>> GetProductAttributeMappingsByProductId(int productId)
        {
            return await _context.ProductAttributeMappings
                .Where(m => m.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductFormViewModel> PopulateSelectedAttributesAsync(ProductFormViewModel model, int productId)
        {
            // Fetch attribute mappings for the given product
            var attributeMappings = await _context.ProductAttributeMappings
                .Where(m => m.ProductId == productId)
                .Include(m => m.ProductAttribute)
                .Include(m => m.ProductAttributeValue)
                .ToListAsync();

            // Group attribute mappings by AttributeId
            model.SelectedAttributes = attributeMappings
                .GroupBy(m => m.ProductAttributeId)
                .Select(group => new ProductAttributeMappingViewModel
                {
                    AttributeId = group.Key,
                    AttributeName = group.First().ProductAttribute.Name,
                    Values = group.Select(m => new ProductAttributeValueFormModel
                    {
                        ValueId = m.ProductAttributeValueId,
                        ValueName = m.ProductAttributeValue.Value,
                        IsSelected = true,
                        RegualrPrice = m.RegularPrice,
                        SalePrice = m.SalePrice,
                        SaleStartDate = m.SaleStartDate,
                        SaleEndDate = m.SaleEndDate,
                        SKU = m.SKU,
                        StockQuantity = m.StockQuantity,
                        ImageUrl = m.ImageUrl,
                        KeepImage = !string.IsNullOrEmpty(m.ImageUrl)
                    }).ToList()
                }).ToList();

            return model;
        }
        public void RemoveProductAttributeMapping(ProductAttributeMapping mapping)
        {
            _context.ProductAttributeMappings.Remove(mapping);
        }

        public async Task AddProductTab(ProductTab tab)
        {
            await _context.ProductTabs.AddAsync(tab);
        }
        public void RemoveProductTab(ProductTab tab)
        {
            _context.ProductTabs.Remove(tab);
        }

        public async Task<(IEnumerable<ProductReviewViewModel>, int)> GetReviewsByProductIdAsync(int productId, int page, int pageSize, string search
         , string sort, bool showPublished)
        {
            IQueryable<ProductReview> query = _context.ProductReviews
                .Where(ca => ca.ProductId == productId);

            if (showPublished)
                query = query.Where(ca => ca.Published);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(ca => ca.FormResponse != null &&
                                          ca.FormResponse.ResponseDetails != null &&
                                          ca.FormResponse.ResponseDetails
                                          .Any(rd => rd.ResponseValue != null
                                          && EF.Functions.Like(rd.ResponseValue.ToLower(), $"%{search.ToLower()}%")));

            // Sorting
            if (sort == "date-asc")
                query = query.OrderBy(ca => ca.FormResponse != null ? ca.FormResponse.SubmittedAt : DateTime.MinValue);
            else if (sort == "date-desc")
                query = query.OrderByDescending(ca => ca.FormResponse != null ? ca.FormResponse.SubmittedAt : DateTime.MinValue);
            else
                query = query.OrderByDescending(ca => ca.FormResponse != null ? ca.FormResponse.SubmittedAt : DateTime.MinValue);

            // Get total count before pagination
            int totalCount = await query.CountAsync();

            // Apply Includes after filtering and sorting
            query = query
                .Include(ca => ca.Product)
                .Include(ca => ca.FormResponse)
                    .ThenInclude(fr => fr.ResponseDetails)
                        .ThenInclude(rd => rd.Field);

            // Apply pagination and projection
            var reviews = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ca => new ProductReviewViewModel
                {
                    ReviewId = ca.Id,
                    SubmittedAt = ca.FormResponse != null ? ca.FormResponse.SubmittedAt : DateTime.MinValue,
                    ProductName = ca.Product.Title,
                    IsPublished = ca.Published,
                    Details = ca.FormResponse != null && ca.FormResponse.ResponseDetails != null
                        ? ca.FormResponse.ResponseDetails.Select(rd => new ApplicationDetailViewModel
                        {
                            FieldName = rd.Field.FieldName,
                            FieldType = rd.Field.FieldType,
                            ResponseValue = rd.ResponseValue,
                            FileUrl = rd.Field.FieldType == "file" ? rd.ResponseValue : null
                        }).ToList()
                        : new List<ApplicationDetailViewModel>()
                })
                .ToListAsync();

            return (reviews, totalCount);
        }

        public async Task TogglePublishAsyncAsync(int[] reviewsIds)
        {
            var reviews = await _context.ProductReviews
                .Where(ca => reviewsIds.Contains(ca.Id))
                .ToListAsync();
            foreach (var rev in reviews)
                rev.Published = !rev.Published;
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
