using System.Collections.Generic;
using Hmac.WebApi.Core.Models;

namespace Hmac.WebApi.Core.Repositories
{
    public interface IApiClientRepository
    {
        List<ApiClient> GetAllClients();
    }
}