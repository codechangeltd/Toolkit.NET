namespace CodeChange.Toolkit.Autofac
{
    using CodeChange.Toolkit.Reflection;
    using global::Autofac;
    using System;

    /// <summary>
    /// Represents an Autofac implementation of the type activator
    /// </summary>
    public class TypeActivator : ITypeActivator
    {
        private readonly ILifetimeScope _scope;

        /// <summary>
        /// Constructs the activator with an Autofac lifetime scope
        /// </summary>
        /// <param name="scope">The lifetime scope</param>
        public TypeActivator
            (
                ILifetimeScope scope
            )
        {
            Validate.IsNotNull(scope);

            _scope = scope;
        }

        /// <summary>
        /// Gets an instance of the type specified
        /// </summary>
        /// <param name="type">The type to resolve</param>
        /// <returns>An instance of the type specified</returns>
        public object GetInstance(Type type)
        {
            return _scope.Resolve(type);
        }

        /// <summary>
        /// Gets an instance of the type specified
        /// </summary>
        /// <typeparam name="T">The type to resolve</typeparam>
        /// <returns>An instance of the type specified</returns>
        public T GetInstance<T>()
        {
            return _scope.Resolve<T>();
        }
    }
}
