using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.WebPages;

namespace Hmac.WebApi.Core
{
    public static class ApiSignature
    {
        /// <summary>
        /// Used by SDK and clients to make requests. Input values are in strings.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="absoluteUri"></param>
        /// <param name="contentType"></param>
        /// <param name="date"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string CreateToken(string method, string absoluteUri, string contentType, string date, string secret)
        {
            return CreateToken(
                FlattenRequestDetails(method,
                    absoluteUri,
                    contentType,
                    date
                    ), secret);
        }

        /// <summary>
        /// Used by SDK and clients to make requests, so we must use the HttpWebRequest class
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string CreateToken(HttpWebRequest webRequest, string secret)
        {
            return CreateToken(
                FlattenRequestDetails(webRequest.Method,
                    webRequest.RequestUri.AbsoluteUri,
                    webRequest.ContentType,
                    webRequest.Date.ToUniversalTime().ToString("r")
                    ), secret);
        }

        public static string CreateToken(HttpRequestMessage webRequest, string secret)
        {
            return CreateToken(
                FlattenRequestDetails(webRequest.Method.Method,
                    webRequest.RequestUri.AbsoluteUri,
                    webRequest.Content.Headers.ContentType == null ? "" : webRequest.Content.Headers.ContentType.MediaType,
                    webRequest.Content.Headers.GetValues("Date").SingleOrDefault().AsDateTime().ToUniversalTime().ToString("r")
                    ), secret);
        }

        private static string CreateToken(string message, string secret)
        {
            // don't allow null secrets
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            var keyByte = encoding.GetBytes(secret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new System.Security.Cryptography.HMACSHA256(keyByte))
            {
                var hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        private static string FlattenRequestDetails(
            string httpMethod, string url, string contentType, string date)
        {
            // If it is a GET then we don't care about the contentType since there will never be contentTypes with GET.
            if (httpMethod.ToUpper() == "GET")
                contentType = "";

            var message = string.Format("{0}{1}{2}{3}", httpMethod, url, contentType, date);

            return message;
        }

        public static string GetDate(HttpHeaders headers)
        {
            return GetHttpRequestHeader(headers, ApiCustomHttpHeaders.Date);
        }

        public static string GetApiKey(HttpHeaders headers)
        {
            return GetHttpRequestHeader(headers, ApiCustomHttpHeaders.ApiKey);
        }

        public static string GetSignature(HttpHeaders headers)
        {
            return GetHttpRequestHeader(headers, ApiCustomHttpHeaders.Signature);
        }

        private static string GetHttpRequestHeader(HttpHeaders headers, string headerName)
        {
            if (!headers.Contains(headerName))
                return string.Empty;

            return headers.GetValues(headerName).SingleOrDefault();
        }
    }
}