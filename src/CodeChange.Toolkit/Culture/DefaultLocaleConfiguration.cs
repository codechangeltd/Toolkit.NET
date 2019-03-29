namespace CodeChange.Toolkit.Culture
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents the default locale configuration implementation
    /// </summary>
    public class DefaultLocaleConfiguration : ILocaleConfiguration
    {
        /// <summary>
        /// Constructs the configuration with the default values
        /// </summary>
        public DefaultLocaleConfiguration()
        {
            this.DefaultTimeZone = TimeZoneInfo.Local;
            this.DefaultCulture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Gets the time zone offset value (in minutes)
        /// </summary>
        public int? TimeZoneOffset { get; private set; }

        /// <summary>
        /// Sets the time zone offset value
        /// </summary>
        /// <param name="offset">The new offset value</param>
        /// <remarks>
        /// The timezone offset must be between -12 and 14 hours.
        /// 
        /// However, the offset is measured in minutes, so the 
        /// range is actually -720 to 840.
        /// </remarks>
        public void SetTimeZoneOffset
            (
                int offset
            )
        {
            if (offset < -720 || offset > 840)
            {
                throw new ArgumentException
                (
                    "The timezone offset must be between -12 and 14 hours."
                );
            }

            this.TimeZoneOffset = offset;
        }

        /// <summary>
        /// Gets default time zone to use
        /// </summary>
        public TimeZoneInfo DefaultTimeZone { get; private set; }

        /// <summary>
        /// Gets the default culture to use
        /// </summary>
        public CultureInfo DefaultCulture { get; private set; }
    }
}
