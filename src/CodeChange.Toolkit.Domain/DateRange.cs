namespace CodeChange.Toolkit.Domain
{
    using CSharpFunctionalExtensions;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a single date range
    /// </summary>
    public class DateRange : ValueObject
    {
        /// <summary>
        /// Constructs the data range with the start and end dates
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        public DateRange
            (
                DateTime startDate,
                DateTime? endDate = null
            )
        {
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        /// <summary>
        /// Gets the start date range
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Gets the end date range
        /// </summary>
        public DateTime? EndDate { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.StartDate;
            yield return this.EndDate;
        }
    }
}
