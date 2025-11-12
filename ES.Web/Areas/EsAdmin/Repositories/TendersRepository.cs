

using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using ES.Core.Enums;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class TendersRepository
    {
        private readonly ApplicationDbContext _context;

        public TendersRepository(ApplicationDbContext context)
        {
            _context = context;
           
        }
        //public async Task<Page> GetPageByIdAsync(int id)
        //{
        //    var page = await _context.Tenders
        //                .Include(p => p.RelatedCategories)
        //                .SingleOrDefaultAsync(p => p.Id == id);
        //    return page;

        //}
        //public async Task<Page> GetPageByIdWithTranslationsAsync(int pageId)
        //{
        //    var page = await _context.Tenders
        //         .Include(c => c.Language)
        //         .Include(c => c.PageTranslates!)
        //         .ThenInclude(c => c.Language)
        //         .SingleOrDefaultAsync(c => c.Id == pageId);

        //    if (page == null)
        //        throw new Exception(message: "Page not found");

        //    return page;
        //}


        //public async Task<Page> GetPageWithGalleryImagesAsync(int id)
        //{
        //    var page = await _context.Tenders
        //                .Include(p => p.PageGalleryImages)
        //                .Include(p => p.PageFiles)
        //                .Include(p => p.RelatedCategories)
        //                .SingleOrDefaultAsync(p => p.Id == id);
        //    return page;
        //}

        public async Task<IEnumerable<TenderViewModel>> GetAllTendersAsync()
        {
            return await _context.Tenders
                        .Select(p => new TenderViewModel
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Code = p.Code,
                            CopyPrice = p.CopyPrice,
                            StartDate = p.StartDate,
                            EndDate = p.EndDate,
                            Publish = p.Publish,
                            MoveToArchive = p.MoveToArchive,

                        })
                        .ToListAsync();
        }

        public async Task<int> AddTenderAsync(Tender tender)
        {
            await _context.Tenders.AddAsync(tender);
            await _context.SaveChangesAsync();
            return tender.Id;
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

        public async Task<TenderFormViewModel> InitializeTenderFormViewModelAsync(TenderFormViewModel? model = null)
        {
            model ??= new TenderFormViewModel(); // Initialize a new model if none is provided.

            model.SortingTypes = GetSelectListFromEnum<TypeOfSorting>();
            model.Tenders = await GetTendersNamesAsync();

            return model;
        }
      
        public void Updatepage(Tender tender)
        {
            _context.Update(tender);
            _context.SaveChanges();
        }

        public void Deletepage(Tender tender)
        {
            _context.Tenders.Remove(tender);
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

        public async Task<IEnumerable<SelectListItem>> GetTendersNamesAsync()
        {
            return await _context.Tenders
             .Select(pc => new SelectListItem
             {
                 Value = pc.Id.ToString(),
                 Text = pc.Title
             })
             .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetTendersSlugesAsync()
        {
            return await _context.Tenders
             .Select(pc => new SelectListItem
             {
                 Value = pc.Slug,
                 Text = pc.Title
             })
             .ToListAsync();
        }
    }
}
