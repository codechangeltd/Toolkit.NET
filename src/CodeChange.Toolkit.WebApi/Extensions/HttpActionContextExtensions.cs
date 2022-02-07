namespace System.Web.Http
{
    using Newtonsoft.Json.Linq;
    using Nito.AsyncEx.Synchronous;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http.Controllers;

    public static class HttpActionContextExtensions
    {
        /// <summary>
        /// Resolves a service instance for the service type specified
        /// </summary>
        /// <typeparam name="TService">The service type to resolve</typeparam>
        /// <param name="actionContext">The HTTP action context</param>
        /// <returns>The service instance</returns>
        public static TService ResolveService<TService>(this HttpActionContext actionContext) where TService : class
        {
            Validate.IsNotNull(actionContext);

            // Get the request lifetime scope so you can resolve services
            var requestScope = actionContext.Request.GetDependencyScope();

            if (requestScope == null)
            {
                throw new InvalidOperationException
                (
                    "The dependency scope has not been instantiated for this request."
                );
            }

            // Resolve the service you want to use
            var service = requestScope.GetService(typeof(TService)) as TService;

            if (service == null)
            {
                var typeName = typeof(TService).ToString();
                
                throw new TypeLoadException
                (
                    $"No service could be resolved for the type {typeName}."
                );
            }

            return service;
        }
        
        /// <summary>
        /// Gets a single query string or posted body parameter value
        /// </summary>
        /// <param name="actionContext">The action context</param>
        /// <param name="parameterName">The parameter name</param>
        /// <returns>The parameter value</returns>
        public static string GetParameterValue(this HttpActionContext actionContext, string parameterName)
        {
            Validate.IsNotNull(actionContext);
            Validate.IsNotNull(parameterName);

            var value = String.Empty;

            // Convert query string parameters into a dictionary for ease of access
            var queryStringCollection = actionContext.Request
                .GetQueryNameValuePairs()
                .ToDictionary(s => s.Key, s => s.Value);

            if (queryStringCollection.ContainsKey(parameterName))
            {
                value = queryStringCollection[parameterName];
            }
            else
            {
                var methodType = actionContext.Request.Method.Method;

                if (methodType.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    var task = actionContext.Request.Content.ReadAsStringAsync();
                    var body = task.WaitAndUnwrapException();

                    var json = JObject.Parse(body);
                    var token = json[parameterName];

                    if (token != null)
                    {
                        value = token.ToString();
                    }
                }
            }

            if (String.IsNullOrEmpty(value) || value.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                value = null;
            }

            return value;
        }
    }
}
