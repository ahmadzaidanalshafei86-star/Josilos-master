using ES.Web.Areas.EsAdmin.Models;


namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class MediaManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MediaManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Permissions.MediaManagment.Read)]
        public async Task<IActionResult> Index()
        {
            // Fetch categories
            var categories = await _context.Categories.ToListAsync();

            // Map category media
            var categoryMedia = categories
                .SelectMany(c => new List<MediaViewModel>
                {
            new MediaViewModel
            {
                Id = c.Id,
                CategoryOrPageName = c.Name,
                ImageUrl = !string.IsNullOrEmpty(c.CoverImageUrl) ? $"/images/Categories/{c.CoverImageUrl}" : null,
                AltName = c.CoverImageAltName,
                Type = "Category Cover Images",
                CreatedDate = c.CreatedDate
            },
            new MediaViewModel
            {
                Id = c.Id,
                CategoryOrPageName = c.Name,
                ImageUrl = !string.IsNullOrEmpty(c.FeaturedImageUrl) ? $"/images/Categories/{c.FeaturedImageUrl}" : null,
                AltName = c.FeaturedImageAltName,
                Type = "Category Featured Images",
                CreatedDate = c.CreatedDate
            }
                })
                .Where(m => !string.IsNullOrEmpty(m.ImageUrl))
                .ToList();

            // Fetch category gallery images (GalleryImages)
            var categoryGalleryImages = await _context.GalleryImages.ToListAsync();

            var categoryGalleryMedia = categoryGalleryImages
                .Select(m => new MediaViewModel
                {
                    Id = m.Id,
                    CategoryOrPageName = m.Category.Name,
                    ImageUrl = !string.IsNullOrEmpty(m.GalleryImageUrl) ? $"/images/Categories/GalleryImages/{m.GalleryImageUrl}" : null,
                    AltName = m.AltName,
                    Type = "Category Gallery Images",
                    CreatedDate = m.CreatedDate
                })
                .Where(m => !string.IsNullOrEmpty(m.ImageUrl))
                .ToList();

            // Fetch pages
            var pages = await _context.Pages.ToListAsync();

            var pageMedia = pages
                .SelectMany(p => new List<MediaViewModel>
                {
            new MediaViewModel
            {
                Id = p.Id,
                CategoryOrPageName = p.Title,
                ImageUrl = !string.IsNullOrEmpty(p.CoverImageUrl) ? $"/images/Pages/{p.CoverImageUrl}" : null,
                AltName = p.CoverImageAltName,
                Type = "Page Cover Images",
                CreatedDate = p.CreatedDate
            },
            new MediaViewModel
            {
                Id = p.Id,
                CategoryOrPageName = p.Title,
                ImageUrl = !string.IsNullOrEmpty(p.FeatruedImageUrl) ? $"/images/Pages/{p.FeatruedImageUrl}" : null,
                AltName = p.FeaturedImageAltName,
                Type = "Page Featured Images",
                CreatedDate = p.CreatedDate
            }
                })
                .Where(m => !string.IsNullOrEmpty(m.ImageUrl))
                .ToList();

            // Fetch page gallery images (PageGalleryImages)
            var pageGalleryImages = await _context.PageGalleryImages.ToListAsync();

            var pageGalleryMedia = pageGalleryImages
                .Select(m => new MediaViewModel
                {
                    Id = m.Id,
                    CategoryOrPageName = m.Page.Title,
                    ImageUrl = !string.IsNullOrEmpty(m.GalleryImageUrl) ? $"/images/Pages/GalleryImages/{m.GalleryImageUrl}" : null,
                    AltName = m.AltName,
                    Type = "Page Gallery Images",
                    CreatedDate = m.CreatedDate
                })
                .Where(m => !string.IsNullOrEmpty(m.ImageUrl))
                .ToList();

            // Fetch page files
            var pageFiles = await _context.PageFiles.ToListAsync();

            var pageFileModels = pageFiles
                .Select(f => new MediaViewModel
                {
                    Id = f.Id,
                    CategoryOrPageName = f.Page.Title,
                    ImageUrl = !string.IsNullOrEmpty(f.FileUrl) ? $"/documents/Pages/{f.FileUrl}" : null,
                    AltName = f.AltName,
                    Type = "Page Files",
                    CreatedDate = f.CreatedDate
                })
                .Where(m => !string.IsNullOrEmpty(m.ImageUrl))
                .ToList();

            // Combine all media and order by CreatedDate descending (latest first)
            var mediaItems = categoryMedia
                .Concat(categoryGalleryMedia)
                .Concat(pageMedia)
                .Concat(pageGalleryMedia)
                .Concat(pageFileModels)
                .OrderByDescending(m => m.CreatedDate)
                .ToList();

            return View(mediaItems);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAltName([FromBody] UpdateMediaAltNameViewModel model)
        {
            if (!User.HasClaim("Permission", Permissions.MediaManagment.Update))
                return StatusCode(403);


            if (model == null || string.IsNullOrWhiteSpace(model.AltName))
                return Json(new { success = false, message = "Invalid data" });

            bool updated = false;

            switch (model.Type)
            {
                case "Category Cover Images":
                    var categoryCover = await _context.Categories.FirstOrDefaultAsync(c => c.Id == model.Id);
                    if (categoryCover != null)
                    {
                        categoryCover.CoverImageAltName = model.AltName;
                        updated = true;
                    }
                    break;

                case "Category Featured Images":
                    var categoryFeatured = await _context.Categories.FirstOrDefaultAsync(c => c.Id == model.Id);
                    if (categoryFeatured != null)
                    {
                        categoryFeatured.FeaturedImageAltName = model.AltName;
                        updated = true;
                    }
                    break;

                case "Category Gallery Images":
                    var galleryImage = await _context.GalleryImages.FirstOrDefaultAsync(g => g.Id == model.Id);
                    if (galleryImage != null)
                    {
                        galleryImage.AltName = model.AltName;
                        updated = true;
                    }
                    break;

                case "Page Cover Images":
                    var pageCover = await _context.Pages.FirstOrDefaultAsync(p => p.Id == model.Id);
                    if (pageCover != null)
                    {
                        pageCover.CoverImageAltName = model.AltName;
                        updated = true;
                    }
                    break;

                case "Page Featured Images":
                    var pageFeatured = await _context.Pages.FirstOrDefaultAsync(p => p.Id == model.Id);
                    if (pageFeatured != null)
                    {
                        pageFeatured.FeaturedImageAltName = model.AltName;
                        updated = true;
                    }
                    break;

                case "Page Gallery Images":
                    var PagegalleryImage = await _context.PageGalleryImages.FirstOrDefaultAsync(g => g.Id == model.Id);
                    if (PagegalleryImage != null)
                    {
                        PagegalleryImage.AltName = model.AltName;
                        updated = true;
                    }
                    break;

                case "Page Files":
                    var pageFile = await _context.PageFiles.FirstOrDefaultAsync(f => f.Id == model.Id);
                    if (pageFile != null)
                    {
                        pageFile.AltName = model.AltName;
                        updated = true;
                    }
                    break;

                default:
                    return Json(new { success = false, message = "Invalid media type" });
            }

            if (updated)
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Media not found" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadCroppedImage(IFormFile croppedImage, string originalImageUrl)
        {

            if (!User.HasClaim("Permission", Permissions.MediaManagment.Update))
                return StatusCode(403);

            if (croppedImage == null || string.IsNullOrEmpty(originalImageUrl))
                return Json(new { success = false, message = "Invalid image data." });

            // Get the current domain and scheme (http or https)
            var currentDomain = $"{Request.Scheme}://{Request.Host}";

            // Remove the domain part from the original image URL to get the relative path
            var relativeImageUrl = originalImageUrl.Replace(currentDomain, "");

            // Extract the file name and folder path from the relative URL
            var fileName = Path.GetFileName(relativeImageUrl);  // Get the image file name
            var folderPath = relativeImageUrl.Substring(0, relativeImageUrl.LastIndexOf(fileName)); // Get the folder path

            // Ensure the folder path is relative to the "wwwroot/images" directory
            var relativeFolderPath = Path.Combine("wwwroot", folderPath.TrimStart('/'));

            // Define the full file path to save the cropped image (using the same file name)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativeFolderPath, fileName);

            // Delete the old image if it exists
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Ensure the directory exists or create it
            if (!Directory.Exists(relativeFolderPath))
            {
                Directory.CreateDirectory(relativeFolderPath);
            }

            // Save the cropped image to the same location
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await croppedImage.CopyToAsync(fileStream);
            }

            // Construct the new image URL (relative to your web root)
            var newImageUrl = $"/images/{folderPath.Substring(8)}{fileName}";

            return Json(new { success = true, imageUrl = newImageUrl });
        }


    }
}
