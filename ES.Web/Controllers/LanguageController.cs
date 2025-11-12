using ES.Web.Helpers;
using Microsoft.AspNetCore.Localization;


namespace ES.Web.Controllers
{
    [AllowAnonymous]
    public class LanguageController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Set the website language in a separate cookie
            Response.Cookies.Append(
                CustomCookieRequestCultureProvider.WebsiteCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    Path = "/" // Available site-wide but separate from admin
                }
            );

            // If returnUrl is null or empty, use the current URL
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "~/";
            }

            return LocalRedirect(returnUrl);
        }
    }
}
