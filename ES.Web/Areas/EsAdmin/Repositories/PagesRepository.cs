using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using ES.Core.Enums;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class PagesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly CategoriesRepository _categoriesRepository;
        private readonly FormRepository _formsrepository;
        public PagesRepository(ApplicationDbContext context, CategoriesRepository categoriesRepository, FormRepository formsrepository)
        {
            _context = context;
            _categoriesRepository = categoriesRepository;
            _formsrepository = formsrepository;
        }
        public async Task<Page> GetPageByIdAsync(int id)
        {
            var page = await _context.Pages
                        .Include(p => p.RelatedCategories)
                        .SingleOrDefaultAsync(p => p.Id == id);
            return page;

        }
        public async Task<Page> GetPageByIdWithTranslationsAsync(int pageId)
        {
            var page = await _context.Pages
                 .Include(c => c.Language)
                 .Include(c => c.PageTranslates!)
                 .ThenInclude(c => c.Language)
                 .SingleOrDefaultAsync(c => c.Id == pageId);

            if (page == null)
                throw new Exception(message: "Page not found");

            return page;
        }


        public async Task<Page> GetPageWithGalleryImagesAsync(int id)
        {
            var page = await _context.Pages
                        .Include(p => p.PageGalleryImages)
                        .Include(p => p.PageFiles)
                        .Include(p => p.RelatedCategories)
                        .SingleOrDefaultAsync(p => p.Id == id);
            return page;
        }

        public async Task<IEnumerable<PageViewModel>> GetAllPagesWithCategoryNameAsync()
        {
            return await _context.Pages
                        .Include(p => p.Category)
                        .Select(p => new PageViewModel
                        {
                            Id = p.Id,
                            Slug = p.Slug,
                            Title = p.Title,
                            CreatedDate = p.CreatedDate,
                            Order = p.Order,
                            IsPublished = p.IsPublished,
                            CategoryName = p.Category!.Name
                        })
                        .ToListAsync();
        }

        public async Task<int> AddPageAsync(Page page)
        {
            await _context.Pages.AddAsync(page);
            await _context.SaveChangesAsync();
            return page.Id;
        }

        public async Task AddRelatedCategoriesAsync(Page page, List<int> relatedCategoryIds)
        {

            var categories = await _context.Categories
                .Where(c => relatedCategoryIds.Contains(c.Id))
                .ToListAsync();

            var pageCategories = categories.Select(category => new PageCategory
            {
                PageId = page.Id,
                CategoryId = category.Id
            }).ToList();

            //  Initialize the RelatedCategories collection if it is null
            page.RelatedCategories ??= new List<PageCategory>();

            // Add all the new PageCategory entities to the collection
            page.RelatedCategories.AddRange(pageCategories);

            await _context.SaveChangesAsync();
        }


        public async Task<PageFormViewModel> InitializePageFormViewModelAsync(PageFormViewModel? model = null)
        {
            model ??= new PageFormViewModel(); // Initialize a new model if none is provided.

            model.Categories = await _categoriesRepository.GetCategoriesNamesAsync();
            model.Forms = await _formsrepository.GetFormsNamesAsync();
            model.GalleryStyles = GetSelectListFromEnum<GalleryStyle>();
            model.DateInput = DateTime.Now;

            model.LinkToCategories = await _categoriesRepository.GetCategoriesSlugsNamesAsync();

            model.LinkToFiles = await _context.Documents
             .Select(d => new SelectListItem
             {
                 Value = d.FullUrl,
                 Text = d.OriginalName
             })
             .ToListAsync();

            // to get Disable the order input in the form in case of selection not manual category
            model.DisabledOrderCategories = await _context.Categories
                      .Where(c => c.TypeOfSorting != TypeOfSorting.Manual)
                      .Select(c => c.Id)
                      .ToListAsync();

            return model;
        }

        public void Updatepage(Page page)
        {
            _context.Update(page);
            _context.SaveChanges();
        }

        public void Deletepage(Page page)
        {
            _context.Pages.Remove(page);
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

        public async Task<IEnumerable<SelectListItem>> GetPagesNamesAsync()
        {
            return await _context.Pages
             .Select(pc => new SelectListItem
             {
                 Value = pc.Id.ToString(),
                 Text = pc.Title
             })
             .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetPagesSlugesAsync()
        {
            return await _context.Pages
             .Select(pc => new SelectListItem
             {
                 Value = pc.Slug,
                 Text = pc.Title
             })
             .ToListAsync();
        }
    }
}
