namespace CodeChange.Toolkit.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// Represents the result of a single page from a paged collection
    /// </summary>
    /// <typeparam name="T">The paged object type</typeparam>
    public class PagedResult<T> : IEnumerable<T>
    {
        public PagedResult(int pageNumber, int pageCount, int pageSize, int itemCount, IEnumerable<T> results)
        {
            Validate.IsNotNull(results);

            this.CurrentPageNumber = pageNumber;
            this.PageCount = pageCount;
            this.PageSize = pageSize;
            this.TotalItemCount = itemCount;
            this.Results = results;
        }

        internal protected PagedResult(IPagedCollection<T> collection, int pageNumber)
        {
            Validate.IsNotNull(collection);

            this.CurrentPageNumber = pageNumber;
            this.PageCount = collection.PageCount;
            this.PageSize = collection.PageSize;
            this.TotalItemCount = collection.ItemCount;
            this.Results = collection.GetItems(pageNumber);
        }

        /// <summary>
        /// Gets the current page number
        /// </summary>
        public int CurrentPageNumber { get; }

        /// <summary>
        /// Gets the page count (for the page size)
        /// </summary>
        public int PageCount { get; }

        /// <summary>
        /// Gets the page size (number of objects per page)
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the total number of items in all pages
        /// </summary>
        public int TotalItemCount { get; }

        /// <summary>
        /// Gets the objects in the page
        /// </summary>
        public IEnumerable<T> Results { get; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection</returns>
        public IEnumerator<T> GetEnumerator() => this.Results.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.Results.GetEnumerator();

        /// <summary>
        /// Gets a description of the paged result
        /// </summary>
        /// <returns>A page indicator description</returns>
        public override string ToString()
        {
            return $"Page {this.CurrentPageNumber} of {this.PageCount}";
        }
    }
}
