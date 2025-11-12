using System.Text.RegularExpressions;

namespace ES.Web.Areas.EsAdmin.Services
{
    public class SlugService
    {
        private readonly ApplicationDbContext _context;

        public SlugService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Public method: Generate a unique slug for Pages or Categories
        public string GenerateUniqueSlug(string title, string entityType, int? excludeId = null)
        {
            // Generate the base slug from the title
            string slug = GenerateSlug(title);

            // Ensure it's unique by checking the appropriate entity (Pages or Categories)
            return EnsureUniqueSlug(slug, entityType, excludeId);
        }

        private string GenerateSlug(string title)
        {
            // Convert to lowercase and trim whitespace
            string slug = title.ToLower().Trim();

            // Remove special characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            return slug;
        }

        // Private method: Ensure the slug is unique for the specified entity type (Pages or Categories)
        private string EnsureUniqueSlug(string slug, string entityType, int? excludeId)
        {
            string uniqueSlug = slug;
            int count = 1;

            // Check for uniqueness in the appropriate entity type
            if (entityType == "Page")
            {
                while (_context.Pages.Any(p => p.Slug == uniqueSlug && (excludeId == null || p.Id != excludeId)))
                {
                    uniqueSlug = $"{slug}-{count}";
                    count++;
                }
            }
            else if (entityType == "Category")
            {
                while (_context.Categories.Any(c => c.Slug == uniqueSlug && (excludeId == null || c.Id != excludeId)))
                {
                    uniqueSlug = $"{slug}-{count}";
                    count++;
                }
            }
            else if (entityType == "Material")
            {
                while (_context.Materials.Any(c => c.Slug == uniqueSlug && (excludeId == null || c.Id != excludeId)))
                {
                    uniqueSlug = $"{slug}-{count}";
                    count++;
                }
            }

            else if (entityType == "Tender")
            {
                while (_context.Tenders.Any(c => c.Slug == uniqueSlug && (excludeId == null || c.Id != excludeId)))
                {
                    uniqueSlug = $"{slug}-{count}";
                    count++;
                }
            }
            else if (entityType == "EcomCategory")
            {
                while (_context.EcomCategories.Any(ec => ec.Slug == uniqueSlug && (excludeId == null || ec.Id != excludeId)))
                {
                    uniqueSlug = $"{slug}-{count}";
                    count++;
                }
            }
            else if (entityType == "Product")
            {
                while (_context.Products.Any(p => p.Slug == uniqueSlug && (excludeId == null || p.Id != excludeId)))
                {
                    uniqueSlug = $"{slug}-{count}";
                    count++;
                }
            }
            else if (entityType == "Brand")
            {
                while (_context.Brands.Any(p => p.Slug == uniqueSlug && (excludeId == null || p.Id != excludeId)))
                {
                    uniqueSlug = $"{slug}-{count}";
                    count++;
                }
            }
            else
            {
                throw new ArgumentException("Unknown entity type", nameof(entityType));
            }

            return uniqueSlug;
        }
    }

}
