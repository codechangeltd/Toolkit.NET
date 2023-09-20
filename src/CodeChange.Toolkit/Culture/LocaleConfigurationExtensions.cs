namespace CodeChange.Toolkit.Culture
{
    using System;
    
    public static class LocaleConfigurationExtensions
    {
        /// <summary>
        /// The local date and time
        /// </summary>
        /// <param name="locale">The locale configuration</param>
        /// <returns>The date and time</returns>
        public static DateTime GetNowLocal(this ILocaleConfiguration locale)
        {
            return DateTime.UtcNow.UtcToLocalTime(locale);
        }

        /// <summary>
        /// The local date without the time component
        /// </summary>
        /// <param name="locale">The locale configuration</param>
        /// <returns>The date</returns>
        public static DateTime GetTodayLocal(this ILocaleConfiguration locale)
        {
            return GetNowLocal(locale).Date;
        }
    }
}
