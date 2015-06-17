using System.Web.Mvc;
using Hmac.WebApi.Core.ApiLibrary;

namespace Hmac.WebApi.Controllers
{
    public class BaseController : Controller
    {
        private EsqtvApi _restClient;
        protected EsqtvApi RestClient
        {
            get
            {
                return _restClient ?? (_restClient = new EsqtvApi("75a6e149019f417b92001278c7d780f3", "c7b064ebd5a64a88b40811b836374bac", "http://localhost:44388/api/"));
            }
            set
            {
                _restClient = value;
            }
        }
    }
}