using ES.Web.Helpers;
using ES.Web.Models;
using Microsoft.Data.SqlClient;
using System.Data;


namespace ES.Web.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SearchResultsViewModel>> SearchAsync(string query, string sortOrder = "desc")
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchResultsViewModel>();

            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var parameters = new[]
            {
                new SqlParameter("@Query", SqlDbType.NVarChar) { Value = query },
                new SqlParameter("@LanguageId", SqlDbType.Int) { Value = languageId },
                new SqlParameter("@SortOrder", SqlDbType.NVarChar) { Value = sortOrder }
            };

            await _context.Database.OpenConnectionAsync();

            using var command = (SqlCommand)_context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SearchContent";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            var results = new List<SearchResultsViewModel>();

            // --- Result set 1: Pages (Default Language) ---
            while (await reader.ReadAsync())
            {
                results.Add(new SearchResultsViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) 
                        ? (reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? "" : reader.GetString(reader.GetOrdinal("LongDescription")))
                        : reader.GetString(reader.GetOrdinal("ShortDescription")),
                    Type = reader.GetString(reader.GetOrdinal("ContentType")),
                    Url = reader.IsDBNull(reader.GetOrdinal("URLTarget")) 
                        ? $"/page/{reader.GetString(reader.GetOrdinal("Slug"))}"
                        : reader.GetString(reader.GetOrdinal("URLTarget")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "Pages" : reader.GetString(reader.GetOrdinal("CategoryName"))
                });
            }

            // --- Result set 2: Page Translations ---
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new SearchResultsViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) 
                        ? (reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? "" : reader.GetString(reader.GetOrdinal("LongDescription")))
                        : reader.GetString(reader.GetOrdinal("ShortDescription")),
                    Type = reader.GetString(reader.GetOrdinal("ContentType")),
                    Url = reader.IsDBNull(reader.GetOrdinal("URLTarget")) 
                        ? $"/page/{reader.GetString(reader.GetOrdinal("Slug"))}"
                        : reader.GetString(reader.GetOrdinal("URLTarget")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "Pages" : reader.GetString(reader.GetOrdinal("CategoryName"))
                });
            }

            // --- Result set 3: Categories (Default Language) ---
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new SearchResultsViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) 
                        ? (reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? "" : reader.GetString(reader.GetOrdinal("LongDescription")))
                        : reader.GetString(reader.GetOrdinal("ShortDescription")),
                    Type = reader.GetString(reader.GetOrdinal("ContentType")),
                    Url = reader.IsDBNull(reader.GetOrdinal("Link")) 
                        ? $"/categories/{reader.GetString(reader.GetOrdinal("Slug"))}"
                        : reader.GetString(reader.GetOrdinal("Link")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader.GetString(reader.GetOrdinal("CategoryName"))
                });
            }

            // --- Result set 4: Category Translations ---
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new SearchResultsViewModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.IsDBNull(reader.GetOrdinal("ShortDescription")) 
                        ? (reader.IsDBNull(reader.GetOrdinal("LongDescription")) ? "" : reader.GetString(reader.GetOrdinal("LongDescription")))
                        : reader.GetString(reader.GetOrdinal("ShortDescription")),
                    Type = reader.GetString(reader.GetOrdinal("ContentType")),
                    Url = reader.IsDBNull(reader.GetOrdinal("Link")) 
                        ? $"/categories/{reader.GetString(reader.GetOrdinal("Slug"))}"
                        : reader.GetString(reader.GetOrdinal("Link")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader.GetString(reader.GetOrdinal("CategoryName"))
                });
            }

            // Remove duplicates by Id and Type, then sort
            var uniqueResults = results
                .GroupBy(r => new { r.Id, r.Type })
                .Select(g => g.First());

            // Sort results based on sortOrder
            if (sortOrder == "asc")
            {
                return uniqueResults.OrderBy(r => r.CreatedAt).ToList();
            }
            else
            {
                return uniqueResults.OrderByDescending(r => r.CreatedAt).ToList();
            }
        }
    }
}