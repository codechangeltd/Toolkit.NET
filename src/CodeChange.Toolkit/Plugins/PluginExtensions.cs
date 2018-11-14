namespace CodeChange.Toolkit.Plugins
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents various extension methods for the IPlugin interface
    /// </summary>
    public static class PluginExtensions
    {
        /// <summary>
        /// Gets the concrete interface type of the plug-in specified
        /// </summary>
        /// <param name="plugin">The plug-in</param>
        /// <returns>The plug-in interface type</returns>
        /// <remarks>
        /// The interface type returned will either be IPlugin or a derived type
        /// </remarks>
        public static Type GetPluginInterfaceType
            (
                this IPlugin plugin
            )
        {
            Validate.IsNotNull(plugin);

            var pluginType = plugin.GetType();
            var pluginInterfaces = pluginType.GetInterfaces();

            var interfaceType = pluginInterfaces.FirstOrDefault
            (
                t => t.IsAssignableFrom(typeof(IPlugin))
            );

            return interfaceType;
        }
    }
}
