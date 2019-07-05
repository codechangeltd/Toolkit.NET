namespace System
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    
    /// <summary>
    /// Useful extension methods for handling types at runtime
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines if the type can be converted to the type specified from the given type value
        /// </summary>
        /// <param name="fromType">The current type</param>
        /// <param name="toType">The new type</param>
        /// <param name="fromObject">The current type value</param>
        /// <returns>True, if the type can be converted; otherwise false</returns>
        public static bool CanConvert
            (
                this Type fromType,
                Type toType,
                object fromObject
            )
        {
            if (fromObject == null)
            {
                return toType.IsNullableType();
            }
            else
            {
                var fromObjectType = fromObject.GetType();

                if (fromObjectType == toType)
                {
                    return true;
                }
                else if (toType.IsAssignableFrom(fromObjectType) 
                    || fromObjectType.IsAssignableFrom(toType))
                {
                    return true;
                }
                else
                {
                    var converterType = typeof(TypeConverterChecker<,>).MakeGenericType
                    (
                        fromType,
                        toType
                    );

                    var instance = Activator.CreateInstance
                    (
                        converterType,
                        fromObject
                    );

                    var canConvertProperty = converterType.GetProperty
                    (
                        "CanConvert"
                    );

                    return (bool)canConvertProperty.GetGetMethod().Invoke
                    (
                        instance,
                        null
                    );
                }
            }
        }

        /// <summary>
        /// Gets the default value for a type at runtime
        /// </summary>
        /// <param name="t">The type to get the default value for</param>
        /// <returns>The default value</returns>
        public static object GetDefaultValue
            (
                this Type t
            )
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
            {
                return Activator.CreateInstance(t);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines if the type has a property with the name specified
        /// </summary>
        /// <param name="t">The type to check</param>
        /// <param name="propertyName">The name of the property to look for</param>
        /// <returns>True, if the property exists; otherwise false</returns>
        public static bool HasProperty
            (
                this Type t,
                string propertyName
            )
        {
            return t.GetProperty(propertyName) != null;
        }

        /// <summary>
        /// Gets a property value from an object instance for a type
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="obj">The object</param>
        /// <returns>The property value</returns>
        public static object GetPropertyValue
            (
                this Type type,
                string propertyName,
                object obj
            )
        {
            if (false == type.HasProperty(propertyName))
            {
                throw new InvalidOperationException
                (
                    $"{type.Name} does not contain a property named {propertyName}."
                );
            }

            var property = type.GetProperty(propertyName);
            var value = property.GetValue(obj);

            return value;
        }

        /// <summary>
        /// Determines if a type is numeric. Nullable numeric types are considered numeric.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="acceptNullables">If true nullable numeric types are also accepted</param>
        public static bool IsNumericType
            (
                this Type type,
                bool acceptNullables = true
            )
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                case TypeCode.Object:

                    var isNullable = 
                    (
                        type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    );

                    if (acceptNullables && type.IsGenericType && isNullable)
                    {
                        return IsNumericType
                        (
                            Nullable.GetUnderlyingType(type)
                        );
                    }

                    return false;
            }

            return false;
        }

        /// <summary>
        /// Determines if the type specified is a nullable type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True, if the type is nullable; otherwise false</returns>
        public static bool IsNullableType
            (
                this Type type
            )
        {
            Validate.IsNotNull(type);

            return 
            (
                false == type.IsValueType || (Nullable.GetUnderlyingType(type) != null)
            );
        }

        /// <summary>
        /// Determines if the type specified is an enumerable type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True, if the type is enumerable; otherwise false</returns>
        /// <remarks>
        /// All enumerable types are allowed, except for string
        /// </remarks>
        public static bool IsEnumerable
            (
                this Type type
            )
        {
            Validate.IsNotNull(type);

            if (type == typeof(string))
            {
                return false;
            }
            else
            {
                return type.GetInterfaces().Contains
                (
                    typeof(IEnumerable)
                );
            }
        }

        /// <summary>
        /// Determines if the type specified is a dictionary
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True, if the type is a dictionary; otherwise false</returns>
        public static bool IsDictionary
            (
                this Type type
            )
        {
            Validate.IsNotNull(type);

            return
            (
                type.IsGenericType 
                    && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
            );
        }

        /// <summary>
        /// Gets the enumerable type from a collection type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The enumerable type found</returns>
        public static Type GetEnumerableType
            (
                this Type type
            )
        {
            foreach (var intType in type.GetInterfaces())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return intType.GetGenericArguments()[0];
                }
            }

            return null;
        }


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

            if (type.IsGenericType 
                && type.GetGenericTypeDefinition() == typeof(Nullable<>))
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
        /// Determines if the type implements a specific interface
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="interfaceType">The interface type</param>
        /// <returns>True, if the type implements the interface; otherwise false</returns>
        public static bool ImplementsInterface
            (
                this Type type,
                Type interfaceType
            )
        {
            Validate.IsNotNull(type);
            Validate.IsNotNull(interfaceType);

            return type.GetInterfaces().Contains
            (
                interfaceType
            );
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
