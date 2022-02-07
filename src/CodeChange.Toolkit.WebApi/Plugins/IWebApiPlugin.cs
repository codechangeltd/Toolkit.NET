namespace CodeChange.Toolkit.WebApi.Plugins
{
    using CodeChange.Toolkit.Plugins;
    using System.Net.Http;

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
        /// Executes the plug-in using a HTTP request message
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <returns>A HTTP response message containing the execution results</returns>
        HttpResponseMessage Execute(HttpRequestMessage request);
    }
}
