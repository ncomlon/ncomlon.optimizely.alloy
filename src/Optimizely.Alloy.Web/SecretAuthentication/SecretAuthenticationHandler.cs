using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Optimizely.Alloy.Web.SecretAuthentication
{
    public class SecretAuthenticationHandler : IAuthenticationHandler
    {
        public const string SchemeName = "SecretAuthentication";
        public const string DisplayName = "Secret Authentication";

        private const string SchemeParameterName = "Secret";
        private const string Secret = "SuperDuperSecretValue";

        private ClaimsPrincipal? _principal;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            if (AuthenticationHeaderValue.TryParse(context.Request.Headers.Authorization, out var authorization) &&
                authorization.Scheme.Equals(SchemeParameterName, StringComparison.OrdinalIgnoreCase) &&
                authorization.Parameter == Secret)
            {
                _principal = new ClaimsPrincipal(new ClaimsIdentity([
                    new Claim(ClaimTypes.Name, "SecretUser"),
                new Claim(ClaimTypes.Role, "CmsAdmins")
                ], SchemeName));
            }

            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            if (_principal is null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            return Task.FromResult(
                AuthenticateResult.Success(
                    new AuthenticationTicket(_principal, SchemeName)));
        }

        public Task ChallengeAsync(AuthenticationProperties? properties) => Task.CompletedTask;

        public Task ForbidAsync(AuthenticationProperties? properties) => Task.CompletedTask;
    }
}
