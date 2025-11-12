using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace ES.Web.Filters
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string ApiKeyHeaderName = "Authorization";
        private readonly string _apiKey;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration config)
            : base(options, logger, encoder, clock)
        {
            _apiKey = config["ApiSettings:ApiKey"]; // stored in appsettings.json
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(ApiKeyHeaderName))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

            var headerValue = Request.Headers[ApiKeyHeaderName].ToString();

            if (headerValue == $"ApiKey {_apiKey}")
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "ApiUser") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }
    }
}
