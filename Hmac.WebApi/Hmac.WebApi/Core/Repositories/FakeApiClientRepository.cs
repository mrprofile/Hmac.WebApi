using System.Collections.Generic;
using Hmac.WebApi.Controllers;
using Hmac.WebApi.Core.Models;

namespace Hmac.WebApi.Core.Repositories
{
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
}