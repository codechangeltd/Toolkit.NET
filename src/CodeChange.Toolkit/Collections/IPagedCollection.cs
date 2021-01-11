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
        /// Gets the total number of items in the collection
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        /// Gets a specific page from the collection
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <returns>A paged result</returns>
        PagedResult<T> GetPage(int pageNumber);

        /// <summary>
        /// Gets all pages in the collection
        /// </summary>
        /// <returns>A collection of paged results</returns>
        IEnumerable<PagedResult<T>> GetAllPages();

        /// <summary>
        /// Gets the items within a specific page
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <returns>A collection of the items from the page</returns>
        IEnumerable<T> GetItems(int pageNumber);

        /// <summary>
        /// Gets all the items in the collection
        /// </summary>
        /// <returns>A collection of items of type T</returns>
        IEnumerable<T> GetAllItems();

        /// <summary>
        /// Gets a specific page from the collection
        /// </summary>
        /// <param name="page">The page number</param>
        /// <returns>A paged result</returns>
        PagedResult<T> this[int page] { get; }
    }
}
