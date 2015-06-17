using System.Threading.Tasks;
using RestSharp;

namespace Hmac.WebApi.Core.ApiLibrary
{
    public class EsqtvApi
    {
        private readonly string _publicKey;
        private readonly string _privateKey;
        private readonly string _baseUrl;
        private readonly RestClient _client;

        public EsqtvApi(string publicKey, string privateKey, string serviceUrl)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
            _baseUrl = serviceUrl;

            _client = new RestClient(_baseUrl){ Timeout = 30000 };
        }

        public IRestResponse<T> Execute<T>(IRestRequest request) where T : new()
        {
            request.Parameters.GenerateAuthenticationHeader(request.Method.GetMethod(), _baseUrl + request.Resource, _publicKey, _privateKey);

            var restResponse = _client.Execute<T>(request);

            return restResponse;
        }

        public async Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request) where T : new()
        {
            request.Parameters.GenerateAuthenticationHeader(request.Method.GetMethod(), _baseUrl + request.Resource, _publicKey, _privateKey);

            var restResponse = await _client.ExecuteTaskAsync<T>(request);
            
            return restResponse;
        }
    }
}
