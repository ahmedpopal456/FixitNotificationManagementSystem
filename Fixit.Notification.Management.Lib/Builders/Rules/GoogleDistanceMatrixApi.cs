using Fixit.Notification.Management.Lib.Builders.Extensions;
using Fixit.Notification.Management.Lib.Models.Classification;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Fixit.Notification.Management.Lib.Builders.Rules
{
    public static class GoogleDistanceMatrixApi
    {
        /// <summary>
        /// Get the value for the distance and duration from Google Direction Matrix API
        /// </summary>
        /// <param name="distanceMatrixUri">Google distance matrix URI</param>
        /// <param name="origin">Location address of the craftsman</param>
        /// <param name="destination">Location address of the fix request</param>
        /// <param name="googleApiKey">Google API key</param>
        /// <returns>Returns GoogleDistanceMatrixResponse object</returns>
        public static GoogleDistanceMatrixResponse GetLocationDistanceAndDuration(string distanceMatrixUri, string origin, string destination, string googleApiKey)
        {
            var requestValues = default(GoogleDistanceMatrixResponse);
            Uri url = new Uri(distanceMatrixUri);
            string requestUriString = @UriExtensions.AddQueryParam(url, "origins", origin)
                                                    .AddQueryParam("destinations", destination)
                                                    .AddQueryParam("key", googleApiKey).ToString();

            WebRequest request = WebRequest.Create(requestUriString);
            using (WebResponse response = request.GetResponse())
            {
                Stream data = response.GetResponseStream();
                using StreamReader reader = new StreamReader(data);
                string responseFromServer = reader.ReadToEnd();
                requestValues = JsonConvert.DeserializeObject<GoogleDistanceMatrixResponse>(responseFromServer);
            }

            return requestValues;
        }
    }
}
