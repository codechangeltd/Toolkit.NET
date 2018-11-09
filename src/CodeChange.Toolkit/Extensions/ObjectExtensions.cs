namespace System
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Various extension helper methods for objects
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines if the object is null or an empty string
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True, if the object is null or empty; otherwise false</returns>
        public static bool IsNullOrEmpty
            (
                this object obj
            )
        {
            if (obj == null)
            {
                return true;
            }
            else
            {
                return String.IsNullOrWhiteSpace
                (
                    obj.ToString()
                );
            }
        }

        /// <summary>
        /// Determines if the object has any non empty properties
        /// </summary>
        /// <param name="obj">The object to check</param>
        /// <returns>True, if the object has all empty properties; otherwise false</returns>
        public static bool HasNonEmptyProperties
            (
                this object obj
            )
        {
            var type = obj.GetType();

            var properties = type.GetProperties
            (
                BindingFlags.Public | BindingFlags.Instance
            );

            var hasProperty = properties.Select
            (
                x => x.GetValue(obj, null)
            )
            .Any
            (
                x => x != null 
                    && false == x.GetType().IsEnum
                    && false == x.IsNullOrEmpty()
            );

            return hasProperty;
        }

        /// <summary>
        /// Uses reflection to generate a hash code for the object specified
        /// </summary>
        /// <param name="obj">The object to generate the hash code for</param>
        /// <returns>The hash code that was generated</returns>
        public static int GenerateHashCode
            (
                this object obj
            )
        {
            if (obj == null || false == obj.HasNonEmptyProperties())
            {
                return 0;
            }

            var objectType = obj.GetType();
            var properties = objectType.GetProperties();
            var result = default(int);

            unchecked
            {
                if (objectType == typeof(string))
                {
                    result = obj.GetHashCode();
                }
                else if (objectType.IsEnumerable())
                {
                    foreach (var item in (IEnumerable)obj)
                    {
                        result = (result * 397) ^ item.GenerateHashCode();
                    }
                }
                else
                {
                    foreach (var property in properties)
                    {
                        var propertyValue = property.GetValue(obj);

                        if (propertyValue == null)
                        {
                            result = (result * 397) ^ 0;
                        }
                        else if (propertyValue.GetType().IsEnumerable())
                        {
                            // Currently only supports string collections as anything else is too complex
                            if (propertyValue is IEnumerable<string>)
                            {
                                foreach (var value in (propertyValue as IEnumerable<string>))
                                {
                                    result = (result * 397) ^ value.GetHashCode();
                                }
                            }
                        }
                        else
                        {
                            result = (result * 397) ^ propertyValue.GetHashCode();
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Determines if an object is a numeric type
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True, if the object is numeric; otherwise false</returns>
        public static bool IsNumeric
            (
                this object value
            )
        {
            if (value == null)
            {
                return false;
            }
            else if (value is string)
            {
                return ((string)value).IsNumeric();
            }
            else
            {
                return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
            }
        }
    }
}
