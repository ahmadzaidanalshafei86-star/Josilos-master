using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Core.Enums;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class CategoriesRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<CategoriesViewModel>> GetCategoriesWithParentInfoAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .Select(c => new CategoriesViewModel
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Name = c.Name,
                    ParentCategoryName = c.ParentCategory.Name,
                    CreatedDate = c.CreatedDate,
                    IsPublished = c.IsPublished,

                })
                .ToListAsync();

            return categories;
            //return await _context.CategoryViewModel
            //.FromSqlRaw("EXEC GetCategoriesWithParentInfo")
            //.ToListAsync();

        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.PagesRelatedToThis)
                  .ThenInclude(p => p.PageFiles) // Include PageFiles
                .Include(c => c.PagesRelatedToThis)
                 .ThenInclude(p => p.PageGalleryImages) // Include PageGalleryImages
                .FirstOrDefaultAsync(c => c.Id == id); // Filter by specific category

            if (category == null)
                throw new Exception(message: "Category not found");

            return category;
        }

        public async Task<Category> GetCategoryByIdWithTranslationsAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Language)
                .Include(c => c.CategoryTranslates!)
                .ThenInclude(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                throw new Exception(message: "Category not found");

            return category;
        }

        public async Task<Category> GetCategoryWithAllDataAsync(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.GalleryImages)
                .Include(c => c.RelatedCategories)
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                throw new Exception(message: "Category not found");

            return category;
        }


        public async Task<IEnumerable<SelectListItem>> GetCategoriesSlugsNamesAsync()
        {
            return await _context.Categories
             .Select(pc => new SelectListItem
             {
                 Value = pc.Slug,
                 Text = pc.Name
             })
             .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesNamesAsync()
        {
            return await _context.Categories
             .Select(pc => new SelectListItem
             {
                 Value = pc.Id.ToString(),
                 Text = pc.Name
             })
             .ToListAsync();
        }




        public async Task<int> AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }// Return the ID of the newly created category

        //    var idParameter = new SqlParameter
        //    {
        //        ParameterName = "@Id",
        //        SqlDbType = System.Data.SqlDbType.Int,
        //        Direction = System.Data.ParameterDirection.Output
        //    };

        //    await _context.Database.ExecuteSqlRawAsync(
        //        @"EXEC AddCategory 
        //    @Name, @LongDescription, @ShortDescription, @FeaturedImageUrl, @FeaturedImageAltName,
        //    @CoverImageUrl, @CoverImageAltName, @MetaDescription, @MetaKeywords, @Order, @TypeOfSorting,
        //    @CreatedDate, @ParentCategoryId, @LanguageId, @ThemeId, @IsPublished, @Id OUTPUT",
        //        new SqlParameter("@Name", category.Name),
        //        new SqlParameter("@LongDescription", category.LongDescription ?? (object)DBNull.Value),
        //        new SqlParameter("@ShortDescription", category.ShortDescription ?? (object)DBNull.Value),
        //        new SqlParameter("@FeaturedImageUrl", category.FeaturedImageUrl ?? (object)DBNull.Value),
        //        new SqlParameter("@FeaturedImageAltName", category.FeaturedImageAltName ?? (object)DBNull.Value),
        //        new SqlParameter("@CoverImageUrl", category.CoverImageUrl ?? (object)DBNull.Value),
        //        new SqlParameter("@CoverImageAltName", category.CoverImageAltName ?? (object)DBNull.Value),
        //        new SqlParameter("@MetaDescription", category.MetaDescription ?? (object)DBNull.Value),
        //        new SqlParameter("@MetaKeywords", category.MetaKeywords ?? (object)DBNull.Value),
        //        new SqlParameter("@Order", category.Order),
        //        new SqlParameter("@TypeOfSorting", (int)category.TypeOfSorting),
        //        new SqlParameter("@CreatedDate", category.CreatedDate),
        //        new SqlParameter("@ParentCategoryId", category.ParentCategoryId ?? (object)DBNull.Value),
        //        new SqlParameter("@LanguageId", category.LanguageId),
        //        new SqlParameter("@ThemeId", category.ThemeId),
        //        new SqlParameter("@IsPublished", category.IsPublished),
        //        idParameter
        //    );

        //    return (int)idParameter.Value;
        //}

        public async Task AddRelatedCategoriesAsync(Category category, List<int> relatedCategoryIds)
        {
            foreach (var relatedCategoryId in relatedCategoryIds)
            {
                var relatedCategory = await _context.Categories.FindAsync(relatedCategoryId);
                if (relatedCategory != null)
                {
                    category.RelatedCategories?.Add(relatedCategory);
                }
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }


        public async Task<IEnumerable<SelectListItem>> GetThemesAsync()
        {
            return await _context.Themes
           .Select(th => new SelectListItem
           {
               Value = th.Id.ToString(),
               Text = th.ThemeName
           })
           .OrderBy(th => th.Value)
           .ToListAsync();
        }

        public async Task<CategoryFormViewModel> InitializeCategoryFormViewModelAsync(CategoryFormViewModel? model = null)
        {
            model ??= new CategoryFormViewModel(); // Initialize a new model if none is provided.

            model.SortingTypes = GetSelectListFromEnum<TypeOfSorting>();
            model.GalleryStyles = GetSelectListFromEnum<GalleryStyle>();
            model.Categories = await GetCategoriesNamesAsync();
            model.Themes = await GetThemesAsync();

            return model;
        }

        public bool IsParentCategory(int CategoryId)
        {
            var childCategories = _context.Categories.Where(c => c.ParentCategoryId == CategoryId);
            if (childCategories.Any())
                return true;

            return false;
        }

        public bool IsRelatedToAnotherCategory(int CategoryId)
        {
            var IsRelated = _context.Categories.Any(c => c.RelatedCategories.Any(r => r.Id == CategoryId));
            if (IsRelated)
                return true;

            return false;
        }

        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }

        private List<SelectListItem> GetSelectListFromEnum<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)(object)e).ToString(),
                    Text = e.ToString()
                }).ToList();
        }


    }
}
