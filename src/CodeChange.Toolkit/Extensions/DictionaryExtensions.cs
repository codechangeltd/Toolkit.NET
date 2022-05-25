namespace System.Collections.Generic;

using System;
using System.Linq;
using System.Reflection;

public static class DictionaryExtensions
{
    /// <summary>
    /// Maps a dictionary of string-string to a target object
    /// </summary>
    /// <param name="dictionary">The dictionary</param>
    /// <param name="target">The target</param>
    /// <returns>The target to map to</returns>
    public static void MapTo(this Dictionary<string, object> dictionary, object target)
    {
        Validate.IsNotNull(dictionary);
        Validate.IsNotNull(target);

        var properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var matchingItem = dictionary.FirstOrDefault
            (
                x => x.Key.Equals(property.Name, StringComparison.OrdinalIgnoreCase)
            );

            var matchFound = !matchingItem.Equals(default(KeyValuePair<string, object>));

            if (matchFound)
            {
                var propertyType = property.PropertyType;
                var value = matchingItem.Value;

                if (value.GetType() == propertyType)
                {
                    property.SetValue(target, value);
                }
                else
                {
                    var convertedValue = ObjectConverter.Convert(value, propertyType);

                    property.SetValue(target, convertedValue);
                }
            }
        }
    }
}
