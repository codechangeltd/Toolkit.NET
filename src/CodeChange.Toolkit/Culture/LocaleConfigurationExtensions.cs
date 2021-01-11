namespace CodeChange.Toolkit.Culture
{
    using System;
    
    /// <summary>
    /// Represents various extension methods for locale configurations
    /// </summary>
    public static class LocaleConfigurationExtensions
    {
        /// <summary>
        /// Gets the local date and time
        /// </summary>
        /// <param name="locale">The locale configuration</param>
        /// <returns>The date and time</returns>
        public static DateTime GetNowLocal(this ILocaleConfiguration locale)
        {
            return DateTime.UtcNow.UtcToLocalTime(locale);
        }

        /// <summary>
        /// Gets the local date without the time component
        /// </summary>
        /// <param name="locale">The locale configuration</param>
        /// <returns>The date</returns>
        public static DateTime GetTodayLocal(this ILocaleConfiguration locale)
        {
            return GetNowLocal(locale).Date;
        }
    }
}
