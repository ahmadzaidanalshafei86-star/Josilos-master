using Microsoft.AspNetCore.Localization;

namespace ES.Web.Helpers
{
    public class CustomCookieRequestCultureProvider : IRequestCultureProvider
    {
        public const string WebsiteCookieName = "ES.Website.Culture";
        public const string AdminCookieName = "ES.Admin.Culture";

        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var request = httpContext.Request;

            // Check multiple ways to detect admin area
            var isAdminArea = request.Path.StartsWithSegments("/EsAdmin") ||
                             request.RouteValues["area"]?.ToString()?.Equals("EsAdmin", StringComparison.OrdinalIgnoreCase) == true;

            var cookieName = isAdminArea ? AdminCookieName : WebsiteCookieName;
            var cookieValue = request.Cookies[cookieName];

            if (string.IsNullOrEmpty(cookieValue))
            {
                return Task.FromResult((ProviderCultureResult)null);
            }

            var culture = CookieRequestCultureProvider.ParseCookieValue(cookieValue);
            return Task.FromResult(culture);
        }
    }
}
