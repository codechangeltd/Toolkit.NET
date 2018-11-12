namespace CodeChange.Toolkit.Domain
{
    using System;

    /// <summary>
    /// Provides various type extension methods for domain
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines if a type is a simple type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True, if the type is simple; otherwise false</returns>
        /// <remarks>
        /// By simple type, we mean a value type, enum or string.
        /// </remarks>
        public static bool IsSimple
            (
                this Type type
            )
        {
            Validate.IsNotNull(type);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Nullable type, check if the nested type is simple.
                return IsSimple
                (
                    type.GetGenericArguments()[0]
                );
            }
            else
            {
                return
                (
                    type.IsPrimitive
                        || type.IsEnum
                        || type.Equals(typeof(string))
                        || type.Equals(typeof(decimal))
                );
            }
        }

        /// <summary>
        /// Determines if the type is a date time type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True, if it is a date time; otherwise false</returns>
        public static bool IsDateTime
            (
                this Type type
            )
        {
            Validate.IsNotNull(type);

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
