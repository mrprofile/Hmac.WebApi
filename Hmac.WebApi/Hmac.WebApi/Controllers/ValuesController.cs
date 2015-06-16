using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Security;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Hmac.WebApi.Core;

namespace Hmac.WebApi.Controllers
{
    public class ApiClient
    {
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Application { get; set; }
    }

    public interface IApiClientRepository
    {
        List<ApiClient> GetAllClients();
    }

    public class ApiClientRepository : IApiClientRepository
    {
        public List<ApiClient> GetAllClients()
        {
            return null;
        }
    }

    public class FakeApiClientRepository : IApiClientRepository
    {
        public List<ApiClient> GetAllClients()
        {
            var returnValue = new List<ApiClient>
            {
                new ApiClient()
                {
                    ApiKey = "75a6e149019f417b92001278c7d780f3",
                    Secret = "c7b064ebd5a64a88b40811b836374bac",
                    IsActive = true,
                    Application = "CMS Inception"
                },
                new ApiClient()
                {
                    ApiKey = "63a6e149019f417b92001278c7d780b6",
                    Secret = "c7b064ebd5a64a88b40811b836374bac",
                    IsActive = true,
                    Application = "CMS Angular"
                }
            };

            return returnValue;
        }
    }

    public class AuthenticateAttribute : ActionFilterAttribute
    {
        public FakeApiClientRepository FakeApiClientRepository { get; set; }

        public AuthenticateAttribute()
        {
            FakeApiClientRepository = new FakeApiClientRepository();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var response = actionContext.Response;
            var authErrorMessage = "";

            try
            {
                if (IsAuthenticated(request, response))
                    return;
            }
            catch (SecurityException ex)
            {
                authErrorMessage = ex.Message;
            }

            var errorResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = authErrorMessage };
            actionContext.Response = errorResponse;
        }

        internal bool IsAuthenticated(HttpRequestMessage request, HttpResponseMessage response)
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

            if (signature == ApiSignature.CreateToken(  request.Method.Method,
                                                        request.RequestUri.AbsoluteUri,
                                                        request.Content.Headers.ContentType == null ? "" : request.Content.Headers.ContentType.MediaType,
                                                        requestDate.ToUniversalTime().ToString("r"), user.Secret))
                return true;

            throw new SecurityException("Your request signature (hash) is invalid.");
        }
    }


    public class ValuesController : ApiController
    {
        [AuthenticateAttribute]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}