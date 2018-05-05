using System.Collections.Generic;
using IdentityServer4.Models;

namespace sts
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "CHKTR-API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client 
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("fcallejon_super_secret_string_buuu".Sha256())
                    },
                    AllowedScopes = { "api" },
                    RedirectUris = {
                        "http://localhost:5000",
                        "http://localhost:5000/oauth2-redirect.html",
                        "http://localhost:8181/oauth2-redirect.html"
                    }
                }
            };
        }
    }
}