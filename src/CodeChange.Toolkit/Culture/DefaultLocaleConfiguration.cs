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
            : this(TimeZoneInfo.Local, CultureInfo.CurrentCulture)
        { }

        /// <summary>
        /// Constructs the configuration with the specific values
        /// </summary>
        /// <param name="timeZone">The time zone</param>
        /// <param name="culture">The culture</param>
        public DefaultLocaleConfiguration(TimeZoneInfo timeZone, CultureInfo culture)
        {
            Validate.IsNotNull(timeZone);
            Validate.IsNotNull(culture);

            DefaultTimeZone = timeZone;
            DefaultCulture = culture;
        }

        /// <summary>
        /// The time zone offset value (in minutes)
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
        public void SetTimeZoneOffset(int offset)
        {
            if (offset < -840 || offset > 720)
            {
                throw new ArgumentException("The timezone offset must be between -14 and 12 hours.");
            }

            TimeZoneOffset = offset;
        }

        /// <summary>
        /// Gets default time zone to use
        /// </summary>
        public TimeZoneInfo DefaultTimeZone { get; private set; }

        /// <summary>
        /// The default culture to use
        /// </summary>
        public CultureInfo DefaultCulture { get; private set; }
    }
}
