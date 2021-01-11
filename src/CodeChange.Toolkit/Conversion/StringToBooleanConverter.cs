﻿namespace System
{
    /// <summary>
    /// Converter implementation for converting a string to boolean value
    /// </summary>
    public class StringToBooleanConverter : IConverter<string, bool>
    {
        /// <summary>
        /// Defines a default set of accepted true values
        /// </summary>
        public readonly string[] DefaultAcceptedTrueValues = { "true", "yes", "1" };

        /// <summary>
        /// Creates a new string to boolean converter with the default conversion options
        /// </summary>
        public StringToBooleanConverter()
        {
            this.AcceptedTrueValues = this.DefaultAcceptedTrueValues;
        }

        /// <summary>
        /// Creates a new string to boolean converter with the conversion options specified
        /// </summary>
        /// <param name="acceptedTrueValues">An array of accepted true values</param>
        public StringToBooleanConverter(string[] acceptedTrueValues)
        {
            this.AcceptedTrueValues = acceptedTrueValues ?? this.DefaultAcceptedTrueValues;
        }

        /// <summary>
        /// Gets the accepted true values that have been supplied
        /// </summary>
        public string[] AcceptedTrueValues { get; private set; }

        /// <summary>
        /// Converts the string value specified to a boolean representation
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>True, if an accepted value was matched; otherwise false</returns>
        public bool Convert(string value)
        {
            bool result = false;

            if (false == String.IsNullOrEmpty(value))
            {
                foreach (var comparer in this.AcceptedTrueValues)
                {
                    var comparerFound = value.Trim().StartsWith(comparer, StringComparison.OrdinalIgnoreCase);

                    if (comparerFound)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
