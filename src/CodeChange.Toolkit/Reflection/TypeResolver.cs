namespace System
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a class responsible for resolving types
    /// </summary>
    public class TypeResolver
    {
        /// <summary>
        /// Finds all types matching the specified class name in all loaded assemblies
        /// </summary>
        /// <param name="className">The class name</param>
        /// <returns>A collection of matching class types</returns>
        public static Type[] FindTypesByName(string className)
        {
            Validate.IsNotEmpty(className);
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return FindTypesByName(className, assemblies);
        }

        /// <summary>
        /// Finds all types matching the specified class name in the assemblies specified
        /// </summary>
        /// <param name="className">The class name</param>
        /// <param name="namespaceSearch">The assembly namespace</param>
        /// <returns>A collection of matching class types</returns>
        public static Type[] FindTypesByName(string className, string namespaceSearch)
        {
            Validate.IsNotEmpty(className);
            Validate.IsNotEmpty(namespaceSearch);

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var filteredAssemblies = allAssemblies.Where
            (
                assembly => assembly.GetLoadableTypes().Any
                (
                    type => type.Namespace != null && type.Namespace.StartsWith(namespaceSearch)
                )
            );

            return FindTypesByName(className, filteredAssemblies.ToArray());
        }

        /// <summary>
        /// Finds all types matching the specified class name in the assemblies specified
        /// </summary>
        /// <param name="className">The class name</param>
        /// <param name="assemblies">The assemblies to scan</param>
        /// <returns>A collection of matching class types</returns>
        public static Type[] FindTypesByName(string className, params Assembly[] assemblies)
        {
            Validate.IsNotEmpty(className);
            Validate.IsNotNull(assemblies);

            var matchingTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetLoadableTypes();

                for (int i = 0; i < assemblyTypes.Length; i++)
                {
                    if (assemblyTypes[i].Name == className)
                    {
                        matchingTypes.Add(assemblyTypes[i]);
                    }
                }
            }

            return matchingTypes.ToArray();
        }
    }
}
