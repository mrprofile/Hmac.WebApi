namespace Hmac.WebApi.Core.Models
{
    public class PageResult<T> where T : class
    {
        public string Elapsed { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int ItemCount { get; set; }
        public int CurrentPage { get; set; }

        public T Result { get; set; }
        public dynamic Facets { get; set; }
    }
}