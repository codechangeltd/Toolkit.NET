namespace System.Web.Http;

using System.Net.Http;
using System.Text;

public static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Returns a dictionary of QueryStrings that's easier to work with 
    /// than GetQueryNameValuePairs KevValuePairs collection.
    /// 
    /// If you need to pull a few single values use GetQueryString instead.
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <returns>A dictionary of query string name values</returns>
    public static Dictionary<string, string> GetQueryStrings(this HttpRequestMessage request)
    {
        return request.GetQueryNameValuePairs().ToDictionary
        (
            kv => kv.Key,
            kv => kv.Value,
            StringComparer.OrdinalIgnoreCase
        );
    }

    /// <summary>
    /// Returns an individual query string value
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <param name="key">The key of the query string item to get</param>
    /// <returns>The value of the query string item requested</returns>
    public static string? GetQueryString(this HttpRequestMessage request, string key)
    {
        var queryStrings = request.GetQueryNameValuePairs();

        if (queryStrings == null)
        {
            return default;
        }

        var match = queryStrings.FirstOrDefault(x => String.Compare(x.Key, key, true) == 0);

        if (string.IsNullOrEmpty(match.Value))
        {
            return default;
        }

        return match.Value;
    }

    /// <summary>
    /// Returns an individual HTTP Header value
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <param name="key">The key of the header value to get</param>
    /// <returns>The header value found</returns>
    public static string? GetHeader(this HttpRequestMessage request, string key)
    {
        Validate.IsNotNull(request);

        var headerFound = request.Headers.TryGetValues(key, out IEnumerable<string>? keys);

        if (false == headerFound)
        {
            return null;
        }

        return keys?.First();
    }

    /// <summary>
    /// Retrieves an individual cookie from the cookies collection
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <param name="cookieName">The name of the cookie to get</param>
    /// <returns>The cookie value found</returns>
    public static string? GetCookie(this HttpRequestMessage request, string cookieName)
    {
        Validate.IsNotNull(request);

        var cookies = request.Headers?.GetCookies(cookieName);

        if (cookies != null)
        {
            var cookie = cookies.FirstOrDefault();

            if (cookie != null)
            {
                return cookie[cookieName].Value;
            }
        }

        return default;
    }

    /// <summary>
    /// The referrer URL for the current request
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <returns>The referrer URL</returns>
    public static string? GetReferrerUrl(this HttpRequestMessage request)
    {
        return request?.Headers?.Referrer?.AbsoluteUri;
    }

    /// <summary>
    /// The requested URL for the current request
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <returns>The requested URL</returns>
    public static string? GetRequestUrl(this HttpRequestMessage request)
    {
        return request?.RequestUri?.AbsoluteUri;
    }

    /// <summary>
    /// The user agent from the HTTP request message
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <returns>The user agent</returns>
    public static string? GetUserAgent(this HttpRequestMessage request)
    {
        Validate.IsNotNull(request);

        var userAgentCollection = request.Headers.UserAgent;

        if (userAgentCollection == null || userAgentCollection.Count == 0)
        {
            return default;
        }
        else
        {
            return userAgentCollection.First().ToString();
        }
    }

    /// <summary>
    /// Aggregates the cookie data from a the request headers into a single comma separated string
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <returns>A string containing comma separated cookie data values</returns>
    public static string AggregateCookieData(this HttpRequestMessage request)
    {
        Validate.IsNotNull(request);

        var cookieData = String.Empty;
        var cookieHeaders = request.Headers.GetCookies();

        foreach (var headerValue in cookieHeaders)
        {
            foreach (var cookie in headerValue.Cookies)
            {
                if (false == String.IsNullOrEmpty(cookieData))
                {
                    cookieData += ",";
                }

                cookieData += $"{cookie.Name}={cookie.Value}";
            }
        }

        return cookieData;
    }

    /// <summary>
    /// Aggregates the headers from the request into a single line break separated string
    /// </summary>
    /// <param name="request">The HTTP request message</param>
    /// <returns>A string containing comma separated header values</returns>
    public static string AggregateHeaders(this HttpRequestMessage request)
    {
        Validate.IsNotNull(request);

        var headerBuilder = new StringBuilder();

        foreach (var item in request.Headers.ToList())
        {
            if (item.Value != null)
            {
                var header = String.Empty;

                foreach (var value in item.Value)
                {
                    header += value + ", ";
                }

                // Trim the trailing space and add item to the dictionary
                header = header.TrimEnd(" ".ToCharArray());

                headerBuilder.Append($"{item.Key}: {header}{Environment.NewLine}");
            }
        }

        return headerBuilder.ToString().TrimEnd(Environment.NewLine.ToArray());
    }
}
