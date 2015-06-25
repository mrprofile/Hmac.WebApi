using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Security;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.WebPages;
using Hmac.WebApi.Core;
using Hmac.WebApi.Core.Models;
using Hmac.WebApi.Core.Repositories;

namespace Hmac.WebApi.Filters
{
    public class PagingAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var query = actionContext.Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "query").Value;
            var limit = actionContext.Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "limit").Value;
            var offset = actionContext.Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "offset").Value;
            var dtoRequest = actionContext.ActionArguments.ContainsKey("request") ? actionContext.ActionArguments["request"] : "";

            var dto = dtoRequest as QueryBase;
            if (dto == null) { return; }
            dto.Query = query;
            dto.Limit = limit.IsEmpty() ? 24 : int.Parse(limit);
            dto.Offset = offset.IsEmpty() ? 0 : int.Parse(offset);
            /*var dto = new QueryBase
            {
                Query = query,
                Limit = String.IsNullOrEmpty(limit) ? 24 : int.Parse(limit),
                Offset = String.IsNullOrEmpty(offset) ? 0 : int.Parse(offset)
            };*/

           
        }
    }
    public class AuthenticateAttribute : ActionFilterAttribute
    {
        public IApiClientRepository FakeApiClientRepository { get; set; }

        public AuthenticateAttribute()
        {
            FakeApiClientRepository = new FakeApiClientRepository();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var authErrorMessage = "";

            try
            {
                if (IsAuthenticated(request))
                    return;
            }
            catch (SecurityException ex)
            {
                authErrorMessage = ex.Message;
            }

            var errorResponse = new
            {
                errorMessage = authErrorMessage,
                statusCode = HttpStatusCode.Unauthorized
            };

            actionContext.Response = request.CreateResponse(HttpStatusCode.Unauthorized, errorResponse);
        }

        internal bool IsAuthenticated(HttpRequestMessage request)
        {
            DateTime requestDate;
            if (!DateTime.TryParse(ApiSignature.GetDate(request.Headers), out requestDate))
                throw new SecurityException("You must provide a valid request date in the headers.");

            var difference = requestDate.Subtract(DateTime.Now);
            if (difference.TotalMinutes > 15 || difference.TotalMinutes < -15)
            {
                throw new SecurityException(string.Format(
                    "The request timestamp must be within 15 minutes of the server time. Your request is {0} minutes compared to the server. Server time is currently {1} {2}",
                    difference.TotalMinutes,
                    DateTime.Now.ToLongDateString(),
                    DateTime.Now.ToLongTimeString()));
            }

            var apiKey = ApiSignature.GetApiKey(request.Headers);
            if (String.IsNullOrEmpty(apiKey))
                throw new SecurityException("You must provide a valid API Key with your request");

            var signature = ApiSignature.GetSignature(request.Headers);
            if (string.IsNullOrEmpty(signature))
                throw new SecurityException("You must provide a valid request signature (hash)");

            var memoryCache = MemoryCache.Default;
            var users = memoryCache.Get("esq:apiclient:all") as List<ApiClient>;

            if (users == null)
            {
                users = FakeApiClientRepository.GetAllClients();

                var expiration = DateTimeOffset.UtcNow.AddMinutes(5);
                memoryCache.Add("esq:apiclient:all", users, expiration);
            }

            var user = users.FirstOrDefault(x => x.ApiKey == apiKey);

            if (user == null)
                throw new SecurityException("Your API Key could not be found.");

            if (!user.IsActive)
                throw new SecurityException("Your API user account has been disabled.");

            if (signature == ApiSignature.CreateToken(request.Method.Method,
                request.RequestUri.AbsoluteUri,
                request.Content.Headers.ContentType == null ? "" : request.Content.Headers.ContentType.MediaType,
                requestDate.ToUniversalTime().ToString("r"), user.Secret))
                return true;

            throw new SecurityException("Your request signature (hash) is invalid.");
        }
    }
}