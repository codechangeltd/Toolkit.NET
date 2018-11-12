namespace CodeChange.Toolkit.Domain.Aggregate
{
    using System;

    /// <summary>
    /// Represents a numerical implementation of the globally unique key generator
    /// </summary>
    public class NumericalKeyGenerator : IKeyGenerator
    {
        /// <summary>
        /// Generates a new key value using a date time stamp and a random digit
        /// </summary>
        /// <returns>The key generated</returns>
        public string GenerateKey()
        {
            var dateStamp = DateTime.UtcNow.ToString("yyMMddHHmmssff");
            var randomDigit = new Random().Next(9);

            return dateStamp + randomDigit;
        }
    }
}
