using Microsoft.Playwright;
using System.Text.Json;

namespace PlaywrightUtils.API.Actions
{
    /// <summary>
    /// RestClient capabilities, inject RestClientManager into Specflow Context
    /// </summary>
    public class RestClientManager
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private IAPIRequestContext _requestContext;
        private IAPIResponse _response;

        public IAPIResponse Response
        {
            get { return _response; }
            private set { _response = value; }
        }

        /// <summary>
        /// Initializes RestClient.
        /// </summary>
        public async Task InitializeRestClientAsync(IPlaywright playwrightObject, string baseUrl)
        {
            Log.Debug("Initialize Rest Client...");

            _requestContext = await playwrightObject.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = baseUrl,
            });
        }

        /// <summary>
        /// Sends HTTP(S) POST request and returns its response.
        /// The method will populate request cookies from the context and update context cookies from the response.
        /// The method will automatically follow redirects.
        /// If the body parameter is an object, it will be serialized to json string 
        /// and content-type header will be set to application/json if not explicitly set. 
        /// Otherwise the content-type header will be set to application/octet-stream if not explicitly set.
        /// </summary>
        /// <param name="url">Target URL.</param>
        /// <param name="body">Allows to set post data of the request.</param> 
        public async Task<IAPIResponse> ExecutePOSTRequestAsync(string url, object body)
        {
            _response = await _requestContext.PostAsync(url, new APIRequestContextOptions
            {
                DataObject = body
            });

            return _response;
        }

        /// <summary>
        /// Sends HTTP(S) PUT request and returns its response.
        /// The method will populate request cookies from the context and update context cookies from the response.
        /// The method will automatically follow redirects.
        /// If the body parameter is an object, it will be serialized to json string 
        /// and content-type header will be set to application/json if not explicitly set. 
        /// Otherwise the content-type header will be set to application/octet-stream if not explicitly set.
        /// </summary>
        /// <param name="url">Target URL.</param>
        /// <param name="body">Allows to set post data of the request.</param> 
        public async Task<IAPIResponse> ExecutePUTRequestAsync(string url, object body)
        {
            _response = await _requestContext.PutAsync(url, new APIRequestContextOptions
            {
                DataObject = body,
            });

            return _response;
        }

        /// <summary>
        /// Sends HTTP(S) PATCH request and returns its response.
        /// The method will populate request cookies from the context and update context cookies from the response.
        /// The method will automatically follow redirects.
        /// If the body parameter is an object, it will be serialized to json string 
        /// and content-type header will be set to application/json if not explicitly set. 
        /// Otherwise the content-type header will be set to application/octet-stream if not explicitly set.
        /// </summary>
        /// <param name="url">Target URL.</param>
        /// <param name="body">Allows to set post data of the request.</param> 
        public async Task<IAPIResponse> ExecutePATCHRequestAsync(string url, object body)
        {
            _response = await _requestContext.PatchAsync(url, new APIRequestContextOptions
            {
                DataObject = body,
            });

            return _response;
        }

        /// <summary>
        /// Sends HTTP(S) GET request and returns its response
        /// The method will populate request cookies from the context and update context
        /// cookies from the response. The method will automatically follow redirects.
        /// </summary>
        /// <param name="url">Target URL.</param>
        public async Task<IAPIResponse> ExecuteGETRequestAsync(string url)
        {
            _response = await _requestContext.GetAsync(url);

            return _response;
        }

        /// <summary>
        /// Sends HTTP(S) DELETE request and returns its response.
        /// The method will populate request cookies from the context and update context
        /// cookies from the response. The method will automatically follow redirects.
        /// </summary>
        /// <param name="url">Target URL.</param>
        public async Task<IAPIResponse> ExecuteDELETERequestAsync(string url)
        {
            _response = await _requestContext.DeleteAsync(url);

            return _response;
        }

        /// <summary>
        /// All responses returned by ApiRequestContext.GetAsync() and similar methods are stored in the memory,
        /// so that you can later call ApiResponse.BodyAsync().
        /// This method discards all stored responses, and makes ApiResponse.BodyAsync() throw "Response disposed" error.
        /// </summary>
        public async Task DisposeRequestContextAsync()
        {
            await _requestContext.DisposeAsync();
        }

        /// <summary>
        /// Deserialize last JSON RestResponse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public async Task<T?> DesrializeJsonResponseAsync<T>()
            where T : class
        {
            var jsonResponse = await _response.JsonAsync();

            if (jsonResponse == null)
            {
                Log.Error("Provided Response object or Response Content is null!");
                throw new NullReferenceException("Provided Response object or Response Content is null!");
            }

            return jsonResponse?.Deserialize<T>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
