namespace System.Web.Http
{
    using System.Linq;
    using System.Net.Http;

    /// <summary>
    /// Various extension methods for the HttpResponseMessage class
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Returns an individual HTTP Header value
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <param name="key">The key of the header value to get</param>
        /// <returns>The header value found</returns>
        public static string GetHeader
            (
                this HttpResponseMessage request,
                string key
            )
        {
            Validate.IsNotNull(request);

            var headerFound = request.Headers.TryGetValues
            (
                key,
                out var keys
            );

            if (false == headerFound)
            {
                return null;
            }
            else
            {
                return keys.First();
            }
        }
    }
}
