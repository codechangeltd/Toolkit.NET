namespace CodeChange.Toolkit.AspNetCore.Plugins
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Reflection;

    /// <summary>
    /// Represents a web API specific contract resolver for JSON serialisation
    /// </summary>
    /// <remarks>
    /// The contract resolver is based on the camel case property names contract resolver.
    /// The intention is that the generated JSON is used for web API purposes and any binary 
    /// data should be ignored as this wont be used by the caller.
    /// </remarks>
    public class WebApiPluginContractResolver : CamelCasePropertyNamesContractResolver
    {
        /// <summary>
        /// Overridden so we can ignore all byte array data types
        /// </summary>
        /// <param name="member">The member info</param>
        /// <param name="memberSerialization">The member serialization</param>
        /// <returns>The JSON property that was created</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(byte[]))
            {
                property.ShouldSerialize = instance => { return false; };
            }

            return property;
        }
    }
}
