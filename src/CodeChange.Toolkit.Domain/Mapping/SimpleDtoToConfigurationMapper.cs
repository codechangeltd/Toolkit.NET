namespace CodeChange.Toolkit.Domain.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a simple implementation of a DTO to configuration mapper
    /// </summary>
    public class SimpleDtoToConfigurationMapper : IDtoToConfigurationMapper
    {
        private static Dictionary<Type, PropertyInfo[]> _propertyCache 
            = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Maps a DTO to a new configuration object
        /// </summary>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <typeparam name="TConfiguration">The configuration type</typeparam>
        /// <param name="dto">The DTO to map</param>
        /// <returns>The mapped configuration</returns>
        public TConfiguration Map<TDto, TConfiguration>
            (
                TDto dto
            )

            where TConfiguration : class, new()
        {
            if (dto == null)
            {
                throw new ArgumentException
                (
                    "The DTO cannot be null."
                );
            }

            var configuration = new TConfiguration();
            var configProperties = GetMappableProperties(configuration);
            var dtoProperties = GetMappableProperties(dto);
            var mappedNames = new List<string>();

            foreach (var configProperty in configProperties)
            {
                var alreadyMapped = mappedNames.Contains
                (
                    configProperty.Name
                );

                if (alreadyMapped)
                {
                    continue;
                }

                var dtoProperty = dtoProperties.FirstOrDefault
                (
                    p => p.Name == configProperty.Name
                );
                
                if (dtoProperty != null)
                {
                    var configPropertyType = configProperty.PropertyType;

                    var dtoPropertyValue = dtoProperty.GetValue
                    (
                        dto
                    );

                    if (configPropertyType == dtoProperty.PropertyType)
                    {
                        configProperty.SetValue
                        (
                            configuration,
                            dtoPropertyValue
                        );

                        mappedNames.Add
                        (
                            configProperty.Name
                        );
                    }
                    else
                    {
                        throw new InvalidOperationException
                        (
                            $"The property {configProperty.Name} could not be mapped."
                        );
                    }
                }
            }

            return configuration;
        }

        /// <summary>
        /// Maps a collection of DTOs to a collection of configurations
        /// </summary>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <typeparam name="TConfiguration">The configuration type</typeparam>
        /// <param name="dtos">The DTOs to map</param>
        /// <returns>The mapped configuration</returns>
        public IEnumerable<TConfiguration> Map<TDto, TConfiguration>
            (
                IEnumerable<TDto> dtos
            )
            
            where TConfiguration : class, new()
        {
            if (dtos == null)
            {
                throw new ArgumentException
                (
                    "The DTO collection cannot be null."
                );
            }

            var configurations = new List<TConfiguration>();

            foreach (var dto in dtos)
            {
                var configuration = Map<TDto, TConfiguration>
                (
                    dto
                );

                configurations.Add(configuration);
            }

            return configurations;
        }
        
        /// <summary>
        /// Gets a collection of "map-able" properties from an object
        /// </summary>
        /// <param name="o">The object</param>
        /// <returns>A collection of matching properties</returns>
        private IEnumerable<PropertyInfo> GetMappableProperties
            (
                object o
            )
        {
            var type = o.GetType();

            if (_propertyCache.ContainsKey(type))
            {
                return _propertyCache[type];
            }
            else
            {
                var properties = type.GetProperties
                (
                    BindingFlags.Public | BindingFlags.Instance
                );

                properties = properties.Where
                (
                    p => p.CanRead
                )
                .ToArray();

                _propertyCache[type] = properties;

                return properties;
            }
        }
    }
}
