namespace CodeChange.Toolkit.Media
{
    using System;
    
    /// <summary>
    /// Represents a container for a single image file
    /// </summary>
    public class ImageFileContainer : FileContainer
    {
        /// <summary>
        /// Constructs a new image file container with the file contents, content type and file name
        /// </summary>
        /// <param name="fileContents">The file contents</param>
        /// <param name="contentType">The files content type</param>
        /// <param name="fileName">The file name (optional)</param>
        public ImageFileContainer(byte[] fileContents, string contentType, string fileName = null)
            : base(fileContents, contentType, fileName)
        { }

        /// <summary>
        /// Determines if the content type specified is valid for the file
        /// </summary>
        /// <param name="contentType">The content type to check</param>
        /// <returns>True, if the content type is valid; otherwise false</returns>
        protected override bool IsValidContentType(string contentType)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }
            else
            {
                contentType = contentType.ToLower();

                return contentType == "image/jpeg"
                    || contentType == "image/pjpeg"
                    || contentType == "image/png"
                    || contentType == "image/gif"
                    || contentType == "image/svg+xml";
            }
        }
    }
}
