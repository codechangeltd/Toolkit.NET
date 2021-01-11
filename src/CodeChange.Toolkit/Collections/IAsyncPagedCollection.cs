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
        /// Asynchronously gets the total number of pages
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <remarks>The total number of pages</remarks>
        Task<int> GetPageCount(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets a page of items from the collection
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of the items from the page</returns>
        Task<PagedResult<T>> GetPage(int pageNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets a page of items from the collection
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of the items from the page</returns>
        Task<PagedResult<T>> this[int page, CancellationToken cancellationToken = default] { get; }

        /// <summary>
        /// Asynchronously gets all pages in the collection
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of collections, each representing a page</returns>
        Task<IEnumerable<PagedResult<T>>> GetAllPages(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets the total number of items in the collection
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <remarks>The total number of items</remarks>
        Task<int> GetItemCount(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets a collection of items for the page number specified
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of the items from the page</returns>
        Task<IEnumerable<T>> GetItems(int pageNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously gets all items in the collection (not paginated)
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of items</returns>
        Task<IEnumerable<T>> GetAllItems(CancellationToken cancellationToken = default);
    }
}
