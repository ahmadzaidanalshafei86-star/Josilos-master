using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class BrandsRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BrandViewModel>> GetBrandsAsync()
        {
            return await _context.Brands
                .Select(b => new BrandViewModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    LogoUrl = b.LogoUrl,
                    LogoAltName = b.LogoAltName,
                    CreatedAt = b.CreatedAt,
                    IsActive = b.IsActive
                })
                .ToListAsync();
        }

        public async Task<Brand?> GetBrandWithTranslationAsync(int brandId)
        {
            return await _context.Brands
                 .Include(c => c.Language)
                 .Include(c => c.BrandTranslates!)
                    .ThenInclude(c => c.Language)
                 .SingleOrDefaultAsync(c => c.Id == brandId);
        }

        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<int> AddBrandAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return brand.Id;
        }

        public async Task<Brand?> IsBrandNameExist(string name)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.Name == name);
        }

        public async Task UpdateBrandAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBrandAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetBrandsNamesAsync()
        {
            return await _context.Brands
                .Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                })
                .ToListAsync();
        }
    }
}
