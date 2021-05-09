namespace CodeChange.Toolkit.Domain
{
    using System;

    /// <summary>
    /// Represents a base class for a builder that is responsible for creating a complex immutable object
    /// </summary>
    /// <typeparam name="T">The type being constructed by the builder</typeparam>
    public abstract class BuilderBase<T> : IBuilder<T>
    {
        /// <summary>
        /// Holds a flag value used to determine if the object has been built
        /// </summary>
        private bool _built;

        /// <summary>
        /// Creates a new object instance from the builder specified using an implicit operation
        /// </summary>
        /// <param name="builder">The builder object to use</param>
        /// <returns>The build result of the builder object specified by the implicit operation</returns>
        public static implicit operator T(BuilderBase<T> builder)
        {
            return builder.Build();
        }

        /// <summary>
        /// Executes the build process and returns the object instance created
        /// </summary>
        /// <returns>The object instance created</returns>
        public T Build()
        {
            if (_built)
            {
                var message = $"An instance of {typeof(T)} has already been built.";

                throw new InvalidOperationException(message);
            }

            _built = true;

            return GetInstance();
        }

        /// <summary>
        /// When overridden in a derived class, creates an instance of the object with the build parameters
        /// </summary>
        /// <returns>The object instance created</returns>
        protected abstract T GetInstance();
    }
}
