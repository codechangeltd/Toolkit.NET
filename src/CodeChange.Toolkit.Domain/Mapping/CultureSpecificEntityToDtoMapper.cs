namespace CodeChange.Toolkit.Domain.Mapping
{
    using CodeChange.Toolkit.Culture;
    using System;

    /// <summary>
    /// Represents a culture specific implementation of an entity to DTO mapper
    /// </summary>
    /// <remarks>
    /// There are some conventions to use with DTOs, these are:
    /// 
    /// - DTO class names must end with "Dto" and have an empty constructor
    /// - Nested collections of DTOs must be of type List<>
    /// - Properties named "Key" will be mapped to the entities lookup key
    /// </remarks>
    public class CultureSpecificEntityToDtoMapper : SimpleEntityToDtoMapper
    {
        public CultureSpecificEntityToDtoMapper(ILocaleConfiguration localeConfiguration)
        {
            Validate.IsNotNull(localeConfiguration);

            LocaleConfiguration = localeConfiguration;
        }
    }
}
