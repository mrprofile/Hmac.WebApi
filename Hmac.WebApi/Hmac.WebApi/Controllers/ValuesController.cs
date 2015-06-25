using System.Net;
using System.Net.Http;
using System.Web.Http;
using Hmac.WebApi.Core.Models;
using Hmac.WebApi.Filters;

namespace Hmac.WebApi.Controllers
{
    public class MyRequest : QueryBase
    {
        public string Data { get; set; }
    }

    public class ApiBaseController : ApiController
    {

        public HttpResponseMessage Ok()
        {
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        public HttpResponseMessage Ok(object data)
        {
            var httpresponse = Request.CreateResponse(HttpStatusCode.OK, data);

            return httpresponse;
        }

    }
    public class ValuesController : ApiBaseController
    {
        [PagingAttribute]
        public HttpResponseMessage Get(MyRequest request)
        {

            var response = new[] { "value1", "value2" };

            return Ok(response);
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