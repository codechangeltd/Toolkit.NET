namespace System
{
    /// <summary>
    /// Various extension methods for the TimeSpan class
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Formats the time difference between the two dates specified as a descriptive string
        /// </summary>
        /// <param name="span">The time span</param>
        /// <returns>The formatted time difference</returns>
        public static string ToLongString
            (
                this TimeSpan span
            )
        {
            string template;

            if (span.Days > 0)
            {
                template = "{0} days, {1} hours, {2} minutes and {3} seconds";
            }
            else if (span.Hours > 0)
            {
                template = "{1} hours, {2} minutes and {3} seconds";
            }
            else if (span.Minutes > 0)
            {
                template = "{2} minutes and {3} seconds";
            }
            else
            {
                template = "{3}.{4} seconds";
            }

            return template.With
            (
                span.Days,
                span.Hours,
                span.Minutes,
                span.Seconds,
                span.Milliseconds
            );
        }
    }
}
