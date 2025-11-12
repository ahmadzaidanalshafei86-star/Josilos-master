using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;
using ES.Web.Helpers;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class LanguagesController : Controller
    {
        private readonly LanguagesRepository _languagesRepository;
        private readonly ILanguageService _languageService;
        public LanguagesController(LanguagesRepository languagesRepository, ILanguageService languageService)
        {
            _languagesRepository = languagesRepository;
            _languageService = languageService;
        }


        // For the drop down list to change admin language only
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Set the admin language in a separate cookie
            Response.Cookies.Append(
                CustomCookieRequestCultureProvider.AdminCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    Path = "/" // Make it available site-wide but still separate cookie
                }
            );

            // If returnUrl is null or empty, use the current URL with query parameters
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Request.Path + Request.QueryString;
            }

            return LocalRedirect(returnUrl);
        }


        [HttpGet]
        public async Task<IActionResult> SetDefaultDbLanguage()
        {
            var languages = await _languagesRepository.GetAllLanguages();

            LanguageSelectionViewModel model = new()
            {
                Languages = languages.ToList(),
            };

            model.SelectedLanguageCode = await _languageService.GetDefaultDbCultureAsync();
            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultDbLanguage(LanguageSelectionViewModel model)
        {
            var selectedLanguage = model.SelectedLanguageCode;

            // Define the path to the JSON file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CMS", "defaultDbLanguage.json");

            // Create a JSON object to store the selected language
            var languageData = new
            {
                DefaultDbCulture = selectedLanguage,
                UpdatedAt = DateTime.UtcNow
            };

            // Serialize the object to JSON
            var jsonData = System.Text.Json.JsonSerializer.Serialize(languageData, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true // Makes the JSON human-readable
            });

            // Write the JSON data to the file
            await System.IO.File.WriteAllTextAsync(filePath, jsonData);


            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        public IActionResult Add()
        {
            var languageDropdown = LanguageList.Languages.Select(l => new SelectListItem
            {
                Value = l.Value,
                Text = l.Key
            }).ToList();

            // Pass the dropdown list to the view
            ViewBag.LanguageDropdown = languageDropdown;

            return View("Form");
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.SuperAdmin)]
        public IActionResult Add(string code)
        {

            var languageName = LanguageList.Languages.FirstOrDefault(l => l.Value == code).Key;

            Language language = new()
            {
                Name = languageName,
                Code = code
            };

            _languagesRepository.AddLanguage(language);

            return RedirectToAction(nameof(SetDefaultDbLanguage));
        }
    }
}
