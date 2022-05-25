namespace System.Web.Http;

using MimeTypes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Helpers;

public static class ControllerExtensions
{
    /// <summary>
    /// Gets the image that was uploaded to the controller during a request
    /// </summary>
    /// <param name="_">The controller</param>
    /// <returns>The image that was uploaded</returns>
    public static WebImage GetUploadedImage(this ApiController _)
    {
        var image = WebImage.GetImageFromRequest();
        
        if (image == null)
        {
            throw new InvalidOperationException("No image was supplied.");
        }

        return image;
    }

    /// <summary>
    /// Streams a file contents to the response as a HTTP response message
    /// </summary>
    /// <param name="_">The controller reference</param>
    /// <param name="fileContents">The file contents to stream</param>
    /// <param name="contentType">The file content type</param>
    /// <param name="fileName">The file name to display (optional)</param>
    /// <param name="dispositionType">The content disposition type (optional)</param>
    /// <returns>An action result with the streamed file data</returns>
    public static HttpResponseMessage StreamFile
        (
            this ApiController _,
            byte[] fileContents,
            string contentType,
            string? fileName = null,
            string dispositionType = "inline"
        )
    {
        if (fileContents == null || fileContents.Length == 0)
        {
            throw new ArgumentException("An empty file cannot be streamed.");
        }

        if (String.IsNullOrEmpty(fileName))
        {
            var fileExtension = MimeTypeMap.GetExtension(contentType);

            fileName = $"Attachment.{fileExtension}";
        }

        // Remove commas from the file name as they are not supported
        fileName = fileName.Replace(",", String.Empty);

        var result = new HttpResponseMessage(HttpStatusCode.OK);
        var contentTypeHeader = new MediaTypeHeaderValue(contentType);
        
        var dispositionHeader = new ContentDispositionHeaderValue(dispositionType)
        {
            FileName = fileName
        };

        result.Content = new ByteArrayContent(fileContents);
        result.Content.Headers.ContentType = contentTypeHeader;
        result.Content.Headers.ContentLength = fileContents.Length;
        result.Content.Headers.ContentDisposition = dispositionHeader;

        return result;
    }

    /// <summary>
    /// Streams an image contents to the response
    /// </summary>
    /// <param name="controller">The Web API controller</param>
    /// <param name="fileContents">The image file contents</param>
    /// <param name="fileContentType">The image file content type</param>
    /// <returns>A HTTP response message containing the image</returns>
    public static HttpResponseMessage StreamImage(this ApiController _, byte[] fileContents, string fileContentType)
    {
        var result = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(fileContents)
        };

        result.Content.Headers.ContentType = new MediaTypeHeaderValue(fileContentType);

        return result;
    }

    /// <summary>
    /// Streams a single, transparent pixel to the response as an action result
    /// </summary>
    /// <param name="controller">The controller reference</param>
    /// <returns>An action result with the streamed pixel data</returns>
    public static HttpResponseMessage StreamPixel(this ApiController controller)
    {
        var result = new HttpResponseMessage(HttpStatusCode.OK);

        // Below is the base 64 encoding for a transparent GIF
        var content = Convert.FromBase64String("R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==");

        result.Content = new ByteArrayContent(content);
        result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/gif");

        return result;
    }

    /// <summary>
    /// Gets the referrer URL for the current request
    /// </summary>
    /// <param name="controller">The controller reference</param>
    /// <returns>The referrer URL</returns>
    public static string GetReferrerUrl(this ApiController controller)
    {
        Validate.IsNotNull(controller);

        return controller.Request.GetReferrerUrl();
    }

    /// <summary>
    /// Gets the requested URL for the current request
    /// </summary>
    /// <param name="controller">The controller reference</param>
    /// <returns>The requested URL</returns>
    public static string GetRequestUrl(this ApiController controller)
    {
        Validate.IsNotNull(controller);

        return controller.Request.GetRequestUrl();
    }

    /// <summary>
    /// Aggregates the cookie data from a the request headers into a single comma separated string
    /// </summary>
    /// <param name="controller">The controller reference</param>
    /// <returns>A string containing comma separated cookie data values</returns>
    public static string AggregateCookieData(this ApiController controller)
    {
        Validate.IsNotNull(controller);

        return controller.Request.AggregateCookieData();
    }

    /// <summary>
    /// Aggregates the headers from the request into a single line break separated string
    /// </summary>
    /// <param name="controller">The controller reference</param>
    /// <returns>A string containing comma separated header values</returns>
    public static string AggregateHeaders(this ApiController controller)
    {
        Validate.IsNotNull(controller);

        return controller.Request.AggregateHeaders();
    }

    /// <summary>
    /// Gets the user agent from the current request
    /// </summary>
    /// <param name="controller">The controller reference</param>
    /// <returns>The user agent value that was found</returns>
    public static string GetUserAgent(this ApiController controller)
    {
        Validate.IsNotNull(controller);

        return controller.Request.GetUserAgent();
    }
}
