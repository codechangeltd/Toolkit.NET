namespace CodeChange.Toolkit.WebApi.Plugins
{
    using CodeChange.Toolkit.Plugins;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

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

            var matchFound = Parameters.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (matchFound)
            {
                throw new InvalidOperationException
                (
                    $"A parameter with the name '{name}' has already been defined."
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

            var parameter = Parameters.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

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

            if (Parameters.Length == 0)
            {
                return;
            }
            else
            {
                foreach (var parameter in Parameters)
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
        /// Executes the plug-in using a HTTP request message
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <returns>A HTTP response message containing the execution results</returns>
        public virtual HttpResponseMessage Execute(HttpRequestMessage request)
        {
            Validate.IsNotNull(request);

            var uri = request.RequestUri;
            var parameterValues = uri.ParseQueryString().ToDictionary();
            
            ValidateParameterValues(parameterValues);
            
            try
            {
                var result = InvokeMethod(request, parameterValues);

                return CreateSuccessResponse(request, result);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        return request.CreateResponse(response.StatusCode, ex.Message);
                    }
                    else
                    {
                        return RespondToUnhandledException(ex);
                    }
                }
                else
                {
                    return RespondToUnhandledException(ex);
                }
            }
            catch (Exception ex)
            {
                return RespondToUnhandledException(ex);
            }

            HttpResponseMessage RespondToUnhandledException(Exception ex)
            {
                LogError(ex, request);

                while (ex.InnerException != null)
                {
                    LogError(ex.InnerException, request);

                    ex = ex.InnerException;
                }

                return request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Creates a success HTTP response message from the value specified
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <param name="value">The value to output</param>
        /// <returns>The HTTP response message created</returns>
        protected virtual HttpResponseMessage CreateSuccessResponse(HttpRequestMessage request, object value)
        {
            Validate.IsNotNull(request);

            if (value == null)
            {
                return request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                var formatter = GetMediaTypeFormatter();

                return request.CreateResponse(HttpStatusCode.OK, value, formatter);
            }
        }

        /// <summary>
        /// Gets the default media type formatter
        /// </summary>
        /// <returns>The media type formatter</returns>
        protected virtual MediaTypeFormatter GetMediaTypeFormatter()
        {
            var jsonFormatter = new JsonMediaTypeFormatter
            {
                // Specify camel case JSON serialization
                SerializerSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new WebApiPluginContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            };

            jsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            return jsonFormatter;
        }

        /// <summary>
        /// When implemented, invokes the method using the parameter values specified
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <param name="parameterValues">The parameter values</param>
        /// <returns>The value returned</returns>
        /// <remarks>
        /// For methods that return void, null should be returned instead.
        /// </remarks>
        protected abstract object InvokeMethod(HttpRequestMessage request, Dictionary<string, string> parameterValues);

        /// <summary>
        /// Logs an error using the exception and HTTP request specified
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="request">The HTTP request</param>
        protected virtual void LogError(Exception ex, HttpRequestMessage request)
        {
            Validate.IsNotNull(ex);

            var message = ex.Message;
            var url = request.RequestUri.ToString();

            Debug.WriteLine($"The following error occurred '{message}' while invoking {url}.");
        }
    }
}
