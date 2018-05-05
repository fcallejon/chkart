using Microsoft.AspNetCore.Authentication;

namespace chktr.ApiKeyAuthentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "CHKTR-APIKEY";
        public string Scheme => DefaultScheme;
    }
}