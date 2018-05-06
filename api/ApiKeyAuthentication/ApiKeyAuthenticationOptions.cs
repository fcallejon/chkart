using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Authentication;

namespace chktr.ApiKeyAuthentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        private readonly Dictionary<string, string> _applicationsPerKey = new Dictionary<string, string>();
        public const string DefaultScheme = "CHKTR-APIKEY";
        public string Scheme => DefaultScheme;
        public IReadOnlyDictionary<string, string> ApplicationsPerKey 
            => new ReadOnlyDictionary<string, string>(_applicationsPerKey);

        public bool TryAddApplicationKey(string applicationName, string applicationKey)
        {
            return _applicationsPerKey.TryAdd(applicationKey, applicationName);
        }
    }
}