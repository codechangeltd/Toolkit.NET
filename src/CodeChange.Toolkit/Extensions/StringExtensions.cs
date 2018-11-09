namespace System
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Provides various extension methods for the String class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Defines a RegEx pattern for matching apostrophes
        /// </summary>
        private const string ApostrophePattern = @"'|’";

        /// <summary>
        /// Defines a RegEx pattern for matching bad URL characters
        /// </summary>
        private const string UrlBadCharPattern = @"[^a-z0-9]";

        /// <summary>
        /// Defines a RegEx pattern for matching multiple spaces
        /// </summary>
        private const string MultiSpacePattern = @" +";

        /// <summary>
        /// Defines a RegEx pattern for matching HTML tags
        /// </summary>
        private const string HtmlPattern = @"<.*?>";

        /// <summary>
        /// Defines a RegEx pattern for matching email addresses
        /// </summary>
        private const string EmailAddressPattern = 
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))" +
            @"|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|" +
            @"(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        /// <summary>
        /// Defines a compiled RegEx object for matching apostrophes
        /// </summary>
        private static readonly Regex _apostropheRegex = new Regex
        (
            ApostrophePattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        /// <summary>
        /// Defines a compiled RegEx object for matching bad URL characters
        /// </summary>
        private static readonly Regex _urlBadCharRegex = new Regex
        (
            UrlBadCharPattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        /// <summary>
        /// Defines a compiled RegEx object for matching multiple spaces
        /// </summary>
        private static readonly Regex _multiSpaceRegex = new Regex
        (
            MultiSpacePattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        /// <summary>
        /// Defines a compiled RegEx object for matching HTML tags
        /// </summary>
        private static readonly Regex _htmlRegex = new Regex
        (
            HtmlPattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        /// <summary>
        /// Defines a compiled RegEx object for matching email addresses
        /// </summary>
        private static readonly Regex _emailRegex = new Regex
        (
            EmailAddressPattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        /// <summary>
        /// Truncates a given string to the number of characters specified by the length value
        /// </summary>
        /// <param name="input">The string to truncate</param>
        /// <param name="length">The length to reduce to</param>
        /// <returns>The truncated string</returns>
        public static string Truncate
            (
                this string input,
                int length
            )
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }
            else
            {
                if (input.Length > length)
                {
                    var truncated = input.Substring
                    (
                        0,
                        (length + 3)
                    );

                    return truncated + "...";
                }
                else
                {
                    return input;
                }
            }
        }

        /// <summary>
        /// Shortcut syntax sugar for String.Format() that just requires the args values
        /// </summary>
        /// <param name="value">The string to format</param>
        /// <param name="args">The args values to merge into the string</param>
        /// <returns>The formatted string</returns>
        public static string With
            (
                this string value,
                params object[] args
            )
        {
            Validate.IsNotEmpty(value);

            return String.Format(value, args);
        }
        
        /// <summary>
        /// Use the current thread's culture info for conversion
        /// </summary>
        public static string ToTitleCase
            (
                this string value
            )
        {
            Validate.IsNotEmpty(value);

            var cultureInfo = Thread.CurrentThread.CurrentCulture;

            return cultureInfo.TextInfo.ToTitleCase
            (
                value.ToLower()
            );
        }

        /// <summary>
        /// Overload which uses the culture info with the specified name
        /// </summary>
        public static string ToTitleCase
            (
                this string value,
                string cultureInfoName
            )
        {
            Validate.IsNotEmpty(value);

            var cultureInfo = new CultureInfo(cultureInfoName);

            return cultureInfo.TextInfo.ToTitleCase
            (
                value.ToLower()
            );
        }

        /// <summary>
        /// Overload which uses the specified culture info
        /// </summary>
        public static string ToTitleCase
            (
                this string value,
                CultureInfo cultureInfo
            )
        {
            Validate.IsNotEmpty(value);

            return cultureInfo.TextInfo.ToTitleCase
            (
                value.ToLower()
            );
        }

        /// <summary>
        /// Determines if the string has a value
        /// </summary>
        public static bool HasValue
            (
                this string value
            )
        {
            Validate.IsNotEmpty(value);

            return 
            (
                false == String.IsNullOrEmpty(value) && value.Trim().Length > 0
            );
        }

        /// <summary>
        /// Determines if one string has the same value as another
        /// </summary>
        public static bool IsEqualTo
            (
                this string value,
                string other
            )
        {
            Validate.IsNotEmpty(value);

            return value.Equals
            (
                other,
                StringComparison.OrdinalIgnoreCase
            );
        }

        /// <summary>
        /// Determines if the string value can be parsed into a numeric value
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns></returns>
        public static bool IsNumeric
            (
                this string value
            )
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            else
            {
                return Double.TryParse
                (
                    value,
                    out double number
                );
            }
        }

        /// <summary>
        /// Determines if a string contains only alpha numeric characters
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True, if the string contains only alpha numeric; otherwise false</returns>
        public static bool IsAlphaNumeric
            (
                this string value
            )
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            else
            {
                return value.All
                (
                    Char.IsLetterOrDigit
                );
            }
        }

        /// <summary>
        /// Determines if the given string contains non-printable (control) characters
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True, if the string contains non-printable characters; otherwise false</returns>
        public static bool ContainsNonPrintableCharacters
            (
                this string value
            )
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                return value.Any(c => Char.IsControl(c));
            }
        }

        /// <summary>
        /// Determines if the string specified contains any non-ASCII characters
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True, if the string is ASCII only; otherwise false</returns>
        /// <remarks>
        /// ASCII encoding replaces non-ASCII with question marks, so we use UTF8 to see 
        /// if multi-byte sequences are there.
        /// </remarks>
        public static bool ContainsNonAscii
            (
                this string value
            )
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                return Encoding.UTF8.GetByteCount(value) != value.Length;
            }
        }

        /// <summary>
        /// Determines if a string contains a substring using a specific culture
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="toCheck">The substring to check for</param>
        /// <param name="comparison">The comparison method</param>
        /// <returns>True, if a match was found</returns>
        public static bool Contains
            (
                this string source,
                string toCheck,
                StringComparison comparison
            )
        {
            var index = source.IndexOf
            (
                toCheck,
                comparison
            );

            return index >= 0;
        }

        /// <summary>
        /// Returns the integer equivalent of the string value
        /// </summary>
        public static int? ToInt
            (
                this string value,
                int? defaultValue = null
            )
        {
            Validate.IsNotEmpty(value);

            return value.IsNumeric() ? Int32.Parse(value) : defaultValue;
        }

        /// <summary>
        /// Removes real and database safe HTML characters from a given string
        /// </summary>
        /// <param name="input">The text to clean</param>
        /// <returns>The text without any HTML tags</returns>
        public static string StripHtml
            (
                this string input
            )
        {
            Validate.IsNotEmpty(input);

            if (false == String.IsNullOrEmpty(input))
            {
                input = _htmlRegex.Replace
                (
                    input,
                    " "
                );
            }

            return input;
        }

        /// <summary>
        /// Checks if a given string contains HTML characters, returns true if it does
        /// </summary>
        /// <param name="input">The text to check</param>
        /// <returns>True, if the input contains HTML</returns>
        public static bool ContainsHtml
            (
                this string input
            )
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            else
            {
                return _htmlRegex.IsMatch(input);
            }
        }

        /// <summary>
        /// Determines if the URL value specified is an absolute URL (and not a relative one)
        /// </summary>
        /// <param name="url">The URL value to check</param>
        /// <returns>True, if the URL is absolute; otherwise false</returns>
        public static bool IsAbsoluteUrl
            (
                this string url
            )
        {
            Validate.IsNotEmpty(url);
            
            return Uri.TryCreate
            (
                url,
                UriKind.Absolute,
                out var result
            );
        }

        /// <summary>
        /// Encodes a URL into a nice, safe, SEO friendly URL format
        /// </summary>
        /// <param name="urlToEncode">The URL value to encode</param>
        /// <returns>The encoded URL value</returns>
        public static string ToFriendlyUrl
            (
                this string urlToEncode
            )
        {
            Validate.IsNotEmpty(urlToEncode);

            // Remove side spaces and convert to lower
            urlToEncode = (urlToEncode + "").ToLower();

            // Remove apostrophes from words
            urlToEncode = _apostropheRegex.Replace
            (
                urlToEncode,
                String.Empty
            );

            // Replace invalid characters with a space
            urlToEncode = _urlBadCharRegex.Replace
            (
                urlToEncode,
                " "
            );

            // Convert multiple spaces into one dash
            urlToEncode = _multiSpaceRegex.Replace
            (
                urlToEncode,
                "-"
            );

            return urlToEncode.Trim();
        }

        /// <summary>
        /// Spacifies a string value by adding a separator (usually space) between words
        /// </summary>
        /// <param name="value">The string value to spacify</param>
        /// <param name="separator">The separator value to use, default is a space</param>
        /// <returns>The spacified string value</returns>
        public static string Spacify
            (
                this string value,
                string separator = " "
            )
        {
            Validate.IsNotEmpty(value);

            if (String.IsNullOrEmpty(value) || value.Contains(separator))
            {
                return value;
            }
            else
            {
                var result = String.Empty;
                var previousChar = '\0';

                foreach (var currentChar in value)
                {
                    if (previousChar != Char.MinValue && Char.IsLetter(currentChar))
                    {
                        if (Char.IsNumber(previousChar) & !Char.IsNumber(currentChar) 
                            || Char.IsUpper(currentChar) & Char.IsLower(previousChar))
                        {
                            result += separator + Convert.ToString(currentChar);
                        }
                        else
                        {
                            result += Convert.ToString(currentChar);
                        }
                    }
                    else
                    {
                        result += Convert.ToString(currentChar);
                    }

                    previousChar = currentChar;
                }

                if (false == String.IsNullOrEmpty(result))
                {
                    result = result.Replace
                    (
                        "_",
                        separator
                    );
                        
                    result = result.Replace
                    (
                        "  ",
                        " "
                    );
                }

                return result;
            }
        }

        /// <summary>
        /// Splits the string into an array of individual words
        /// </summary>
        /// <param name="value">The value to split into words</param>
        /// <returns>An array of extracted words</returns>
        public static string[] SplitIntoWords
            (
                this string value
            )
        {
            Validate.IsNotEmpty(value);

            // Trim and remove double spaces to avoid empty words
            value = value.Trim().Replace("  ", " ");

            var matchingWords = new List<string>();
            var wordBuilder = new StringBuilder();
            var currentIndex = 0;
            var previousChar = '\0';

            foreach (char currentChar in value)
            {
                if (Char.IsLetterOrDigit(currentChar) || Char.IsSymbol(currentChar))
                {
                    wordBuilder.Append(currentChar);
                }
                else if (Char.IsWhiteSpace(currentChar) || Char.IsPunctuation(currentChar))
                {
                    if (wordBuilder.Length > 0)
                    {
                        var flushWord = true;

                        // Check ahead of the current position to see if the next character is a digit
                        // If the current digit is a number, the current punctuation is a full stop 
                        // and the next digit is also a number, then treat it as part of the word
                        // (e.g. "1.50" would be treated as a whole word instead of "1" and "50")
                        if (Char.IsNumber(previousChar) && currentChar == '.' && (currentIndex + 1) < value.Length)
                        {
                            var nextChar = value[currentIndex + 1];

                            if (Char.IsNumber(nextChar))
                            {
                                wordBuilder.Append(currentChar);
                                flushWord = false;
                            }
                        }

                        // Flush the word into the matching words collection
                        if (flushWord)
                        {
                            matchingWords.Add(wordBuilder.ToString());
                            wordBuilder.Clear();
                        }
                    }
                }

                currentIndex++;
                previousChar = currentChar;
            }

            // Add anything not yet flushed to the words list
            if (wordBuilder.Length > 0)
            {
                matchingWords.Add(wordBuilder.ToString());
            }

            return matchingWords.ToArray();
        }

        /// <summary>
        /// Removes all special characters from the string value
        /// </summary>
        /// <param name="value">The value to remove special characters from</param>
        /// <returns>The string without special characters</returns>
        public static string RemoveSpecialCharacters
            (
                this string value
            )
        {
            Validate.IsNotEmpty(value);

            var sb = new StringBuilder();

            foreach (char c in value)
            {
                if (Char.IsLetterOrDigit(c) || Char.IsSymbol(c) || Char.IsWhiteSpace(c) || Char.IsPunctuation(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the first string with a non-empty non-null value
        /// </summary>
        /// <param name="input">The input value</param>
        /// <param name="alternative">The alternative value</param>
        /// <returns>The first string with a non-empty non-null value</returns>
        public static string Or
            (
                this string input,
                string alternative
            )
        {
            Validate.IsNotEmpty(input);

            if (String.IsNullOrEmpty(input))
            {
                return alternative;
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Determines if the pattern is like the text using a wildcard match
        /// </summary>
        /// <param name="pattern">The pattern containing the wildcards</param>
        /// <param name="text">The text to match against</param>
        /// <param name="caseSensitive">If true, a case sensitive match is performed</param>
        /// <returns>True, if the pattern matches the text</returns>
        public static bool IsLike
            (
                this string pattern,
                string text,
                bool caseSensitive = false
            )
        {
            Validate.IsNotEmpty(pattern);

            pattern = pattern.Replace(".", @"\.");
            pattern = pattern.Replace("?", ".");
            pattern = pattern.Replace("*", ".*?");
            pattern = pattern.Replace(@"\", @"\\");
            pattern = pattern.Replace(" ", @"\s");

            var regex = new Regex
            (
                pattern,
                caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase
            );

            return regex.IsMatch(text);
        }

        /// <summary>
        /// Creates an MD5 fingerprint of the string value specified
        /// </summary>
        public static string ToMd5Fingerprint
            (
                this string value
            )
        {
            Validate.IsNotEmpty(value);

            var bytes = Encoding.Unicode.GetBytes
            (
                value.ToCharArray()
            );

            var hash = new MD5CryptoServiceProvider().ComputeHash
            (
                bytes
            );

            // Concatenate the hash bytes into one long string
            var builder = hash.Aggregate
            (
                new StringBuilder(32),
                (sb, b) => sb.Append(b.ToString("X2"))
            );

            return builder.ToString();
        }

        /// <summary>
        /// Gets the line number from the position specified in a string
        /// </summary>
        /// <param name="value">The string value to get the line number from</param>
        /// <param name="position">The position to get the line number for</param>
        /// <returns>The line number found</returns>
        public static int LineFromPosition
            (
                this string value,
                int position
            )
        {
            Validate.IsNotEmpty(value);

            var lineNumber = 1;

            if (position > value.Length)
            {
                position = value.Length;
            }

            for (int i = 0; i <= position - 1; i++)
            {
                if (value[i] == '\n')
                {
                    lineNumber++;
                }
            }

            return lineNumber;
        }

        /// <summary>
        /// Splits the string on commas and removes any leading/trailing whitespace from each result item.
        /// </summary>
        /// <param name="value">The value to split</param>
        /// <param name="separator">The separator character</param>
        /// <returns>An array of strings parsed from the input string.</returns>
        public static string[] SplitAndTrim
            (
                this string value,
                char separator
            )
        {
            Validate.IsNotEmpty(value);

            if (String.IsNullOrEmpty(value))
            {
                return new string[0];
            }

            var split = from piece in value.Split(separator)
                        let trimmed = piece.Trim()
                        where false == String.IsNullOrEmpty(trimmed)
                        select trimmed;

            return split.ToArray();
        }

        /// <summary>
        /// Finds all instances of a substring within a string and returns the indexes of each occurrence
        /// </summary>
        /// <param name="haystack">The string to search</param>
        /// <param name="needle">The string to search for</param>
        /// <returns>An enumeration of all indexes indicating where the substring was found</returns>
        public static IEnumerable<int> IndexesOf
            (
                this string haystack,
                string needle
            )
        {
            Validate.IsNotEmpty(haystack);

            int lastIndex = 0;

            while (true)
            {
                int index = haystack.IndexOf
                (
                    needle,
                    lastIndex
                );
                
                if (index == -1)
                {
                    yield break;
                }

                yield return index;
                
                lastIndex = index + needle.Length;
            }
        }

        /// <summary>
        /// Counts the number of occurrences of a substring within the string specified
        /// </summary>
        /// <param name="input">The input string to search</param>
        /// <param name="search">The search string</param>
        /// <returns>The number of occurrences found</returns>
        public static int Count
            (
                this string input,
                string search
            )
        {
            Validate.IsNotEmpty(input);

            var indexes = input.IndexesOf(search);

            return indexes == null ? 0 : indexes.Count();
        }

        /// <summary>
        /// Extracts the string found to the left of the string value specified
        /// </summary>
        /// <param name="input">The string to search</param>
        /// <param name="value">The matching value</param>
        /// <returns>The string found to the left of the value specified</returns>
        public static string LeftOf
            (
                this string input,
                string value
            )
        {
            Validate.IsNotEmpty(input);

            var firstIndex = input.IndexOf(value);

            if (firstIndex < 1)
            {
                return String.Empty;
            }
            else
            {
                return input.Substring
                (
                    0,
                    firstIndex
                );
            }
        }

        /// <summary>
        /// Extracts the string found to the right of the string value specified
        /// </summary>
        /// <param name="input">The string to search</param>
        /// <param name="value">The matching value</param>
        /// <returns>The string found to the right of the value specified</returns>
        public static string RightOf
            (
                this string input,
                string value
            )
        {
            Validate.IsNotEmpty(input);

            var lastIndex = input.LastIndexOf(value);

            if ((lastIndex + value.Length) == input.Length)
            {
                return String.Empty;
            }
            else
            {
                return input.Substring
                (
                    lastIndex + 1
                );
            }
        }

        /// <summary>
        /// Transforms a string to it's URL slug friendly version
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The slug that was generated</returns>
        public static string ToUrlSlug
            (
                this string value
            )
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            value = value.ToLowerInvariant();

            var encoding = Encoding.GetEncoding("Cyrillic");
            var bytes = encoding.GetBytes(value);

            value = Encoding.ASCII.GetString(bytes);

            // Replace spaces
            value = Regex.Replace
            (
                value,
                @"\s",
                "-",
                RegexOptions.Compiled
            );

            // Remove invalid chars
            value = Regex.Replace
            (
                value,
                @"[^a-z0-9\s-_]",
                String.Empty,
                RegexOptions.Compiled
            );

            // Trim dashes from end
            value = value.Trim('-', '_');

            // Replace double occurrences of - or _
            value = Regex.Replace
            (
                value,
                @"([-_]){2,}",
                "$1",
                RegexOptions.Compiled
            );

            return value;
        }

        /// <summary>
        /// Checks if the string value specified is a valid URL slug
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True, if the slug is valid; otherwise false</returns>
        public static bool IsValidUrlSlug
            (
                this string value
            )
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                var generatedSlug = ToUrlSlug(value);

                return value.Equals
                (
                    generatedSlug
                );
            }
        }

        /// <summary>
        /// Determines if the string is a valid email address
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True, if the string contains a valid email address; otherwise false</returns>
        public static bool IsValidEmailAddress
            (
                this string value
            )
        {
            if (String.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                return _emailRegex.IsMatch(value);
            }
        }

        /// <summary>
        /// Removes a query string parameter from a URL based on the key specified
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="key">The query string parameter key</param>
        /// <returns>The URL without the query string parameter</returns>
        public static string RemoveQueryStringByKey
            (
                this string url,
                string key
            )
        {
            var uri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString
            (
                uri.Query
            );

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            var pagePathWithoutQueryString = uri.GetLeftPart
            (
                UriPartial.Path
            );

            if ( newQueryString.Count > 0)
            {
                return String.Format
                (
                    "{0}?{1}",
                    pagePathWithoutQueryString,
                    newQueryString
                );
            }
            else
            {
                return pagePathWithoutQueryString;
            }
        }
    }
}
