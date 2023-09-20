namespace CodeChange.Toolkit.Culture
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Defines the configuration for locale settings
    /// </summary>
    public interface ILocaleConfiguration
    {
        /// <summary>
        /// The time zone offset value (in minutes)
        /// </summary>
        int? TimeZoneOffset { get; }

        /// <summary>
        /// Sets the time zone offset value
        /// </summary>
        /// <param name="offset">The new offset value</param>
        void SetTimeZoneOffset(int offset);

        /// <summary>
        /// The default time zone to use
        /// </summary>
        TimeZoneInfo DefaultTimeZone { get; }

        /// <summary>
        /// The default culture to use
        /// </summary>
        CultureInfo DefaultCulture { get; }
    }
}
