namespace System
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Rounds the double value up to the number of decimal places specified
        /// </summary>
        /// <param name="input">The value to round up</param>
        /// <param name="places">The number of decimal places</param>
        /// <returns>The rounded up value</returns>
        public static double RoundUp(this double input, int places)
        {
            var multiplier = Math.Pow(10, Convert.ToDouble(places));

            return Math.Ceiling(input * multiplier) / multiplier;
        }
    }
}
