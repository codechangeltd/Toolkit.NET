namespace System
{
    /// <summary>
    /// Represents a utility for converting objects
    /// </summary>
    public static class ObjectConverter
    {
        /// <summary>
        /// Converts an object to the type specified
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="toType">The type to convert to</param>
        /// <returns>The converted value</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.Reflection.TargetInvocationException"></exception>
        /// <exception cref="System.MethodAccessException"></exception>
        /// <exception cref="System.MemberAccessException"></exception>
        /// <exception cref="System.Runtime.InteropServices.InvalidComObjectException"></exception>
        /// <exception cref="System.MissingMethodException"></exception>
        /// <exception cref="System.Runtime.InteropServices.COMException"></exception>
        /// <exception cref="System.TypeLoadException"></exception>
        /// <exception cref="System.Reflection.AmbiguousMatchException"></exception>
        /// <exception cref="System.Reflection.TargetException"></exception>
        /// <exception cref="System.Reflection.TargetParameterCountException"></exception>
        public static object Convert(object value, Type toType)
        {
            var converterType = typeof(GenericObjectToTypeConverter<>);
            var converterArgs = new Type[] { toType };

            var genericType = converterType.MakeGenericType(converterArgs);
            var converterInstance = Activator.CreateInstance(genericType);
            var convertMethod = genericType.GetMethod("Convert");
            var convertedValue = convertMethod.Invoke(converterInstance, new object[] { value });

            return convertedValue;
        }
    }
}
