namespace System
{
    using System.Collections.Generic;
    
    public class EnumInspector
    {
        /// <summary>
        /// Gets a collection of enum item info from the enum specified
        /// </summary>
        /// <param name="e">The enum</param>
        /// <returns>A collection of matching enum info</returns>
        public static IEnumerable<EnumItemInfo> GetEnumInfo(Enum e)
        {
            Validate.IsNotNull(e);

            return GetEnumInfo(e.GetType());
        }

        /// <summary>
        /// Gets a collection of enum item info from the enum type specified
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <returns>A collection of matching enum info</returns>
        public static IEnumerable<EnumItemInfo> GetEnumInfo(Type enumType)
        {
            Validate.IsNotNull(enumType);

            if (false == enumType.IsEnum)
            {
                throw new ArgumentException($"The type {enumType.Name} is not a valid enum.");
            }

            var enumInfo = new List<EnumItemInfo>();
            var enumValues = Enum.GetValues(enumType);

            foreach (var entry in enumValues)
            {
                var value = (int)entry;
                var name = entry.ToString()!;
                var description = entry.GetDescription();

                enumInfo.Add(new EnumItemInfo(value, name, description));
            }

            return enumInfo;
        }
    }
}
