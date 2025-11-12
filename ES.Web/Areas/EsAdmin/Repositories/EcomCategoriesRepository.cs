using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class EcomCategoriesRepository
    {
        private readonly ApplicationDbContext _context;

        public EcomCategoriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<EcomCategory> GetCategoryByIdAsync(int id)
        {
            var category = await _context.EcomCategories
                 .Include(c => c.ParentCategory)
                 .FirstOrDefaultAsync(c => c.Id == id);

            return category;
        }

        public async Task<IEnumerable<EcomCategoryViewModel>> GetCategories()
        {
            return await _context.EcomCategories
                .Select(c => new EcomCategoryViewModel
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Name = c.Name,
                    FeaturedImageUrl = c.FeaturedImageUrl,
                    FeaturedImageAltName = c.FeaturedImageAltName,
                    ParentCategoryName = c.ParentCategory.Name,
                    CreatedDate = c.CreatedDate,
                    IsPublished = c.IsPublished
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesNamesAsync()
        {
            var categories = await _context.EcomCategories
                .Include(c => c.ChildCategories) // Include child categories
                .ToListAsync();

            var categoryList = new List<SelectListItem>();

            // Find root categories (those without a parent)
            var rootCategories = categories.Where(c => c.ParentCategoryId == null).ToList();

            // Recursively process categories
            foreach (var category in rootCategories)
            {
                AddCategoryWithChildren(category, categoryList, categories, 0);
            }

            return categoryList;
        }
        private void AddCategoryWithChildren(EcomCategory category, List<SelectListItem> categoryList,
                                             List<EcomCategory> allCategories, int level)
        {
            // Use &nbsp; for non-breaking spaces in HTML
            string prefix = new string('\u00A0', level * 4) + "▪ "; // Each level indents by 4 non-breaking spaces

            categoryList.Add(new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = prefix + category.Name
            });

            var children = allCategories.Where(c => c.ParentCategoryId == category.Id).ToList();

            foreach (var child in children)
            {
                AddCategoryWithChildren(child, categoryList, allCategories, level + 1);
            }
        }




        public async Task<EcomCategory> GetCategoryWithTranslationAsync(int id)
        {
            var category = await _context.EcomCategories
                .Include(c => c.Language)
                .Include(c => c.CategoryTranslates!)
                .ThenInclude(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == id);

            return category;
        }

        public async Task<int> AddCategoryAsync(EcomCategory category)
        {
            await _context.EcomCategories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task UpdateCategory(EcomCategory category)
        {
            _context.EcomCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public bool IsParentCategory(int CategoryId)
        {
            var childCategories = _context.EcomCategories.Where(c => c.ParentCategoryId == CategoryId);
            if (childCategories.Any())
                return true;

            return false;
        }

        public async Task DeleteCategory(EcomCategory ecomCategory)
        {
            _context.EcomCategories.Remove(ecomCategory);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesSlugesAsync()
        {
            var categories = await _context.EcomCategories
               .Include(c => c.ChildCategories) // Include child categories
               .ToListAsync();

            var categoryList = new List<SelectListItem>();

            // Find root categories (those without a parent)
            var rootCategories = categories.Where(c => c.ParentCategoryId == null).ToList();

            // Recursively process categories
            foreach (var category in rootCategories)
            {
                AddCategoryWithChildrenForSlug(category, categoryList, categories, 0);
            }

            return categoryList;
        }

        private void AddCategoryWithChildrenForSlug(EcomCategory category, List<SelectListItem> categoryList,
                                        List<EcomCategory> allCategories, int level)
        {
            // Use &nbsp; for non-breaking spaces in HTML
            string prefix = new string('\u00A0', level * 4) + "▪ "; // Each level indents by 4 non-breaking spaces

            categoryList.Add(new SelectListItem
            {
                Value = category.Slug,
                Text = prefix + category.Name
            });

            var children = allCategories.Where(c => c.ParentCategoryId == category.Id).ToList();

            foreach (var child in children)
            {
                AddCategoryWithChildrenForSlug(child, categoryList, allCategories, level + 1);
            }
        }
    }
}

