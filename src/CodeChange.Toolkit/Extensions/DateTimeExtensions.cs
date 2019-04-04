namespace System
{
    using CodeChange.Toolkit.Culture;

    /// <summary>
    /// Various extension methods for manipulating date time values
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Returns a new System.DateTime that adds the specified number of weeks 
        ///     to the value of this instance.
        /// </summary>
        /// <param name="dateTime">
        ///     The date time value to add the weeks to
        /// </param>
        /// <param name="value">
        ///     A number of whole and fractional weeks. 
        ///     The value parameter can be negative or positive.
        /// </param>
        /// <returns>
        ///     An object whose value is the sum of the date and time represented by this 
        ///     instance and the number of weeks represented by value.
        /// </returns>
        public static DateTime AddWeeks
            (
                this DateTime dateTime,
                double value
            )
        {
            return dateTime.AddDays(value * 7);
        }

        /// <summary>
        ///     Converts the value of the current System.DateTime object to local time.
        /// </summary>
        /// <param name="dateTime">
        ///     The date time to convert
        /// </param>
        /// <returns>
        ///     An object whose System.DateTime.Kind property is System.DateTimeKind.Local,
        ///     and whose value is the local time equivalent to the value of the current
        ///     System.DateTime object, or System.DateTime.MaxValue if the converted value
        ///     is too large to be represented by a System.DateTime object, or System.DateTime.MinValue
        ///     if the converted value is too small to be represented as a System.DateTime
        ///     object.
        /// </returns>
        public static DateTime? ToLocalTime
            (
                this DateTime? dateTime
            )
        {
            return dateTime.HasValue ? dateTime.Value.ToLocalTime() : dateTime;
        }

        /// <summary>
        /// Converts a date to local time
        /// </summary>
        /// <param name="date">The date convert</param>
        /// <param name="localeConfiguration">The locale configuration</param>
        /// <returns>The date in local time</returns>
        public static DateTime ToLocalTime
            (
                this DateTime date,
                ILocaleConfiguration localeConfiguration
            )
        {
            if (date.HasTime())
            {
                if (localeConfiguration.TimeZoneOffset.HasValue)
                {
                    var offset =
                    (
                        localeConfiguration.TimeZoneOffset.Value * -1
                    );

                    date = date.AddMinutes(offset);
                }
                else if (localeConfiguration.DefaultTimeZone != null)
                {
                    date = TimeZoneInfo.ConvertTimeFromUtc
                    (
                        date,
                        localeConfiguration.DefaultTimeZone
                    );
                }
                else
                {
                    date = date.ToLocalTime();
                }
            }

            date = DateTime.SpecifyKind
            (
                date,
                DateTimeKind.Unspecified
            );

            return date;
        }

        /// <summary>
        /// Converts a nullable date to local time
        /// </summary>
        /// <param name="date">The date convert</param>
        /// <param name="localeConfiguration">The locale configuration</param>
        /// <returns>The date in local time</returns>
        public static DateTime? ToLocalTime
            (
                this DateTime? date,
                ILocaleConfiguration localeConfiguration
            )
        {
            if (date.HasValue)
            {
                return ToLocalTime
                (
                    date.Value,
                    localeConfiguration
                );
            }
            else
            {
                return date;
            }
        }

        /// <summary>
        ///     Converts the value of the current System.DateTime object to Coordinated Universal Time (UTC).
        /// </summary>
        /// <param name="dateTime">
        ///     The date time to convert
        /// </param>
        /// <returns>
        ///     An object whose System.DateTime.Kind property is System.DateTimeKind.Utc,
        ///     and whose value is the UTC equivalent to the value of the current System.DateTime
        ///     object, or System.DateTime.MaxValue if the converted value is too large to
        ///     be represented by a System.DateTime object, or System.DateTime.MinValue if
        ///     the converted value is too small to be represented by a System.DateTime object.
        /// </returns>
        public static DateTime? ToUniversalTime
            (
                this DateTime? dateTime
            )
        {
            return dateTime.HasValue ? dateTime.Value.ToUniversalTime() : dateTime;
        }

        /// <summary>
        /// Determines if the date has a time component
        /// </summary>
        /// <param name="date">The date to check</param>
        /// <returns>True, if there is a time component; otherwise false</returns>
        public static bool HasTime
            (
                this DateTime? date
            )
        {
            if (date.HasValue)
            {
                return HasTime(date.Value);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the date has a time component
        /// </summary>
        /// <param name="date">The date to check</param>
        /// <returns>True, if there is a time component; otherwise false</returns>
        public static bool HasTime
            (
                this DateTime date
            )
        {
            return (date.TimeOfDay > TimeSpan.Zero);
        }
    }
}
