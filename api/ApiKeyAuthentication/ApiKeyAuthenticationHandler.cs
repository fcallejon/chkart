using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace chktr.ApiKeyAuthentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
         : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing authorization header."));
            }

            if (authorization != "A-KEY-GNERATED-BY-A-KEY-MANAGEMENT-SYSTEM")
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Key."));
            }

            var identities = new List<ClaimsIdentity> {new ClaimsIdentity(Options.Scheme)};      
            var authTicket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);
            return Task.FromResult(AuthenticateResult.Success(authTicket));
        }
    }
}