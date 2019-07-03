namespace CodeChange.Toolkit.Reflection
{
    using System;
    
    /// <summary>
    /// Defines a contract for a service that resolves instances of types
    /// </summary>
    public interface ITypeActivator
    {
        /// <summary>
        /// Gets an instance of the type specified
        /// </summary>
        /// <param name="type">The type to resolve</param>
        /// <returns>An instance of the type specified</returns>
        object GetInstance(Type type);

        /// <summary>
        /// Gets an instance of the type specified
        /// </summary>
        /// <typeparam name="T">The type to resolve</typeparam>
        /// <returns>An instance of the type specified</returns>
        T GetInstance<T>();
    }
}
