namespace CodeChange.Toolkit.Domain
{
    /// <summary>
    /// Defines a contract for a globally unique key generator
    /// </summary>
    public interface IKeyGenerator
    {
        /// <summary>
        /// Generates a new globally unique key that can be used an identifier
        /// </summary>
        /// <returns>The key generated</returns>
        string GenerateKey();
    }
}
