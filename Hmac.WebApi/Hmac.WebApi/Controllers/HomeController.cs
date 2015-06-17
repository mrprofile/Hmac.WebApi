using System.Collections.Generic;
using System.Web.Mvc;
using Hmac.WebApi.Core;
using Hmac.WebApi.Core.ApiLibrary;
using RestSharp;

namespace Hmac.WebApi.Controllers
{
    public class BaseController : Controller
    {
        public EsqtvApi RestClient { get; set; }
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var client = new EsqtvApi("75a6e149019f417b92001278c7d780f3", "c7b064ebd5a64a88b40811b836374bac", "http://localhost:44388/api/");

            var request = new RestRequest("values", Method.GET) { RequestFormat = DataFormat.Json };

            var securedResponse = client.Execute<List<string>>(request);

            return View(securedResponse.Data);
        }
    }
}
