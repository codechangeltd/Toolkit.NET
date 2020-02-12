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
        private DateRange() { }

        private DateRange
            (
                DateTime startDate,
                DateTime? endDate = null
            )
        {
            this.StartDate = startDate;
            this.EndDate = endDate ?? DateTime.MaxValue;
        }

        /// <summary>
        /// Creates a new data range start and end dates
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>The result with the date range created</returns>
        public static Result<DateRange> Create
            (
                DateTime startDate,
                DateTime? endDate = null
            )
        {
            if (endDate.HasValue)
            {
                if (endDate.Value < startDate)
                {
                    return Result.Failure<DateRange>
                    (
                        "The end date cannot be before the start date."
                    );
                }
            }

            var range = new DateRange(startDate, endDate);

            return Result.Ok(range);
        }

        /// <summary>
        /// Gets the start date range
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Gets the end date range
        /// </summary>
        public DateTime EndDate { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.StartDate;
            yield return this.EndDate;
        }

        public override string ToString()
        {
            var startDisplay = FormatDate(this.StartDate);
            var endDisplay = FormatDate(this.EndDate);

            string FormatDate(DateTime date)
            {
                return date.ToShortDateString();
            }

            return $"{startDisplay} to {endDisplay}";
        }
    }
}
