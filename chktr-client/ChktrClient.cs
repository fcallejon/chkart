using System;
using RestSharp;
using chktr.Model;
using System.Threading.Tasks;
using System.Net;

namespace chktr.Client
{
    public class ChktrClient
    {
        public string ApplicationName { get; private set; }

        public string ApplicationKey { get; private set; }

        private const string DefaultChktrHost = "http://localhost:5000";
        private const string CartGetUriPath = "/api/Cart/{cartId}";

        private readonly RestClient _client;

        public ChktrClient(string appName, string apiKey)
         : this(appName, apiKey, DefaultChktrHost)
        {
        }

        public ChktrClient(string appName, string apiKey, string chktrHost)
        {
            if (string.IsNullOrEmpty(appName))
            {
                throw new ArgumentNullException(nameof(appName));
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            if (string.IsNullOrEmpty(chktrHost))
            {
                throw new ArgumentNullException(nameof(chktrHost));
            }

            if (!Uri.TryCreate(chktrHost, UriKind.Absolute, out var parsedHostUri))
            {
                throw new UriFormatException($"'{chktrHost}' is not a valid url.");
            }

            ApplicationName = appName;
            ApplicationKey = apiKey;

            _client = new RestClient(parsedHostUri);
            _client.AddDefaultHeader("Authorization", apiKey);
        }

        public async Task<Cart> GetCart(Guid cartId)
        {
            var request = new RestRequest(CartGetUriPath, Method.GET);
            request.AddUrlSegment("cartId", cartId);
            var response = await _client.ExecuteGetTaskAsync<Cart>(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw response.ErrorException;
            }
            return response.Data;
        }
    }
}
