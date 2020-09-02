namespace CodeChange.Toolkit.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the default implementation for IPagedCollection
    /// </summary>
    /// <typeparam name="T">The type of objects to paginate</typeparam>
    public class PagedCollection<T> : IPagedCollection<T>
    {
        private readonly IQueryable<T> _source;
        private readonly int _totalCount;

        /// <summary>
        /// Constructs the paged collection with the collection data
        /// </summary>
        /// <param name="source">The source of data for the collection</param>
        /// <param name="pageSize">The maximum page size</param>
        public PagedCollection(IEnumerable<T> source, int pageSize)
            : this(source.AsQueryable<T>(), pageSize)
        { }

        /// <summary>
        /// Constructs the paged collection with the collection data
        /// </summary>
        /// <param name="source">The source of data for the collection</param>
        /// <param name="pageSize">The maximum page size</param>
        public PagedCollection(IQueryable<T> source, int pageSize)
        {
            Validate.IsNotNull(source);
            Validate.IsGreaterThan(pageSize, 0);

            _source = source;
            _totalCount = source.Count();

            var pageCount = CalculatePageCount(pageSize, _totalCount);

            this.PageSize = pageSize;
            this.PageCount = pageCount;
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

            var pageCount = 
            (
                (totalCount / pageSize) + (remainder == 0 ? 0 : 1)
            );

            return pageCount;
        }

        /// <summary>
        /// Gets the maximum size of any page
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the total number of pages
        /// </summary>
        public int PageCount { get; }

        /// <summary>
        /// Gets a collection of items at the page number specified
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <returns>A collection of the items from the page</returns>
        public IEnumerable<T> GetPage(int pageNumber)
        {
            Validate.IsGreaterThan(pageNumber, 0);
            Validate.IsLessThan(pageNumber, this.PageCount);

            if (_totalCount == 0)
            {
                return _source;
            }

            var pageSize = this.PageSize;
            var skipCount = pageNumber * pageSize;

            var page = _source.Skip(skipCount).Take(pageSize);

            return page;
        }

        /// <summary>
        /// Gets all pages in the collection
        /// </summary>
        /// <returns>A collection of collections, each representing a page</returns>
        public IEnumerable<(int PageNumber, IEnumerable<T> Items)> GetAllPages()
        {
            var pages = new List<(int, IEnumerable<T>)>();

            for (var number = 1; number <= this.PageCount; number++)
            {
                var page = GetPage(number);

                pages.Add((number, page));
            }

            return pages;
        }

        /// <summary>
        /// Gets a collection of items at the page number specified
        /// </summary>
        /// <param name="page">The page number</param>
        /// <returns>A collection of the items from the page</returns>
        public IEnumerable<T> this[int page]
        {
            get
            {
                return GetPage(page);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _source.GetEnumerator();
        }
    }
}
