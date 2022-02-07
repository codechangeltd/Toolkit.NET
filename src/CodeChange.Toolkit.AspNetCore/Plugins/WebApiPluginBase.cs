namespace CodeChange.Toolkit.AspNetCore.Plugins
{
    using CodeChange.Toolkit.Plugins;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    
    /// <summary>
    /// Represents a base class for all Web API plug-in implementations
    /// </summary>
    public abstract class WebApiPluginBase : PluginBase, IWebApiPlugin
    {
        private List<WebApiPluginParameterInfo> _actionParameters;

        /// <summary>
        /// Overrides the name so we can remove the WebApi part
        /// </summary>
        public override string Name => base.Name.Replace("WebApi", String.Empty);

        /// <summary>
        /// Gets an array of parameters for the Web API call
        /// </summary>
        public WebApiPluginParameterInfo[] Parameters
        {
            get
            {
                if (_actionParameters == null)
                {
                    return new WebApiPluginParameterInfo[] {};
                }
                else
                {
                    return _actionParameters.ToArray();
                }
            }
        }

        /// <summary>
        /// Defines a parameter using the default type (string)
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="allowNull">True, if null values are allowed</param>
        protected void DefineParameter(string name, bool allowNull = true)
        {
            Validate.IsNotEmpty(name);

            var parameter = new WebApiPluginParameterInfo(name, typeof(string), null, allowNull);

            DefineParameter(parameter);
        }

        /// <summary>
        /// Defines a parameter using the details specified
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="valueType">The parameter value type</param>
        protected void DefineParameter(string name, Type valueType)
        {
            Validate.IsNotEmpty(name);
            Validate.IsNotNull(valueType);

            var parameter = new WebApiPluginParameterInfo(name, valueType, null, true);

            DefineParameter(parameter);
        }

        /// <summary>
        /// Defines a parameter using the details specified
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="valueType">The parameter value type</param>
        /// <param name="defaultValue">The default value</param>
        /// <param name="allowNull">True, if null is allowed</param>
        protected void DefineParameter(string name, Type valueType, object defaultValue, bool allowNull)
        {
            Validate.IsNotEmpty(name);
            Validate.IsNotNull(valueType);

            var parameter = new WebApiPluginParameterInfo(name, valueType, defaultValue, allowNull);

            DefineParameter(parameter);
        }

        /// <summary>
        /// Defines a parameter
        /// </summary>
        /// <param name="parameter">The parameter information</param>
        private void DefineParameter(WebApiPluginParameterInfo parameter)
        {
            Validate.IsNotNull(parameter);

            var name = parameter.Name;

            if (_actionParameters == null)
            {
                _actionParameters = new List<WebApiPluginParameterInfo>();
            }

            var matchFound = this.Parameters.Any
            (
                _ => _.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
            );

            if (matchFound)
            {
                throw new InvalidOperationException
                (
                    $"A parameter named '{name}' has already been defined."
                );
            }

            _actionParameters.Add(parameter);
        }

        /// <summary>
        /// Gets a single parameter matching the name specified
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <returns>The matching parameter information</returns>
        public virtual WebApiPluginParameterInfo GetParameter(string name)
        {
            Validate.IsNotEmpty(name);

            var parameter = this.Parameters.FirstOrDefault
            (
                _ => _.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
            );

            if (parameter == null)
            {
                throw new KeyNotFoundException
                (
                    $"No parameter was found matching the name '{name}'."
                );
            }

            return parameter;
        }

        /// <summary>
        /// Gets a parameter value as the type specified
        /// </summary>
        /// <typeparam name="T">The value type</typeparam>
        /// <param name="parameterValues">A collection of parameter values</param>
        /// <param name="name">The parameter name</param>
        /// <returns>The parameter value as the type specified</returns>
        protected virtual T GetParameterValue<T>(Dictionary<string, string> parameterValues, string name)
        {
            Validate.IsNotNull(parameterValues);
            Validate.IsNotEmpty(name);

            var found = parameterValues.ContainsKey(name);

            // Use the value if it was found, otherwise use the default
            if (found)
            {
                var rawValue = parameterValues[name];

                return new GenericObjectToTypeConverter<T>().Convert(rawValue);
            }
            else
            {
                var parameter = GetParameter(name);
                var defaultValue = parameter.DefaultValue;

                return new GenericObjectToTypeConverter<T>().Convert(defaultValue);
            }
        }

        /// <summary>
        /// Validates the parameter values specified against the parameters
        /// </summary>
        /// <param name="parameterValues">The parameter values</param>
        protected virtual void ValidateParameterValues(Dictionary<string, string> parameterValues)
        {
            Validate.IsNotNull(parameterValues);

            var parameters = this.Parameters;

            if (parameters.Length == 0)
            {
                return;
            }
            else
            {
                foreach (var parameter in parameters)
                {
                    var name = parameter.Name;

                    // Make sure a value is supplied if the parameter is required
                    if (false == parameter.AllowNull)
                    {
                        if (false == parameterValues.ContainsKey(name))
                        {
                            throw new InvalidOperationException
                            (
                                $"No value was supplied for the parameter '{name}'."
                            );
                        }
                        else if (String.IsNullOrEmpty(parameterValues[name]))
                        {
                            throw new InvalidOperationException
                            (
                                $"The value for the parameter '{name}' cannot be null."
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes the plug-in using a HTTP request
        /// </summary>
        /// <param name="request">The HTTP request</param>
        /// <returns>The result of the plugin execution</returns>
        public virtual ActionResult Execute(HttpRequest request)
        {
            Validate.IsNotNull(request);

            var parameterValues = new Dictionary<string, string>();
            var query = request.Query;

            if (query != null)
            {
                foreach (var item in query)
                {
                    parameterValues.Add(item.Key, item.Value.FirstOrDefault());
                }
            }

            ValidateParameterValues(parameterValues);
            
            try
            {
                var result = InvokeMethod(request, parameterValues);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                LogError(ex, request);

                while (ex.InnerException != null)
                {
                    LogError(ex.InnerException, request);

                    ex = ex.InnerException;
                }

                return new ObjectResult
                (
                    new ProblemDetails()
                    {
                        Detail = ex.Message,
                        Status = (int)HttpStatusCode.InternalServerError,
                        Instance = request.GetEncodedUrl()
                    }
                );
            }
        }

        /// <summary>
        /// When implemented, invokes the method using the parameter values specified
        /// </summary>
        /// <param name="request">The HTTP request</param>
        /// <param name="parameterValues">The parameter values</param>
        /// <returns>The value returned</returns>
        /// <remarks>
        /// For methods that return void, null should be returned instead.
        /// </remarks>
        protected abstract object InvokeMethod(HttpRequest request, Dictionary<string, string> parameterValues);

        /// <summary>
        /// Logs an error using the exception and HTTP request specified
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="request">The HTTP request</param>
        protected virtual void LogError(Exception ex, HttpRequest request)
        {
            Validate.IsNotNull(ex);

            var message = ex.Message;
            var url = request.GetDisplayUrl();

            Debug.WriteLine($"The error '{message}' occurred while invoking {url}.");
        }
    }
}
