using System;
using RestSharp;
using chktr.Model;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("ClientTests")]

namespace chktr.Client
{
    /// <summary>
    /// A Client to use Chackout Cart
    /// 
    /// <example>
    /// <code>
    /// using (var client = new chktr.Client.ChktrClient("YOUR-APP-NAME", "YOUR-API-KEy"))
    /// {
    ///     var cartId = client.Create(new Cart {
    ///         Firstname = "Sample Name",
    ///         Lastname = "Sample Name",
    ///         Items = new [] {
    ///             new CartItem {
    ///                 Description = "Sample Prod 2",
    ///                 Quantity = 3,
    ///                 UnitPrice = 5
    ///             }
    ///         }
    ///     }).Result;
    ///     Console.WriteLine($"Created new Cart: {cartId.ToString()}");
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class ChktrClient : IDisposable
    {
        /// <summary>
        /// Your Application Name used when connecting to the service
        /// </summary>
        /// <returns></returns>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Your Application Key used when connecting to the service
        /// </summary>
        /// <returns></returns>
        public string ApplicationKey { get; private set; }

        private const string DefaultChktrHost = "http://localhost:5000";
        private const string CartIdUriPath = "/api/Cart/{cartId}";
        private const string CartPostUriPath = "/api/Cart";
        private readonly string _nl = Environment.NewLine;
        private IRestClient _client;
        private bool disposed;

        /// <summary>
        /// Creates a new <see cref="ChktrClient" /> using the specified <paramref name="apiKey" /> and <paramref name="appName" />
        /// </summary>
        /// <param name="appName">Your App Name.</param>
        /// <param name="apiKey">Your Api Key.</param>
        public ChktrClient(string appName, string apiKey)
         : this(appName, apiKey, DefaultChktrHost)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ChktrClient" /> using the specified <paramref name="chktrHost"/>, <paramref name="apiKey" /> and <paramref name="appName" />
        /// </summary>
        /// <param name="appName">Your App Name.</param>
        /// <param name="apiKey">Your Api Key.</param>
        /// <param name="chktrHost">An specific host for Checkout Cart Service.</param>
        public ChktrClient(string appName, string apiKey, string chktrHost)
         : this(appName, apiKey, chktrHost, new RestClient())
        {
        }

        internal ChktrClient(string appName, string apiKey, string chktrHost, IRestClient client)
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

            _client = client;
            _client.BaseUrl = parsedHostUri;
            _client.AddDefaultHeader("Authorization", $"{appName}:{apiKey}");
            _client.AddDefaultHeader("Cache-Control", "no-cache");
            _client.AddDefaultHeader("Content-Type", "application/json");
        }

        /// <summary>
        /// Returns the <see cref="Cart" /> for the specified <paramref name="cartId"/>
        /// </summary>
        /// <param name="cartId">The cart Id that's being queried.</param>
        /// <returns>A <see cref="Cart"/> instance.</returns>
        /// <exception cref="ApplicationException">In case server response is 500 or there is any other communication issues.</exception>
        public async Task<Cart> Get(Guid cartId)
        {
            if (cartId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(cartId)}' should have a value", nameof(cartId));
            }

            var request = new RestRequest(CartIdUriPath, Method.GET);
            request.AddUrlSegment("cartId", cartId);
            IRestResponse<Cart> response = null;
            try
            {
                response = await _client.ExecuteGetTaskAsync<Cart>(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                throw CreateException(response, ex);
            }
        }

        /// <summary>
        /// Updates the <see cref="Cart" /> matching the <paramref name="cartId"/>
        /// </summary>
        /// <param name="cartId">The cart id to update.</param>
        /// <param name="data">The new <see cref="Cart"/> data.</param>
        /// <returns>True if was found and updated.</returns>
        /// <exception cref="ApplicationException">In case server response is 500 or there is any other communication issues.</exception>
        public async Task<bool> Update(Guid cartId, Cart data)
        {
            if (cartId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(cartId)}' should have a value", nameof(cartId));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var request = new RestRequest(CartIdUriPath, Method.PUT);
            request.AddUrlSegment("cartId", cartId);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(data);
            IRestResponse<Cart> response = null;
            try
            {
                response = await _client.ExecuteTaskAsync<Cart>(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw response.ErrorException ?? new Exception();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw CreateException(response, ex);
            }
        }

        /// <summary>
        /// Delete a <see cref="Cart" /> using the <paramref name="cartId" />.
        /// </summary>
        /// <param name="cartId">The card id to delete.</param>
        /// <returns>True if the cart was found and deleted.</returns>
        /// <exception cref="ApplicationException">In case server response is 500 or there is any other communication issues.</exception>
        public async Task<bool> Delete(Guid cartId)
        {
            if (cartId == Guid.Empty)
            {
                throw new ArgumentException($"'{nameof(cartId)}' should have a value", nameof(cartId));
            }

            var request = new RestRequest(CartIdUriPath, Method.DELETE);
            IRestResponse<bool> response = null;
            try
            {
                response = await _client.ExecuteTaskAsync<bool>(request);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                throw CreateException(response, ex);
            }
        }

        /// <summary>
        /// Creates a new <see cref="Cart" /> using the data in <paramref name="data"/>.
        /// </summary>
        /// <param name="data">An instance of a <see cref="Cart" /> to be created. </param>
        /// <returns>The new <see cref="Cart" />'s Id.</returns>
        /// <exception cref="ApplicationException">In case server response is 500 or there is any other communication issues.</exception>
        public async Task<Guid> Create(Cart data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var request = new RestRequest(CartPostUriPath, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(data);
            IRestResponse<Guid> response = null;

            try
            {
                response = await _client.ExecutePostTaskAsync<Guid>(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw response.ErrorException ?? new Exception();
                }
                return response.Data;
            }
            catch (Exception ex)
            {
                throw CreateException(response, ex);
            }
        }

        /// <summary>
        /// Disposes the Checkout Cart Service client
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                // free managed resources  
                if (_client != null)
                {
                    _client = null;
                }
                disposed = true;
            }
        }

        private Exception CreateException(IRestResponse response, Exception catchedException)
        {
            if (response == null)
            {
                return catchedException;
            }
            return new ApplicationException(
                    $"{response.StatusDescription} {Environment.NewLine} {response.Content}",
                    response.ErrorException);
        }

        ~ChktrClient()
        {
            Dispose(false);
        }
    }
}
