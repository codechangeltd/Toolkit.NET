namespace CodeChange.Toolkit.Domain
{
    using System;

    /// <summary>
    /// Defines a contract for objects that needing logging information attached to them
    /// </summary>
    public interface ILoggingInfo
    {
        /// <summary>
        /// Gets or sets the date the object was created
        /// </summary>
        DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date the object was last modified
        /// </summary>
        DateTime DateModified { get; set; }
    }
}
