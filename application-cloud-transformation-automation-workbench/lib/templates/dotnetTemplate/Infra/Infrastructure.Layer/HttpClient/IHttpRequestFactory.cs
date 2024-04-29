using System.Net.Http;

namespace Infrastructure.Common.HttpClient
{
    public interface IHttpRequestFactory
    {
        Task<HttpResponseMessage> Delete(string requestUri);
        Task<HttpResponseMessage> Delete(string requestUri, string bearerToken);
        Task<HttpResponseMessage> Get(string requestUri);
        Task<HttpResponseMessage> Get(string requestUri, string bearerToken);
        Task<HttpResponseMessage> Patch(string requestUri, object value);
        Task<HttpResponseMessage> Patch(string requestUri, object value, string bearerToken);
        Task<HttpResponseMessage> Post(string requestUri, object value);
        Task<HttpResponseMessage> Post(string requestUri, object value, string bearerToken);
        Task<HttpResponseMessage> PostFile(string requestUri, string filePath, string apiParamName);
        Task<HttpResponseMessage> PostFile(string requestUri, string filePath, string apiParamName, string bearerToken);
        Task<HttpResponseMessage> Put(string requestUri, object value);
        Task<HttpResponseMessage> Put(string requestUri, object value, string bearerToken);
    }
}