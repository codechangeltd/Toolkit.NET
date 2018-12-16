namespace System
{
    /// <summary>
    /// Used for checking if an object of type TFrom can be converted to the type TTo
    /// </summary>
    /// <typeparam name="TFrom">The convert from type</typeparam>
    /// <typeparam name="TTo">The convert to type</typeparam>
    internal class TypeConverterChecker<TFrom, TTo>
    {
        public bool CanConvert { get; private set; }

        public TypeConverterChecker(TFrom from)
        {
            this.CanConvert = false;

            // Check for string to numeric type conversions first
            if (typeof(TFrom) == typeof(string) && typeof(TTo).IsNumericType())
            {
                if (from.IsNumeric())
                {
                    try
                    {
                        Convert.ChangeType(from, typeof(TTo));
                        this.CanConvert = true;
                    }
                    catch { }
                }
            }
            else
            {
                try
                {
                    var to = (TTo)(dynamic)from;
                    this.CanConvert = true;
                }
                catch { }
            }
        }
    }
}
