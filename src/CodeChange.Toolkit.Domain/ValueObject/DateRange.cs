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

            return Result.Success(range);
        }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public bool IsWithinRange(DateTime date)
        {
            return date >= this.StartDate && date <= this.EndDate;
        }

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
