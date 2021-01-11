namespace CodeChange.Toolkit.EntityFrameworkCore
{
    using CodeChange.Toolkit.Collections;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An Entity Framework Core implementation of IAsyncPagedCollection
    /// </summary>
    /// <typeparam name="T">The type of objects to paginate</typeparam>
    public class AsyncPagedCollection<T> : IAsyncPagedCollection<T>
    {
        private readonly IQueryable<T> _source;

        private int? _cachedPageCount;
        private readonly Dictionary<int, IEnumerable<T>> _cachedItems;

        private int? _cachedTotalCount;
        private IEnumerable<T> _cachedAllItems;

        /// <summary>
        /// Constructs the paged collection with the collection data
        /// </summary>
        /// <param name="source">The source of data for the collection</param>
        /// <param name="pageSize">The maximum page size</param>
        public AsyncPagedCollection(IQueryable<T> source, int pageSize)
        {
            Validate.IsNotNull(source);
            Validate.IsGreaterThan(pageSize, 0);

            _source = source;
            _cachedItems = new Dictionary<int, IEnumerable<T>>();

            this.PageSize = pageSize;
        }

        /// <summary>
        /// Gets the maximum size of any page
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Asynchronously gets the total number of pages
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <remarks>The total number of pages</remarks>
        public async Task<int> GetPageCount(CancellationToken cancellationToken = default)
        {
            if (_cachedPageCount == null)
            {
                var pageSize = this.PageSize;
                var totalCount = await GetItemCount(cancellationToken).ConfigureAwait(false);

                _cachedPageCount = CalculatePageCount(pageSize, totalCount);
            }

            return _cachedPageCount.Value;
        }

        /// <summary>
        /// Calculates the page count from the page and collection sizes
        /// </summary>
        /// <param name="pageSize">The page size</param>
        /// <param name="totalCount">The number of items in total</param>
        /// <returns>The page count</returns>
        protected int CalculatePageCount(int pageSize, int totalCount)
        {
            if (totalCount == 0)
            {
                return 0;
            }

            var remainder = totalCount % pageSize;
            var pageCount = ((totalCount / pageSize) + (remainder == 0 ? 0 : 1));

            return pageCount;
        }

        /// <summary>
        /// Asynchronously gets a page of items from the collection
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of the items from the page</returns>
        public async Task<PagedResult<T>> GetPage(int pageNumber, CancellationToken cancellationToken = default)
        {
            var items = await GetItems(pageNumber, cancellationToken).ConfigureAwait(false);
            var pageCount = await GetPageCount(cancellationToken).ConfigureAwait(false);
            var itemCount = await GetItemCount(cancellationToken).ConfigureAwait(false);

            return new PagedResult<T>(pageNumber, pageCount, this.PageSize, itemCount, items);
        }

        /// <summary>
        /// Asynchronously gets a page of items from the collection
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of the items from the page</returns>
        public Task<PagedResult<T>> this[int page, CancellationToken cancellationToken = default]
        {
            get
            {
                return GetPage(page, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously gets all pages in the collection
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of collections, each representing a page</returns>
        public async Task<IEnumerable<PagedResult<T>>> GetAllPages(CancellationToken cancellationToken = default)
        {
            var pages = new List<PagedResult<T>>();

            var allItems = await GetAllItems(cancellationToken).ConfigureAwait(false);
            var pageCount = await GetPageCount(cancellationToken).ConfigureAwait(false);
            var itemCount = allItems.Count();

            for (var pageNumber = 1; pageNumber <= pageCount; pageNumber++)
            {
                var pageSize = this.PageSize;
                var skipCount = (pageNumber * pageSize);
                var pageItems = allItems.Skip(skipCount).Take(pageSize);

                var page = new PagedResult<T>(pageNumber, pageCount, pageSize, itemCount, pageItems);

                pages.Add(page);
            }

            return pages;
        }

        /// <summary>
        /// Asynchronously gets the total number of items in the collection
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <remarks>The total number of items</remarks>
        public async Task<int> GetItemCount(CancellationToken cancellationToken = default)
        {
            if (_cachedTotalCount == null)
            {
                _cachedTotalCount = await _source.CountAsync(cancellationToken).ConfigureAwait(false);
            }

            return _cachedTotalCount.Value;
        }

        /// <summary>
        /// Asynchronously gets a collection of items for the page number specified
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of the items from the page</returns>
        public async Task<IEnumerable<T>> GetItems(int pageNumber, CancellationToken cancellationToken = default)
        {
            Validate.IsGreaterThan(pageNumber, 0);

            if (_cachedItems.ContainsKey(pageNumber))
            {
                return _cachedItems[pageNumber];
            }
            else
            {
                var totalCount = await GetItemCount(cancellationToken).ConfigureAwait(false);

                if (totalCount == 0)
                {
                    return _source;
                }
                else
                {
                    var pageSize = this.PageSize;
                    var skipCount = (pageNumber * pageSize);

                    var items = await _source
                        .Skip(skipCount)
                        .Take(pageSize)
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);

                    _cachedItems[pageNumber] = items;

                    return items;
                }
            }
        }

        /// <summary>
        /// Asynchronously gets all items in the collection (not paginated)
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A collection of items</returns>
        public async Task<IEnumerable<T>> GetAllItems(CancellationToken cancellationToken = default)
        {
            if (_cachedAllItems == null)
            {
                _cachedAllItems = await _source.ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            return _cachedAllItems;
        }
    }
}
