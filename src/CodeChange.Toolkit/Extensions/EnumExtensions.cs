namespace System;

using System.ComponentModel;

public static class EnumExtensions
{
    /// <summary>
    /// The description of an enum value using the description attribute or real value
    /// </summary>
    /// <param name="value">The enum value to get the description for</param>
    /// <returns>A string describing the enum</returns>
    public static string? GetDescription(this object value)
    {
        if (value == null)
        {
            return default;
        }

        var type = value.GetType();

        if (type.IsEnum)
        {
            var name = Enum.GetName(type, value);

            if (false == String.IsNullOrEmpty(name))
            {
                var field = type.GetField(name);

                if (field != null)
                {
                    return (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is not DescriptionAttribute attr) 
                        ? name.Spacify() : attr.Description;
                }
            }

            return null;
        }
        else
        {
            return value.ToString();
        }
    }
}
