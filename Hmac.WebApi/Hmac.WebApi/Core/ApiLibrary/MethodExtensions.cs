using RestSharp;

namespace Hmac.WebApi.Core.ApiLibrary
{
    public static class MethodExtensions
    {
        public static string GetMethod(this Method value)
        {
            string method;

            switch (value)
            {
                case Method.GET:
                    method = "GET";
                    break;
                case Method.POST:
                    method = "POST";
                    break;
                case Method.PUT:
                    method = "PUT";
                    break;
                case Method.DELETE:
                    method = "DELETE";
                    break;
                case Method.HEAD:
                    method = "HEAD";
                    break;
                case Method.OPTIONS:
                    method = "OPTIONS";
                    break;
                default:
                    method = "PATCH";
                    break;
            }

            return method;
        }
    }
}