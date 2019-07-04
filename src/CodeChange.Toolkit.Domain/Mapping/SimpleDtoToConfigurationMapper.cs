namespace CodeChange.Toolkit.Domain.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a simple implementation of a DTO to configuration mapper
    /// </summary>
    public class SimpleDtoToConfigurationMapper : IDtoToConfigurationMapper
    {
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache 
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

            var configuration = (TConfiguration)Map
            (
                dto,
                new TConfiguration()
            );

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
                var configuration = (TConfiguration)Map
                (
                    dto,
                    new TConfiguration()
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

        /// <summary>
        /// Maps a DTO to an existing configuration object
        /// </summary>
        /// <param name="dto">The DTO to map from</param>
        /// <param name="configuration">The configuration to map to</param>
        /// <returns>The mapped configuration</returns>
        private object Map
            (
                object dto,
                object configuration
            )
        {
            if (dto == null)
            {
                return configuration;
            }

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
                        // NOTE:
                        // This code handles the scenario where the two property 
                        // types are different but can be converted 
                        // (e.g. nested collection of capture DTOs that can be 
                        // mapped to a collection of configurations).

                        var canConvert = CanConvertValue
                        (
                            dtoProperty,
                            configProperty
                        );

                        if (canConvert)
                        {
                            object convertedValue;
                            var isNumeric = configPropertyType.IsNumeric();

                            if (isNumeric)
                            {
                                var nestedConfigurationList = new List<object>();
                                var collectionType = configPropertyType.GetEnumerableType();

                                foreach (var item in dtoPropertyValue as IEnumerable)
                                {
                                    var nestedConfiguration = Activator.CreateInstance
                                    (
                                        collectionType
                                    );

                                    nestedConfiguration = Map
                                    (
                                        dtoPropertyValue,
                                        nestedConfiguration
                                    );

                                    nestedConfigurationList.Add
                                    (
                                        nestedConfiguration
                                    );
                                }

                                if (collectionType.IsArray)
                                {
                                    // TODO: convert the array type to the expected type

                                    convertedValue = Array.ConvertAll
                                    (
                                        nestedConfigurationList.ToArray(),
                                        item => ObjectConverter.Convert(item, collectionType)
                                    );
                                }
                                else
                                {
                                    // TODO: convert the list type to the expected type

                                    convertedValue = nestedConfigurationList;
                                }
                            }
                            else
                            {
                                var nestedConfiguration = Activator.CreateInstance
                                (
                                    configPropertyType
                                );

                                convertedValue = Map
                                (
                                    dtoPropertyValue,
                                    nestedConfiguration
                                );
                            }

                            configProperty.SetValue
                            (
                                configuration,
                                convertedValue
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
                                $"Property {configProperty.Name} could not be mapped."
                            );
                        }
                    }
                }
            }

            return configuration;
        }

        /// <summary>
        /// Determines a DTO property can be converted for a configuration
        /// </summary>
        /// <param name="dtoProperty">The DTO property</param>
        /// <param name="configProperty">The configuration property</param>
        /// <returns>True, if the value can be converted; else false</returns>
        /// <remarks>
        /// The convention for auto converting from DTO to configuration 
        /// is that the DTO property type name must end with "Dto", 
        /// while the mapped configuration property type name must end 
        /// with "Configuration".
        /// </remarks>
        private bool CanConvertValue
            (
                PropertyInfo dtoProperty,
                PropertyInfo configProperty
            )
        {
            var dtoType = dtoProperty.PropertyType;
            var configType = configProperty.PropertyType;

            if (false == (dtoType.IsClass && configType.IsClass))
            {
                return false;
            }
            else if (false == dtoType.Name.EndsWith("Dto"))
            {
                return false;
            }
            else if (false == configType.Name.EndsWith("Configuration"))
            {
                return false;
            }
            else
            {
                var isDtoNumeric = dtoType.IsNumeric();
                var isConfigNumeric = configType.IsNumeric();

                return isDtoNumeric == isConfigNumeric;
            }
        }
    }
}
