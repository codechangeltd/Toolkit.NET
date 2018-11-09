namespace CodeChange.Toolkit.Media
{
    using System;
    
    /// <summary>
    /// Represents a container for handling the contents of a single file
    /// </summary>
    public class FileContainer
    {
        /// <summary>
        /// Constructs a new file container with the file data
        /// </summary>
        /// <param name="fileContents">The file contents</param>
        /// <param name="contentType">The files content type</param>
        /// <param name="fileName">The file name (optional)</param>
        public FileContainer
            (
                byte[] fileContents,
                string contentType,
                string fileName = null
            )
        {
            Validate.IsNotNull(fileContents);

            this.FileContents = fileContents;
            this.ContentType = contentType;
            this.FileName = fileName;
            this.FileSize = fileContents.Length;

            var isSupported = IsValidContentType(contentType);

            // Make sure the content type is a supported type
            if (false == isSupported)
            {
                throw new ArgumentException
                (
                    $"The file content type '{contentType}' is not valid."
                );
            }
        }

        /// <summary>
        /// Determines if the content type specified is valid for the file
        /// </summary>
        /// <param name="contentType">The content type to check</param>
        /// <returns>True, if the content type is valid; otherwise false</returns>
        protected virtual bool IsValidContentType
            (
                string contentType
            )
        {
            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the file contents as a byte array
        /// </summary>
        public byte[] FileContents { get; private set; }

        /// <summary>
        /// Gets the files content type
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets the file name
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the files size in bytes
        /// </summary>
        public long FileSize { get; private set; }
    }
}
