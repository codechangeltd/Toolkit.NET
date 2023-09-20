namespace System.Reflection
{
    using System;
    using System.IO;
    using System.Linq;

    public static class AssemblyExtensions
    {
        /// <summary>
        /// The full path to the assembly in standard Windows form
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The assembly path</returns>
        public static string GetFullPath(this Assembly assembly)
        {
            Validate.IsNotNull(assembly);

            var assemblyPath = new Uri(assembly.Location).AbsolutePath;

            assemblyPath = Uri.UnescapeDataString(assemblyPath);
            assemblyPath = Path.GetFullPath(assemblyPath);

            return assemblyPath;
        }

        /// <summary>
        /// Gets all loadable types for the assembly specified
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>A collection of loadable types</returns>
        /// <remarks>
        /// This is an ugly workaround to avoid getting the exception
        /// ReflectionTypeLoadException while loading all types from
        /// an assembly.
        /// 
        /// For example, if the assembly contains types referencing 
        /// an assembly which is currently not available.
        /// 
        /// See this article for more https://tinyurl.com/ycloaxgg
        /// </remarks>
        public static Type[] GetLoadableTypes(this Assembly assembly)
        {
            Validate.IsNotNull(assembly);

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null).ToArray()!;
            }
        }
    }
}
