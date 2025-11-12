using ES.Core.Enums;
using ES.Web.Helpers;
using ES.Web.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ES.Web.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryWithPagesViewModel?> GetCategoryWithPagesBySlugAsync(string Categoryslug, int Take = int.MaxValue)
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var parameters = new[]
            {
                new SqlParameter("@Slug", SqlDbType.NVarChar) { Value = Categoryslug },
                new SqlParameter("@LanguageId", SqlDbType.Int) { Value = languageId }
            };

            await _context.Database.OpenConnectionAsync();

            using var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetCategoryWithPagesBySlug";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            // --- Result set 1: Main category ---
            if (!await reader.ReadAsync())
                return null;

            var category = new
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Slug = reader.GetString(reader.GetOrdinal("Slug")),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                ShortDescription = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription")),
                LongDescription = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription")),
                CoverImageUrl = reader.IsDBNull(reader.GetOrdinal("CoverImageUrl")) ? null : reader.GetString(reader.GetOrdinal("CoverImageUrl")),
                CoverImageAltName = reader.IsDBNull(reader.GetOrdinal("CoverImageAltName")) ? null : reader.GetString(reader.GetOrdinal("CoverImageAltName")),
                FeaturedImageUrl = reader.IsDBNull(reader.GetOrdinal("FeaturedImageUrl")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageUrl")),
                FeaturedImageAltName = reader.IsDBNull(reader.GetOrdinal("FeaturedImageAltName")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageAltName")),
                TypeOfSorting = reader.IsDBNull(reader.GetOrdinal("TypeOfSorting")) ? "Manual" : reader.GetString(reader.GetOrdinal("TypeOfSorting")),
                ThemeName = reader.IsDBNull(reader.GetOrdinal("ThemeName")) ? null : reader.GetString(reader.GetOrdinal("ThemeName"))
            };

            // --- Result set 2: Category translations ---
            await reader.NextResultAsync();
            string? translatedName = null, translatedShortDesc = null, translatedLongDesc = null;

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    translatedName = reader.IsDBNull("Name") ? null : reader.GetString("Name");
                    translatedShortDesc = reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription");
                    translatedLongDesc = reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription");
                    break;
                }
            }

            // --- Result set 3: Related pages ---
            await reader.NextResultAsync();
            var pages = new List<PageViewModel>();
            var typeOfSorting = Enum.TryParse<TypeOfSorting>(category.TypeOfSorting, true, out var parsedSorting) 
                ? parsedSorting 
                : TypeOfSorting.Manual;

            while (await reader.ReadAsync())
            {
                pages.Add(new PageViewModel
                {
                    Slug = reader.GetString("Slug"),
                    Title = reader.IsDBNull("Title") ? null : reader.GetString("Title"),
                    FeaturedImageAltName = reader.IsDBNull("FeaturedImageAltName") ? null : reader.GetString("FeaturedImageAltName"),
                    FeaturedImageUrl = reader.IsDBNull("FeatruedImageUrl") ? null : reader.GetString("FeatruedImageUrl"), // Note: Using actual column name from DB
                    ShortDescription = reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription"),
                    LongDescription = reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription"),
                    VideoUrl = reader.IsDBNull("VideoURL") ? null : reader.GetString("VideoURL"),
                    Order = reader.GetInt32("Order"),
                    CreatedAt = reader.GetDateTime("CreatedDate"),
                    DateInput = reader.IsDBNull("DateInput") ? null : reader.GetDateTime("DateInput"),
                    Count = reader.IsDBNull("Count") ? 0 : reader.GetInt32("Count"),
                    LinkUrl = reader.IsDBNull("LinkToUrl") ? null : reader.GetString("LinkToUrl")
                });
            }

            // --- Result set 4: Page translations ---
            await reader.NextResultAsync();
            var pageTranslations = new Dictionary<string, (string? Title, string? ShortDescription, string? LongDescription)>();

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    var slug = reader.GetString("Slug");
                    pageTranslations[slug] = (
                        reader.IsDBNull("Title") ? null : reader.GetString("Title"),
                        reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription"),
                        reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription")
                    );
                }
            }

            // Apply translations to pages
            foreach (var page in pages)
            {
                if (pageTranslations.TryGetValue(page.Slug, out var translation))
                {
                    page.Title = translation.Title ?? page.Title;
                    page.ShortDescription = translation.ShortDescription ?? page.ShortDescription;
                    page.LongDescription = translation.LongDescription ?? page.LongDescription;
                }
            }

            // Apply sorting
            var sortedPages = typeOfSorting switch
            {
                TypeOfSorting.Manual => pages.OrderBy(p => p.Order).ToList(),
                TypeOfSorting.NewToOld => pages.OrderByDescending(p => p.CreatedAt).ToList(),
                TypeOfSorting.OldToNew => pages.OrderBy(p => p.CreatedAt).ToList(),
                TypeOfSorting.Alphabetical => pages.OrderBy(p => p.Title).ToList(),
                TypeOfSorting.AlphabeticalReversed => pages.OrderByDescending(p => p.Title).ToList(),
                _ => pages.OrderBy(p => p.Order).ToList()
            };

            if (Take < int.MaxValue)
                sortedPages = sortedPages.OrderByDescending(p => p.CreatedAt).Take(Take).ToList();

            var viewModel = new CategoryWithPagesViewModel
            {
                Slug = category.Slug,
                Name = translatedName ?? category.Name,
                ShortDescription = translatedShortDesc ?? category.ShortDescription,
                LongDescription = translatedLongDesc ?? category.LongDescription,
                CoverImageUrl = category.CoverImageUrl,
                CoverimageAltName = category.CoverImageAltName,
                FeaturedImageUrl = category.FeaturedImageUrl,
                FeaturedimageAltName = category.FeaturedImageAltName,
                ThemeName = category.ThemeName,
                Pages = sortedPages
            };

            return viewModel;
        }
        public async Task<CategoryWithPagesViewModel?> GetCategoryWithPagesByIdAsync(int Id, int Take = int.MaxValue)
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var parameters = new[]
            {
                new SqlParameter("@CategoryId", SqlDbType.Int) { Value = Id },
                new SqlParameter("@LanguageId", SqlDbType.Int) { Value = languageId }
            };

            await _context.Database.OpenConnectionAsync();

            using var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetCategoryWithPagesById";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            // --- Result set 1: Main category ---
            if (!await reader.ReadAsync())
                return null;

            var category = new
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Slug = reader.GetString(reader.GetOrdinal("Slug")),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                ShortDescription = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription")),
                LongDescription = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription")),
                CoverImageUrl = reader.IsDBNull(reader.GetOrdinal("CoverImageUrl")) ? null : reader.GetString(reader.GetOrdinal("CoverImageUrl")),
                CoverImageAltName = reader.IsDBNull(reader.GetOrdinal("CoverImageAltName")) ? null : reader.GetString(reader.GetOrdinal("CoverImageAltName")),
                FeaturedImageUrl = reader.IsDBNull(reader.GetOrdinal("FeaturedImageUrl")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageUrl")),
                FeaturedImageAltName = reader.IsDBNull(reader.GetOrdinal("FeaturedImageAltName")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageAltName")),
                TypeOfSorting = reader.IsDBNull(reader.GetOrdinal("TypeOfSorting")) ? "Manual" : reader.GetString(reader.GetOrdinal("TypeOfSorting")),
                ThemeName = reader.IsDBNull(reader.GetOrdinal("ThemeName")) ? null : reader.GetString(reader.GetOrdinal("ThemeName"))
            };

            // --- Result set 2: Category translations ---
            await reader.NextResultAsync();
            string? translatedName = null, translatedShortDesc = null, translatedLongDesc = null;

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    translatedName = reader.IsDBNull("Name") ? null : reader.GetString("Name");
                    translatedShortDesc = reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription");
                    translatedLongDesc = reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription");
                    break;
                }
            }

            // --- Result set 3: Related pages ---
            await reader.NextResultAsync();
            var pages = new List<PageViewModel>();
            var typeOfSorting = Enum.TryParse<TypeOfSorting>(category.TypeOfSorting, true, out var parsedSorting)
                ? parsedSorting
                : TypeOfSorting.Manual;

            while (await reader.ReadAsync())
            {
                pages.Add(new PageViewModel
                {
                    Slug = reader.GetString("Slug"),
                    Title = reader.IsDBNull("Title") ? null : reader.GetString("Title"),
                    FeaturedImageAltName = reader.IsDBNull("FeaturedImageAltName") ? null : reader.GetString("FeaturedImageAltName"),
                    FeaturedImageUrl = reader.IsDBNull("FeatruedImageUrl") ? null : reader.GetString("FeatruedImageUrl"), // Note: Using actual column name from DB
                    ShortDescription = reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription"),
                    LongDescription = reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription"),
                    VideoUrl = reader.IsDBNull("VideoURL") ? null : reader.GetString("VideoURL"),
                    Order = reader.GetInt32("Order"),
                    CreatedAt = reader.GetDateTime("CreatedDate"),
                    DateInput = reader.IsDBNull("DateInput") ? null : reader.GetDateTime("DateInput"),
                    Count = reader.IsDBNull("Count") ? 0 : reader.GetInt32("Count"),
                    LinkUrl = reader.IsDBNull("LinkToUrl") ? null : reader.GetString("LinkToUrl")
                });
            }

            // --- Result set 4: Page translations ---
            await reader.NextResultAsync();
            var pageTranslations = new Dictionary<string, (string? Title, string? ShortDescription, string? LongDescription)>();

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    var slug = reader.GetString("Slug");
                    pageTranslations[slug] = (
                        reader.IsDBNull("Title") ? null : reader.GetString("Title"),
                        reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription"),
                        reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription")
                    );
                }
            }

            // Apply translations to pages
            foreach (var page in pages)
            {
                if (pageTranslations.TryGetValue(page.Slug, out var translation))
                {
                    page.Title = translation.Title ?? page.Title;
                    page.ShortDescription = translation.ShortDescription ?? page.ShortDescription;
                    page.LongDescription = translation.LongDescription ?? page.LongDescription;
                }
            }

            // Apply sorting
            var sortedPages = typeOfSorting switch
            {
                TypeOfSorting.Manual => pages.OrderBy(p => p.Order).ToList(),
                TypeOfSorting.NewToOld => pages.OrderByDescending(p => p.CreatedAt).ToList(),
                TypeOfSorting.OldToNew => pages.OrderBy(p => p.CreatedAt).ToList(),
                TypeOfSorting.Alphabetical => pages.OrderBy(p => p.Title).ToList(),
                TypeOfSorting.AlphabeticalReversed => pages.OrderByDescending(p => p.Title).ToList(),
                _ => pages.OrderBy(p => p.Order).ToList()
            };

            if (Take < int.MaxValue)
                sortedPages = sortedPages.OrderByDescending(p => p.CreatedAt).Take(Take).ToList();

            var viewModel = new CategoryWithPagesViewModel
            {
                Slug = category.Slug,
                Name = translatedName ?? category.Name,
                ShortDescription = translatedShortDesc ?? category.ShortDescription,
                LongDescription = translatedLongDesc ?? category.LongDescription,
                CoverImageUrl = category.CoverImageUrl,
                CoverimageAltName = category.CoverImageAltName,
                FeaturedImageUrl = category.FeaturedImageUrl,
                FeaturedimageAltName = category.FeaturedImageAltName,
                ThemeName = category.ThemeName,
                Pages = sortedPages
            };

            return viewModel;
        }
        public async Task<CategoryOnlyViewModel?> GetCategoryBySlugAsync(string CategorySlug)
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var parameters = new[]
            {
                new SqlParameter("@Slug", SqlDbType.NVarChar) { Value = CategorySlug },
                new SqlParameter("@LanguageId", SqlDbType.Int) { Value = languageId }
            };

            await _context.Database.OpenConnectionAsync();

            using var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetCategoryBySlug";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            // --- Result set 1: Main category ---
            if (!await reader.ReadAsync())
                return null;

            var category = new
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Slug = reader.GetString(reader.GetOrdinal("Slug")),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                ShortDescription = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription")),
                LongDescription = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription")),
                CoverImageUrl = reader.IsDBNull(reader.GetOrdinal("CoverImageUrl")) ? null : reader.GetString(reader.GetOrdinal("CoverImageUrl")),
                CoverImageAltName = reader.IsDBNull(reader.GetOrdinal("CoverImageAltName")) ? null : reader.GetString(reader.GetOrdinal("CoverImageAltName")),
                FeaturedImageUrl = reader.IsDBNull(reader.GetOrdinal("FeaturedImageUrl")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageUrl")),
                FeaturedImageAltName = reader.IsDBNull(reader.GetOrdinal("FeaturedImageAltName")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageAltName"))
            };

            // --- Result set 2: Category translations ---
            await reader.NextResultAsync();
            string? translatedName = null, translatedShortDesc = null, translatedLongDesc = null;

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    translatedName = reader.IsDBNull("Name") ? null : reader.GetString("Name");
                    translatedShortDesc = reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription");
                    translatedLongDesc = reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription");
                    break;
                }
            }

            var viewModel = new CategoryOnlyViewModel
            {
                Slug = category.Slug,
                Name = translatedName ?? category.Name,
                ShortDescription = translatedShortDesc ?? category.ShortDescription,
                LongDescription = translatedLongDesc ?? category.LongDescription,
                CoverImageUrl = category.CoverImageUrl,
                CoverimageAltName = category.CoverImageAltName,
                FeaturedImageUrl = category.FeaturedImageUrl,
                FeaturedimageAltName = category.FeaturedImageAltName,
            };

            return viewModel;
        }
        public async Task<CategoryOnlyViewModel?> GetCategoryByIdAsync(int Id)
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var parameters = new[]
            {
                new SqlParameter("@CategoryId", SqlDbType.Int) { Value = Id },
                new SqlParameter("@LanguageId", SqlDbType.Int) { Value = languageId }
            };

            await _context.Database.OpenConnectionAsync();

            using var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetCategoryById";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            // --- Result set 1: Main category ---
            if (!await reader.ReadAsync())
                return null;

            var category = new
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Slug = reader.GetString(reader.GetOrdinal("Slug")),
                Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? null : reader.GetString(reader.GetOrdinal("Name")),
                ShortDescription = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) ? null : reader.GetString(reader.GetOrdinal("ShortDescription")),
                LongDescription = reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? null : reader.GetString(reader.GetOrdinal("LongDescription")),
                CoverImageUrl = reader.IsDBNull(reader.GetOrdinal("CoverImageUrl")) ? null : reader.GetString(reader.GetOrdinal("CoverImageUrl")),
                CoverImageAltName = reader.IsDBNull(reader.GetOrdinal("CoverImageAltName")) ? null : reader.GetString(reader.GetOrdinal("CoverImageAltName")),
                FeaturedImageUrl = reader.IsDBNull(reader.GetOrdinal("FeaturedImageUrl")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageUrl")),
                FeaturedImageAltName = reader.IsDBNull(reader.GetOrdinal("FeaturedImageAltName")) ? null : reader.GetString(reader.GetOrdinal("FeaturedImageAltName"))
            };

            // --- Result set 2: Category translations ---
            await reader.NextResultAsync();
            string? translatedName = null, translatedShortDesc = null, translatedLongDesc = null;

            while (await reader.ReadAsync())
            {
                if (reader.GetInt32("LanguageId") == languageId)
                {
                    translatedName = reader.IsDBNull("Name") ? null : reader.GetString("Name");
                    translatedShortDesc = reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription");
                    translatedLongDesc = reader.IsDBNull("LongDescription") ? null : reader.GetString("LongDescription");
                    break;
                }
            }

            var viewModel = new CategoryOnlyViewModel
            {
                Slug = category.Slug,
                Name = translatedName ?? category.Name,
                ShortDescription = translatedShortDesc ?? category.ShortDescription,
                LongDescription = translatedLongDesc ?? category.LongDescription,
                CoverImageUrl = category.CoverImageUrl,
                CoverimageAltName = category.CoverImageAltName,
                FeaturedImageUrl = category.FeaturedImageUrl,
                FeaturedimageAltName = category.FeaturedImageAltName,
            };

            return viewModel;
        }
    }
}

