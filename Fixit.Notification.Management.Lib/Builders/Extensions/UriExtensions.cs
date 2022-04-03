using System;

namespace Fixit.Notification.Management.Lib.Builders.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        /// Add query parameter to the URL provided
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramName">Name of the query parameter</param>
        /// <param name="paramValue">Value of the query parameter</param>
        /// <returns>Returns the URI with the added queryparam</returns>
        public static Uri AddQueryParam(this Uri url, string paramName, string paramValue)
        {
            UriBuilder baseUri = new UriBuilder(url);
            string queryToAppend = paramName + "=" + paramValue;

            if (baseUri.Query != null && baseUri.Query.Length > 1)
                baseUri.Query = baseUri.Query.Substring(1) + "&" + queryToAppend;
            else
                baseUri.Query = queryToAppend;

            return baseUri.Uri;
        }
    }
}
