using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Synapxe.Fhir.ProfileValidator.Security;

public class HeadersAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SecurityHeaderName = "X-Ihis-SourceApplication";

    public HeadersAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var request = Context.Request;
        if (request.Headers.TryGetValue(SecurityHeaderName, out var securityHeader))
        {
            var identity = new ClaimsIdentity(securityHeader);
            var principal = new ClaimsPrincipal(identity);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, "HeaderIdentity")));
        }
        else
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing header"));
        }
    }
}
