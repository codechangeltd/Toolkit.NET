namespace CodeChange.Toolkit.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a contract for a paginated collection of objects
    /// </summary>
    /// <typeparam name="T">The type of objects to paginate</typeparam>
    public interface IPagedCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the maximum size of any page
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the total number of pages
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Gets a collection of items at the page number specified
        /// </summary>
        /// <param name="number">The page number</param>
        /// <returns>A collection of the items from the page</returns>
        IEnumerable<T> GetPage
        (
            int number
        );

        /// <summary>
        /// Gets all pages in the collection
        /// </summary>
        /// <returns>A collection of collections, each representing a page</returns>
        IEnumerable<(int PageNumber, IEnumerable<T> Items)> GetAllPages();

        /// <summary>
        /// Gets a collection of items at the page number specified
        /// </summary>
        /// <param name="page">The page number</param>
        /// <returns>A collection of the items from the page</returns>
        IEnumerable<T> this[int page] { get; }
    }
}
