using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class ProductAttributesRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductAttributesRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        // For Attributes
        public async Task<IEnumerable<ProductAttributeViewModel>> GetAllAsync()
        {
            var attributes = await _context.ProductAttributes
               .Include(pa => pa.Values)
               .ToListAsync();

            return attributes.Select(a => new ProductAttributeViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Values = a.Values.Select(v => new ProductAttributeValueViewModel
                {
                    Id = v.Id,
                    Value = v.Value ?? string.Empty,
                    Order = v.Order
                }).ToList()
            }).ToList();
        }

        public async Task<ProductAttribute?> GetAttributeEntityByIdAsync(int id)
        {
            return await _context.ProductAttributes.FindAsync(id);
        }

        public async Task<ProductAttributeViewModel?> GetByIdAsync(int id)
        {
            var attribute = await _context.ProductAttributes
               .Include(pa => pa.Values)
               .FirstOrDefaultAsync(pa => pa.Id == id);

            if (attribute == null) return null;

            return new ProductAttributeViewModel
            {
                Id = attribute.Id,
                Name = attribute.Name,
                Values = attribute.Values.Select(v => new ProductAttributeValueViewModel
                {
                    Id = v.Id,
                    Value = v.Value ?? string.Empty,
                    Order = v.Order
                }).ToList()
            };
        }

        public async Task AddAsync(ProductAttribute attribute)
        {
            await _context.ProductAttributes.AddAsync(attribute);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAttirbuteAsync(ProductAttribute productAttribute)
        {
            _context.ProductAttributes.Remove(productAttribute);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAttributeAsync(ProductAttribute productAttribute)
        {
            _context.ProductAttributes.Update(productAttribute);
            await _context.SaveChangesAsync();
        }


        // For Values
        public async Task<ProductAttributeValue?> GetValueByIdAsync(int id)
        {
            return await _context.ProductAttributeValues.FindAsync(id);
        }
        public async Task AddValueAsync(ProductAttributeValue value)
        {
            await _context.ProductAttributeValues.AddAsync(value);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateValueAsync(ProductAttributeValue value)
        {
            _context.ProductAttributeValues.Update(value);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteValueAsync(ProductAttributeValue value)
        {
            _context.ProductAttributeValues.Remove(value);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // For translations
        public async Task<ProductAttribute?> GetProductAttributeWithTranslationsAsync(int id)
        {
            return await _context.ProductAttributes
                .Include(pa => pa.Language)
                .Include(pa => pa.Translations)
                .ThenInclude(pat => pat.Language)
                .FirstOrDefaultAsync(pa => pa.Id == id);
        }

        public async Task<ProductAttribute?> GetAttibuteWithValuesByIdAsync(int id)
        {
            return await _context.ProductAttributes.Include(Pa => Pa.Values).SingleOrDefaultAsync(Pa => Pa.Id == id);
        }
    }
}
