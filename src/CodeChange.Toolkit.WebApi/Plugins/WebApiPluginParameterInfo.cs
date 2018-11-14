namespace CodeChange.Toolkit.WebApi.Plugins
{
    using System;

    /// <summary>
    /// Represents information about a single Web API plugin parameter
    /// </summary>
    public class WebApiPluginParameterInfo
    {
        /// <summary>
        /// Constructs the parameter with a name and value type
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="valueType">The parameter value type</param>
        internal WebApiPluginParameterInfo
            (
                string name,
                Type valueType
            )
        {
            Validate.IsNotEmpty(name);
            Validate.IsNotNull(valueType);

            this.Name = name;
            this.ValueType = valueType;
        }

        /// <summary>
        /// Constructs the parameter with a name and value type
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="valueType">The parameter value type</param>
        /// <param name="defaultValue">The default value</param>
        /// <param name="allowNull">True, if null is allowed</param>
        internal WebApiPluginParameterInfo
            (
                string name,
                Type valueType,
                object defaultValue,
                bool allowNull
            )
        {
            Validate.IsNotEmpty(name);
            Validate.IsNotNull(valueType);

            this.Name = name;
            this.ValueType = valueType;
            this.DefaultValue = defaultValue;
            this.AllowNull = allowNull;
        }

        /// <summary>
        /// Gets the parameter name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parameter value type
        /// </summary>
        public Type ValueType { get; private set; }

        /// <summary>
        /// Gets the parameters default value
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Gets a flag indicating if null is allowed
        /// </summary>
        public bool AllowNull { get; private set; }
    }
}
