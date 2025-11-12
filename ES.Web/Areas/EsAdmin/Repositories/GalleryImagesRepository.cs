namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class GalleryImagesRepository
    {
        private readonly ApplicationDbContext _context;
        public GalleryImagesRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        // Category GalleryImage {
        public async Task<int> AddGalleryImageAsync(GalleryImage image)
        {
            await _context.GalleryImages.AddAsync(image);
            await _context.SaveChangesAsync();
            return image.Id;
        }


        public void DeleteRangeGalleryImages(IList<GalleryImage> images)
        {
            _context.GalleryImages.RemoveRange(images);
            _context.SaveChanges();
        }

        public async Task<IList<GalleryImage>> GetGalleryImagesOfCategoryAsync(int categoryId)
        {
            return await _context.GalleryImages.Where(gi => gi.CategoryId == categoryId).ToListAsync();
        }

        // }

        // Page GallleryImages {
        public async Task<int> AddPageGalleryImageAsync(PageGalleryImage image)
        {
            await _context.PageGalleryImages.AddAsync(image);
            await _context.SaveChangesAsync();
            return image.Id;
        }

        public async Task<IList<PageGalleryImage>> GetGalleryImagesOfPageAsync(int pageId)
        {
            return await _context.PageGalleryImages.Where(gi => gi.PageId == pageId).ToListAsync();
        }

        public void DeleteRangePageGalleryImages(IList<PageGalleryImage> images)
        {
            _context.PageGalleryImages.RemoveRange(images);
            _context.SaveChanges();
        }
        // }

        // Product GallleryImages {
        public async Task<int> AddProductGalleryImageAsync(ProductGalleryImage image)
        {
            await _context.ProductGalleryImages.AddAsync(image);
            return image.Id;
        }

        public async Task<IList<ProductGalleryImage>> GetGalleryImagesOfProductAsync(int ProductId)
        {
            return await _context.ProductGalleryImages.Where(gi => gi.ProductId == ProductId).ToListAsync();
        }

        public void DeleteRangeProductGalleryImages(IList<ProductGalleryImage> images)
        {
            _context.ProductGalleryImages.RemoveRange(images);
        }
        // }
    }
}
