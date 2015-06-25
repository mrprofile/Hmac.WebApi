using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using RestSharp;

namespace Hmac.WebApi.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var request = new RestRequest("values", Method.GET) { RequestFormat = DataFormat.Json };

            var securedResponse = RestClient.Execute<List<string>>(request);

            return View(securedResponse.Data);
        }

        public async Task<ActionResult> IndexAsync()
        {
            var request = new RestRequest("values", Method.GET) { RequestFormat = DataFormat.Json };

            var securedResponse = RestClient.ExecuteAsync<List<string>>(request);
            await securedResponse;

            return new JsonResult()
            {
                Data = securedResponse.Result.Data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
