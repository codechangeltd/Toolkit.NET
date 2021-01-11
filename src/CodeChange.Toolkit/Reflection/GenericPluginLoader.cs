namespace System.Reflection
{
    using CodeChange.Toolkit.Reflection;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Represents a generic plug-in loader that can dynamically load types
    /// </summary>
    /// <typeparam name="T">The plug-in type to load</typeparam>
    public class GenericPluginLoader<T>
    {
        /// <summary>
        /// Loads all assemblies found in the path specified
        /// </summary>
        /// <param name="path">The path to search</param>
        /// <returns>A collection of the assemblies that were loaded</returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="BadImageFormatException"></exception>
        public IEnumerable<Assembly> LoadAssemblies(string path)
        {
            Validate.IsNotEmpty(path);

            if (Directory.Exists(path))
            {
                var dllFileNames = Directory.GetFiles(path, "*.dll");
                var assemblies = new List<Assembly>(dllFileNames.Length);

                foreach (var dllFile in dllFileNames)
                {
                    var an = AssemblyName.GetAssemblyName(dllFile);
                    var assembly = Assembly.Load(an);

                    assemblies.Add(assembly);
                }

                return assemblies;
            }
            else
            {
                throw new IOException($"The path '{path}' does not exist.");
            }
        }

        /// <summary>
        /// Loads all plug-ins contained in all assemblies that are found in the path specified
        /// </summary>
        /// <param name="path">The path of the plug-ins directory</param>
        /// <param name="activator">THe type activator (optional)</param>
        /// <returns>A collection of matching plug-in instances</returns>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="BadImageFormatException"></exception>
        /// <exception cref="System.Reflection.AmbiguousMatchException"></exception>
        /// <exception cref="System.Reflection.TargetInvocationException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="System.Runtime.InteropServices.InvalidComObjectException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="System.Runtime.InteropServices.COMException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        public IEnumerable<T> LoadPlugins(string path, ITypeActivator activator = null)
        {
            Validate.IsNotEmpty(path);

            var assemblies = LoadAssemblies(path);
            var pluginType = typeof(T);
            var pluginTypes = new List<Type>();

            // Build a collection of plug-in types from each assembly
            foreach (var assembly in assemblies)
            {
                if (assembly != null)
                {
                    var types = assembly.GetLoadableTypes();

                    foreach (var type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if (type.GetInterface(pluginType.FullName) != null)
                            {
                                pluginTypes.Add(type);
                            }
                        }
                    }
                }
            }

            var plugins = new List<T>(pluginTypes.Count);

            // Build a collection of plug-in instances from the plug-in types
            foreach (var type in pluginTypes)
            {
                T plugin;

                if (activator != null)
                {
                    plugin = (T)activator.GetInstance(type);
                }
                else
                {
                    plugin = (T)Activator.CreateInstance(type);
                }

                plugins.Add(plugin);
            }

            return plugins;
        }
    }
}
