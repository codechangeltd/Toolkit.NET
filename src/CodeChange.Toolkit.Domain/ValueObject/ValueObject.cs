namespace CodeChange.Toolkit.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents a base class for all value object types
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public abstract class ValueObject<T> : IEquatable<T> where T : ValueObject<T>
    {
        /// <summary>
        /// Determines if the object equals the object specified
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>True, if the object specified equals the current object; otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            T other = obj as T;
            
            return Equals(other);
        }

        /// <summary>
        /// Gets a has code for the current objects state
        /// </summary>
        /// <returns>The hash-code generated</returns>
        public override int GetHashCode()
        {
            IEnumerable<FieldInfo> fields = GetFields();

            int startValue = 17;
            int multiplier = 59;
            int hashCode = startValue;

            foreach (var field in fields)
            {
                object value = field.GetValue(this);

                if (value != null)
                {
                    hashCode = hashCode * multiplier + value.GetHashCode();
                }
            }

            return hashCode;
        }

        /// <summary>
        /// Determines if the current object equals the object of the type specified
        /// </summary>
        /// <param name="other">The other object to check in the equality operation</param>
        /// <returns>True, if the object equals the other object specified; otherwise false</returns>
        public virtual bool Equals(T other)
        {
            if (other == null)
            {
                return false;
            }

            var t = GetType();
            var otherType = other.GetType();

            if (t != otherType)
            {
                return false;
            }

            var fields = t.GetFields
            (
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            foreach (var field in fields)
            {
                object value1 = field.GetValue(other);
                object value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                    {
                        return false;
                    }
                }

                else if (false == value1.Equals(value2))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a collection of fields in the current object
        /// </summary>
        /// <returns>A collection of field information items</returns>
        private IEnumerable<FieldInfo> GetFields()
        {
            var t = GetType();
            var fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                fields.AddRange
                (
                    t.GetFields
                    (
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                    )
                );

                t = t.BaseType;
            }

            return fields;
        }

        /// <summary>
        /// Determines if the two objects in the equality clause are equal
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns>True, if both objects are equal; otherwise false</returns>
        public static bool operator ==(ValueObject<T> x, ValueObject<T> y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Determines if the two objects in the equality clause are not equal
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns>True, if both objects are not equal; otherwise false</returns>
        public static bool operator !=(ValueObject<T> x, ValueObject<T> y)
        {
            return !(x == y);
        }
    }
}
