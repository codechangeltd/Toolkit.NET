namespace CodeChange.Toolkit.Collections
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a contract for a paginated collection of objects
    /// </summary>
    /// <typeparam name="T">The type of objects to paginate</typeparam>
    public interface IAsyncPagedCollection<T>
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
        /// Asynchronously gets a collection of items at the page number specified
        /// </summary>
        /// <param name="number">The page number</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of the items from the page</returns>
        Task<IEnumerable<T>> GetPage
        (
            int number,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Asynchronously gets a collection of items at the page number specified
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of the items from the page</returns>
        Task<IEnumerable<T>> this[int page, CancellationToken cancellationToken = default] { get; }

        /// <summary>
        /// Asynchronously gets all pages in the collection
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of collections, each representing a page</returns>
        Task<IEnumerable<(int PageNumber, IEnumerable<T> Items)>> GetAllPages
        (
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Asynchronously gets all items in the collection (not paginated)
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of items</returns>
        Task<IEnumerable<T>> GetAllItems
        (
            CancellationToken cancellationToken = default
        );
    }
}
