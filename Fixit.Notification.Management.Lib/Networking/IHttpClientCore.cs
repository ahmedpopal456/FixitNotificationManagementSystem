using System.Threading;
using System.Threading.Tasks;
using Fixit.Notification.Management.Lib.Networking.Local;

namespace Fixit.Notification.Management.Lib.Networking
{
    public interface IHttpClientCore
    {
        /// <summary>
        /// Executes a HTTP GET call and returns response
        /// </summary>
        /// <param name="targetRoute">Target specific route</param>
        /// <param name="cancellationToken"></param>
        /// <param name="parameters">Optional parameters (auth token, headers, etc.) for the request</param>
        /// <returns>HttpResponseMessage</returns>
        Task<T> GetResultAsync<T>(string targetRoute, CancellationToken cancellationToken, HttpProperties parameters = null);

        /// <summary>
        /// Executes a HTTP POST call and returns response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetRoute"></param>
        /// <param name="bodyContent"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="parameters">Optional parameters (auth token, headers, etc.) for the request</param>
        /// <returns></returns>
        Task<T> PostAsync<T>(string targetRoute, object bodyContent, CancellationToken cancellationToken, HttpProperties parameters = null);

        /// <summary>
        /// Executes a HTTP DELETE call and returns response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetRoute"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="parameters">Optional parameters (auth token, headers, etc.) for the request</param>
        /// <returns></returns>
        Task<T> DeleteAsync<T>(string targetRoute, CancellationToken cancellationToken, HttpProperties parameters = null);

        /// <summary>
        /// Executes a HTTP PUT call and returns response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetRoute"></param>
        /// <param name="bodyContent"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="parameters">Optional parameters (auth token, headers, etc.) for the request</param>
        /// <returns></returns>
        Task<T> PutAsync<T>(string targetRoute, object bodyContent, CancellationToken cancellationToken, HttpProperties parameters = null);

        /// <summary>
        /// Build an empty collection of http properties headers
        /// </summary>
        /// <returns>Http Properties with an empty set of headers</returns>
        HttpProperties BuildHttpProperties();
    }
}