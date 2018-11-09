namespace System.Web.Http
{
    /// <summary>
    /// Represents a single posted file
    /// </summary>
    public class HttpPostedFile
    {
        public HttpPostedFile
            (
                string name,
                string filename,
                byte[] fileContents,
                string contentType
            )
        {
            this.FileContents = fileContents;
            this.Name = name;
            this.Filename = filename;
            this.ContentType = contentType;
        }

        /// <summary>
        /// Gets the name of the file
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the original file name
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the file contents
        /// </summary>
        public byte[] FileContents { private set; get; }

        /// <summary>
        /// Gets the content type
        /// </summary>
        public string ContentType { private set; get; }
    }
}
