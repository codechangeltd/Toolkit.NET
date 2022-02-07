namespace System.Web.Http
{
    using CodeChange.Toolkit.Media;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpContentExtensions
    {
        /// <summary>
        /// Asynchronously parses all posted content into fields and files
        /// </summary>
        /// <param name="postedContent">The HTTP posted content</param>
        /// <returns>The parsed content</returns>
        /// <remarks>
        /// See http://chris.59north.com/post/Uploading-files-using-ASPNET-Web-Api
        /// </remarks>
        public static async Task<HttpPostedData> ParseMultipartAsync(this HttpContent postedContent)
        {
            var provider = await postedContent.ReadAsMultipartAsync().ConfigureAwait(false);

            var files = new Dictionary<string, HttpPostedFile>(StringComparer.InvariantCultureIgnoreCase);
            var fields = new Dictionary<string, HttpPostedField>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var content in provider.Contents)
            {
                var headers = content.Headers;
                var fieldName = headers.ContentDisposition.Name.Trim('"');

                if (false == String.IsNullOrEmpty(headers.ContentDisposition.FileName))
                {
                    var fileContents = await content.ReadAsByteArrayAsync().ConfigureAwait(false);

                    var contentType = headers.ContentType.ToString();
                    var fileName = headers.ContentDisposition.FileName.Trim('"');

                    var postedFile = new HttpPostedFile(fieldName, fileName, fileContents, contentType);

                    files.Add(fieldName, postedFile);
                }
                else
                {
                    var data = await content.ReadAsStringAsync().ConfigureAwait(false);

                    var postedField = new HttpPostedField(fieldName, data);

                    fields.Add(fieldName, postedField);
                }
            }

            return new HttpPostedData(fields, files);
        }

        /// <summary>
        /// Gets the field collection as a name value collection
        /// </summary>
        /// <param name="postedData">The HTTP posted data</param>
        /// <returns>The name value collection</returns>
        public static NameValueCollection AsNameValueCollection(this HttpPostedData postedData)
        {
            Validate.IsNotNull(postedData);

            var collection = new NameValueCollection();

            foreach (var item in postedData.Fields)
            {
                var field = item.Value;

                collection.Add(field.Name, field.Value);
            }

            return collection;
        }

        /// <summary>
        /// Gets the file collection as a dictionary of file containers
        /// </summary>
        /// <param name="postedData">The HTTP posted data</param>
        /// <returns>The file container dictionary</returns>
        public static Dictionary<string, FileContainer> AsFileContainerDictionary(this HttpPostedData postedData)
        {
            Validate.IsNotNull(postedData);

            var dictionary = new Dictionary<string, FileContainer>();

            foreach (var item in postedData.Files)
            {
                var file = item.Value;

                if (file.FileContents.Length > 0)
                {
                    var container = new FileContainer(file.FileContents, file.ContentType, file.Filename);

                    dictionary.Add(item.Key, container);
                }
            }

            return dictionary;
        }
    }
}
