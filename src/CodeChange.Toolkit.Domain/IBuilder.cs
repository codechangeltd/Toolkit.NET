namespace CodeChange.Toolkit.Domain
{
    using System;
    using System.Collections.Generic;
    
    /// <summary>
    /// Defines a contract for a builder pattern that is responsible for creating a complex immutable object
    /// </summary>
    /// <typeparam name="T">The type being constructed by the builder</typeparam>
    public interface IBuilder<T>
    {
        /// <summary>
        /// Executes the build process and returns the object instance created
        /// </summary>
        /// <returns>The object instance created</returns>
        T Build();
    }
}
