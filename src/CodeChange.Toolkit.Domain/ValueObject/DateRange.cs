namespace CodeChange.Toolkit.Domain
{
    /// <summary>
    /// Represents a single date range value
    /// </summary>
    public class DateRange : ValueObject, ICloneable
    {
        protected DateRange() { }

        private DateRange(DateTime startDate, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate ?? DateTime.MaxValue;
        }

        public static Result<DateRange> Create(DateTime startDate, DateTime? endDate = null)
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

            return new DateRange(startDate, endDate);
        }

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public bool IsWithinRange(DateTime date)
        {
            return date >= StartDate && date <= EndDate;
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
        }

        public object Clone() => MemberwiseClone();

        public override string ToString()
        {
            var startDisplay = FormatDate(StartDate);
            var endDisplay = FormatDate(EndDate);

            static string FormatDate(DateTime date)
            {
                return date.ToShortDateString();
            }

            return $"{startDisplay} to {endDisplay}";
        }
    }
}
