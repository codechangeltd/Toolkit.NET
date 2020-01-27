namespace CodeChange.Toolkit.Domain
{
    /// <summary>
    /// Represents a query filter for aggregates
    /// </summary>
    public class AggregateFilter
    {
        /// <summary>
        /// Gets or sets the range for date created filtering
        /// </summary>
        public DateRange CreatedRange { get; set; }

        /// <summary>
        /// Gets or sets the range for date modified filtering
        /// </summary>
        public DateRange ModifiedRange { get; set; }

        /// <summary>
        /// Gets or sets the maximum page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page number to retrieve
        /// </summary>
        public int? PageNumber { get; set; }
    }
}
