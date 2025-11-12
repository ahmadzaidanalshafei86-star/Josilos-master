

using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class MaterialsTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        public MaterialsTranslatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<MaterialTranslate> GetMaterialTranslateByIdAsync(int id)
        {
            var translate = await _context.MaterialsTranslate.FindAsync(id);

            if (translate == null)
                throw new Exception(message: "Material not found");

            return translate;

        }

        public async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int materialId)
        {

            // Get all language IDs that already have translations for the given Material
            var translatedLanguageIds = await _context.MaterialsTranslate
                .Where(ct => ct.MaterialId == materialId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the Material and not showing it in the dropdown
            var material = await _context.Materials
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == materialId);

            if (material == null)
                throw new Exception(message: "Material not found");

            return await _context.Languages
           .Where(l => l.Code != material.Language.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }

        public async Task<MaterialTranslationFormViewModel> InitializeMaterialTranslatesFormViewModelAsync(int materialId, MaterialTranslationFormViewModel? model = null)
        {
            model ??= new MaterialTranslationFormViewModel(); // Initialize a new model if none is provided.

            model.Languages = await GetLanguagesAsync(materialId);
            return model;
        }

        public async Task<int> AddMaterialTranslateAsync(MaterialTranslate materialTranslate)
        {
            await _context.MaterialsTranslate.AddAsync(materialTranslate);
            await _context.SaveChangesAsync();
            return materialTranslate.Id;
        }


        public void UpdateMaterial(MaterialTranslate materialTranslate)
        {
            _context.MaterialsTranslate.Update(materialTranslate);
            _context.SaveChanges();
        }

        public async Task<bool> DeleteTranslationAsync(int id)
        {
            var translate = await GetMaterialTranslateByIdAsync(id);

            _context.MaterialsTranslate.Remove(translate);
            await _context.SaveChangesAsync();

            return true;

        }
    }

}
