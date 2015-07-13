using System;

namespace Matcha.WebApi.Controllers
{
    public static class UriExtensions
    {
        public static Uri Combine(this Uri baseUri, object path)
        {
            var baseUrlAsString = baseUri.AbsoluteUri.TrimEnd('/') + "/";
            return new Uri(baseUrlAsString + path.ToString().TrimStart('/'));
        }
    }
}