namespace CodeChange.Toolkit.AspNetCore.Plugins
{
    using CodeChange.Toolkit.Plugins;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines a contract for a single Web API plug-in
    /// </summary>
    public interface IWebApiPlugin : IPlugin
    {
        /// <summary>
        /// Gets an array of parameters for the Web API call
        /// </summary>
        WebApiPluginParameterInfo[] Parameters { get; }

        /// <summary>
        /// Gets a single parameter matching the name specified
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <returns>The matching parameter information</returns>
        WebApiPluginParameterInfo GetParameter(string name);

        /// <summary>
        /// Executes the plug-in using a HTTP request
        /// </summary>
        /// <param name="request">The HTTP request</param>
        /// <returns>The result of the plugin execution</returns>
        ActionResult Execute(HttpRequest request);
    }
}
