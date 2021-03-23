
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Networking.Local;
using static Fixit.Notification.Management.Lib.Networking.Utils.StringUtils;
using System.Linq;

namespace Fixit.Notification.Management.Lib.Networking
{
    public class HttpClientCore : IHttpClientCore
    {
        private readonly string _serviceEndpoint;

        private readonly HttpClient _httpClient;

        public HttpClientCore(HttpClient httpClient,
                              string serviceEndpoint)
        {
            if (string.IsNullOrEmpty(serviceEndpoint))
            {
                throw new ArgumentNullException(nameof(serviceEndpoint));
            }

            _httpClient = httpClient ?? throw new ArgumentNullException($"{nameof(HttpClientCore)} expects a value for {nameof(httpClient)}... null argument was provided");

            _serviceEndpoint = TrimCharacter(serviceEndpoint);
        }

        public async Task<T> GetResultAsync<T>(string targetRoute, CancellationToken cancellationToken, HttpProperties parameters = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_serviceEndpoint}/{targetRoute.Trim('/')}");

            AddHeadersAndToken(ref request, parameters);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            T result = default;
            if (response.IsSuccessStatusCode)
            {
                var returnValue = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<T>(returnValue);
            }
            return result;
        }

        public async Task<T> PostAsync<T>(string targetRoute, object bodyContent, CancellationToken cancellationToken, HttpProperties parameters = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_serviceEndpoint}/{targetRoute.Trim('/')}");

            AddHeadersAndToken(ref request, parameters);

            request.Content = GetContent(bodyContent);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            T result = default;
            if (response.IsSuccessStatusCode)
            {
                var returnValue = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<T>(returnValue);
            }
            return result;
        }

        public async Task<T> PutAsync<T>(string targetRoute, object bodyContent, CancellationToken cancellationToken, HttpProperties parameters = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{_serviceEndpoint}/{targetRoute.Trim('/')}");

            AddHeadersAndToken(ref request, parameters);

            request.Content = new StringContent(JsonConvert.SerializeObject(bodyContent), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            T result = default;
            if (response.IsSuccessStatusCode)
            {
                var returnValue = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<T>(returnValue);
            }
            return result;
        }

        public async Task<T> DeleteAsync<T>(string targetRoute, CancellationToken cancellationToken, HttpProperties parameters = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_serviceEndpoint}/{targetRoute.Trim('/')}");

            AddHeadersAndToken(ref request, parameters);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            T result = default;
            if (response.IsSuccessStatusCode)
            {
                var returnValue = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<T>(returnValue);
            }
            return result;
        }

        public HttpProperties BuildHttpProperties()
        {
            return new HttpProperties { Headers = new List<HeaderDto>() };
        }

        #region Helpers 

        private HttpContent GetContent(object iBodyContent)
        {
            return iBodyContent is FormUrlEncodedContent formUrlEncodedContent
              ? (HttpContent)formUrlEncodedContent
              : new StringContent(JsonConvert.SerializeObject(iBodyContent), Encoding.UTF8, "application/json");
        }

        private void AddHeadersAndToken(ref HttpRequestMessage request, HttpProperties properties)
        {
            if (properties != null && properties.Headers != null && properties.Headers.Any())
            {
                foreach (var header in properties.Headers)
                {
                    request.Headers.Add(header.Name, header.Value);
                }
            }
        }

        #endregion
    }
}
