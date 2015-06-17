using System;

namespace Hmac.WebApi.Core.Models
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
}