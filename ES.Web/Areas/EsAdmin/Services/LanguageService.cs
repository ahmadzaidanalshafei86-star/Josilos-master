using Newtonsoft.Json;

namespace ES.Web.Areas.EsAdmin.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly string _filePath;

        public LanguageService()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CMS", "defaultDbLanguage.json");
        }

        public async Task<string?> GetDefaultDbCultureAsync()
        {
            // Check if the file exists
            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Default language file not found at {_filePath}.");

            try
            {
                // Read the JSON data from the file
                var jsonData = await File.ReadAllTextAsync(_filePath);

                // Deserialize the JSON data using Newtonsoft.Json
                var languageData = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

                // Get the language code from the deserialized data
                if (languageData != null && languageData.TryGetValue("DefaultDbCulture", out var languageCode))
                {
                    return languageCode;
                }

                throw new KeyNotFoundException("DefaultDbCulture not found in the JSON file.");
            }
            catch (JsonSerializationException ex)
            {
                // Handle JSON deserialization errors
                throw new InvalidOperationException("Invalid JSON format in the language file.", ex);
            }
            catch (Exception ex)
            {
                // Handle any other exceptions
                throw new InvalidOperationException("An error occurred while reading the language file.", ex);
            }
        }
    }
}