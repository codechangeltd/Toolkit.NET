namespace CodeChange.Toolkit.Domain.Aggregate
{
    using System;

    /// <summary>
    /// Represents a standard implementation of the globally unique key generator
    /// </summary>
    public class EntityKeyGenerator : IKeyGenerator
    {
        /// <summary>
        /// Generates a new key value using a date time stamp and a new GUID
        /// </summary>
        /// <returns>The key generated</returns>
        public string GenerateKey()
        {
            long i = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }

            return String.Format("{0:x}", i - DateTime.UtcNow.Ticks);
        }
    }
}
