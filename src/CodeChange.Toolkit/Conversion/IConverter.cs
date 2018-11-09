namespace System
{
    /// <summary>
    /// Defines a contract for a service that converts one object type to another
    /// </summary>
    /// <typeparam name="TFrom">The type to convert from</typeparam>
    /// <typeparam name="TTo">The type to convert to</typeparam>
    public interface IConverter<TFrom, TTo>
    {
        /// <summary>
        /// Converts the value of type TFrom to the type TTo. 
        /// Raises an InvalidConversionException if the value cannot be converted.
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns>The value that was converted to type TTo</returns>
        TTo Convert(TFrom value);
    }
}
