using System;
using System.Collections.Generic;
using RestSharp;

namespace Hmac.WebApi.Core.ApiLibrary
{
    public static class ParameterExtensions
    {
        public static List<Parameter> GenerateAuthenticationHeader(this List<Parameter> header, string httpMethod, string absoluteUri,
            string publicKey, string privateKey)
        {
            var date = DateTime.Now.ToUniversalTime().ToString("r");
            var token = ApiSignature.CreateToken(httpMethod, absoluteUri, "application/json", date, privateKey);

            header.Add(new Parameter()
            {
                Name = ApiCustomHttpHeaders.ApiKey,
                Type = ParameterType.HttpHeader,
                Value = publicKey
            });
            header.Add(new Parameter()
            {
                Name = ApiCustomHttpHeaders.Signature,
                Type = ParameterType.HttpHeader,
                Value = token
            });
            header.Add(new Parameter()
            {
                Name = ApiCustomHttpHeaders.Date,
                Type = ParameterType.HttpHeader,
                Value = date
            });

            return header;
        }
    }
}