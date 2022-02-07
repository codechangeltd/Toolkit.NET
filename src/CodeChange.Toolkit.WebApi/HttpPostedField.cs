namespace System.Web.Http
{
    /// <summary>
    /// Represents a single posted field
    /// </summary>
    public class HttpPostedField
    {
        public HttpPostedField(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the field
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the field value
        /// </summary>
        public string Value { get; }
    }
}
