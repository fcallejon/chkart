using System;
using Microsoft.AspNetCore.Authentication;

namespace chktr.ApiKeyAuthentication
{
    public static class ApiKeyAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddApiKeyAuth(this AuthenticationBuilder builder, Action<ApiKeyAuthenticationOptions> configureOptions)
        {
            return builder
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    ApiKeyAuthenticationOptions.DefaultScheme,
                    configureOptions);
        }
    }
}
