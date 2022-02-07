namespace System.Web.Http
{
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// Represents the posted data split into fields and files
    /// </summary>
    public class HttpPostedData
    {
        public HttpPostedData(IDictionary<string, HttpPostedField> fields, IDictionary<string, HttpPostedFile> files)
        {
            this.Fields = fields;
            this.Files = files;
        }

        /// <summary>
        /// Gets the fields in the posted data
        /// </summary>
        public IDictionary<string, HttpPostedField> Fields { get; }

        /// <summary>
        /// Gets the files in the posted data
        /// </summary>
        public IDictionary<string, HttpPostedFile> Files { get; }

        /// <summary>
        /// Gets a specific file that is required
        /// </summary>
        /// <param name="name">The name of the file to get</param>
        /// <returns>The matching file</returns>
        public HttpPostedFile GetRequiredFile(string name)
        {
            if (this.Files.ContainsKey(name))
            {
                return this.Files[name];
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }
    }
}
