


using ES.Core.Entities;
using ES.Core.Enums;
using ES.Web.Areas.EsAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class MaterialsRepository
    {
        private readonly ApplicationDbContext _context;
        public MaterialsRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<MaterialsViewModel>> GetMaterialsWithParentInfoAsync()
        {
            var materials = await _context.Materials
                .Select(c => new MaterialsViewModel
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Name = c.Name

                })
                .ToListAsync();

            return materials;
            //return await _context.materialViewModel
            //.FromSqlRaw("EXEC GetMaterialsWithParentInfo")
            //.ToListAsync();

        }

        public async Task<Material> GetMaterialByIdAsync(int id)
        {
                var material = await _context.Materials
                .FirstOrDefaultAsync(c => c.Id == id); // Filter by specific material

            if (material == null)
                throw new Exception(message: "Material not found");

            return material;
        }

        public async Task<Material> GetMaterialByIdWithTranslationsAsync(int materialId)
        {
            var material = await _context.Materials
                .Include(c => c.Language)
                .Include(c => c.MaterialsTranslates!)
                .ThenInclude(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == materialId);

            if (material == null)
                throw new Exception(message: "Material not found");

            return material;
        }

        public async Task<Material> GetmaterialWithAllDataAsync(int materialId)
        {
            var material = await _context.Materials
                .SingleOrDefaultAsync(c => c.Id == materialId);

            if (material == null)
                throw new Exception(message: "material not found");

            return material;
        }


        public async Task<IEnumerable<SelectListItem>> GetMaterialsSlugsNamesAsync()
        {
            return await _context.Materials
             .Select(pc => new SelectListItem
             {
                 Value = pc.Slug,
                 Text = pc.Name
             })
             .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetMaterialsNamesAsync()
        {
            return await _context.Materials
             .Select(pc => new SelectListItem
             {
                 Value = pc.Id.ToString(),
                 Text = pc.Name
             })
             .ToListAsync();
        }




        public async Task<int> AddmaterialAsync(Material material)
        {
            await _context.Materials.AddAsync(material);
            await _context.SaveChangesAsync();
            return material.Id;
        }// Return the ID of the newly created material

        //    var idParameter = new SqlParameter
        //    {
        //        ParameterName = "@Id",
        //        SqlDbType = System.Data.SqlDbType.Int,
        //        Direction = System.Data.ParameterDirection.Output
        //    };

        //    await _context.Database.ExecuteSqlRawAsync(
        //        @"EXEC Addmaterial 
        //    @Name, @LongDescription, @ShortDescription, @FeaturedImageUrl, @FeaturedImageAltName,
        //    @CoverImageUrl, @CoverImageAltName, @MetaDescription, @MetaKeywords, @Order, @TypeOfSorting,
        //    @CreatedDate, @ParentmaterialId, @LanguageId, @ThemeId, @IsPublished, @Id OUTPUT",
        //        new SqlParameter("@Name", material.Name),
        //        new SqlParameter("@LongDescription", material.LongDescription ?? (object)DBNull.Value),
        //        new SqlParameter("@ShortDescription", material.ShortDescription ?? (object)DBNull.Value),
        //        new SqlParameter("@FeaturedImageUrl", material.FeaturedImageUrl ?? (object)DBNull.Value),
        //        new SqlParameter("@FeaturedImageAltName", material.FeaturedImageAltName ?? (object)DBNull.Value),
        //        new SqlParameter("@CoverImageUrl", material.CoverImageUrl ?? (object)DBNull.Value),
        //        new SqlParameter("@CoverImageAltName", material.CoverImageAltName ?? (object)DBNull.Value),
        //        new SqlParameter("@MetaDescription", material.MetaDescription ?? (object)DBNull.Value),
        //        new SqlParameter("@MetaKeywords", material.MetaKeywords ?? (object)DBNull.Value),
        //        new SqlParameter("@Order", material.Order),
        //        new SqlParameter("@TypeOfSorting", (int)material.TypeOfSorting),
        //        new SqlParameter("@CreatedDate", material.CreatedDate),
        //        new SqlParameter("@ParentmaterialId", material.ParentmaterialId ?? (object)DBNull.Value),
        //        new SqlParameter("@LanguageId", material.LanguageId),
        //        new SqlParameter("@ThemeId", material.ThemeId),
        //        new SqlParameter("@IsPublished", material.IsPublished),
        //        idParameter
        //    );

        //    return (int)idParameter.Value;
        //}

        public async Task AddRelatedMaterialsAsync(Material material, List<int> relatedmaterialIds)
        {
            foreach (var relatedmaterialId in relatedmaterialIds)
            {
                var relatedmaterial = await _context.Materials.FindAsync(relatedmaterialId);
               
            }
        }

        public async Task<bool> DeletematerialAsync(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material == null)
                return false;

            _context.Materials.Remove(material);
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

        public async Task<MaterialFormViewModel> InitializematerialFormViewModelAsync(MaterialFormViewModel? model = null)
        {
            model ??= new MaterialFormViewModel(); // Initialize a new model if none is provided.

            model.SortingTypes = GetSelectListFromEnum<TypeOfSorting>();
            model.Materials = await GetMaterialsNamesAsync();

            return model;
        }

        public bool IsParentmaterial(int materialId)
        {
            var childMaterials = _context.Materials;
            if (childMaterials.Any())
                return true;

            return false;
        }

        public bool IsRelatedToAnothermaterial(int materialId)
        {
            var IsRelated = _context.Materials;
            

            return false;
        }

        public void Updatematerial(Material material)
        {
            _context.Materials.Update(material);
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

