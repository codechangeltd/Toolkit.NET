namespace System
{
    using System.ComponentModel;

    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description of an enum value using the description attribute or real value
        /// </summary>
        /// <param name="value">The enum value to get the description for</param>
        /// <returns>A string describing the enum</returns>
        /// <exception cref="System.Reflection.AmbiguousMatchException"></exception>
        /// <exception cref="System.TypeLoadException"></exception>
        public static string GetDescription(this object value)
        {
            if (value == null)
            {
                return null;
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
                        var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                        return (attr == null) ? name.Spacify() : attr.Description;
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
}
