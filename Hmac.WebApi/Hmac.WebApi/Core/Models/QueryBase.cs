namespace Hmac.WebApi.Core.Models
{
    public class QueryBase
    {
        public string Query { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Order { get; set; }
        public string Sort { get; set; }
    }
}