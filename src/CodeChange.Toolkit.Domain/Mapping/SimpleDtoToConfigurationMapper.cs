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
                    p => p.Name.Equals
                    (
                        configProperty.Name,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

                if (dtoProperty != null)
                {
                    var configPropertyType = configProperty.PropertyType;

                    var valueToSet = dtoProperty.GetValue
                    (
                        dto
                    );

                    // Check if we need to convert the value before setting it
                    if (configPropertyType != dtoProperty.PropertyType)
                    {
                        var canConvertDirectly = dtoProperty.PropertyType.CanConvert
                        (
                            configPropertyType,
                            valueToSet
                        );

                        if (canConvertDirectly)
                        {
                            valueToSet = ObjectConverter.Convert
                            (
                                valueToSet,
                                configPropertyType
                            );
                        }
                        else
                        {
                            // NOTE:
                            // This code handles the scenario where the two property 
                            // types are different but can be converted using reflection
                            // (e.g. nested collection of DTOs that can be mapped to a
                            // collection of configurations).

                            var canConvertIndirectly = CanConvertComplexType
                            (
                                dtoProperty,
                                configProperty
                            );

                            if (canConvertIndirectly)
                            {
                                valueToSet = ConvertComplexType
                                (
                                    valueToSet,
                                    configPropertyType
                                );
                            }
                            else
                            {
                                var name = configProperty.Name;
                                var dtoPropertyType = dtoProperty.PropertyType;

                                throw new InvalidOperationException
                                (
                                    $"Property '{name}' could not be mapped. " +
                                    $"Type {dtoPropertyType.Name} cannot be " +
                                    $"converted to {configPropertyType.Name}."
                                );
                            }
                        }
                    }

                    configProperty.SetValue
                    (
                        configuration,
                        valueToSet
                    );

                    mappedNames.Add
                    (
                        configProperty.Name
                    );
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
        private bool CanConvertComplexType
            (
                PropertyInfo dtoProperty,
                PropertyInfo configProperty
            )
        {
            var dtoType = ResolveUnderlyingType
            (
                dtoProperty.PropertyType
            );

            var configType = ResolveUnderlyingType
            (
                configProperty.PropertyType
            );

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

            Type ResolveUnderlyingType(Type parentType)
            {
                var isEnumerable = parentType.IsEnumerable();

                if (isEnumerable)
                {
                    return parentType.GetEnumerableType();
                }
                else
                {
                    return parentType;
                }
            }
        }

        /// <summary>
        /// Converts a DTO property value to the configuration property type
        /// </summary>
        /// <param name="dtoPropertyValue">The DTO property value</param>
        /// <param name="configPropertyType">The configuration property type</param>
        /// <returns>The converted value</returns>
        private object ConvertComplexType
            (
                object dtoPropertyValue,
                Type configPropertyType
            )
        {
            if (dtoPropertyValue == null)
            {
                return null;
            }

            object convertedValue;
            var isEnumerable = configPropertyType.IsEnumerable();

            if (isEnumerable)
            {
                var nestedConfigList = new List<object>();
                var collectionType = configPropertyType.GetEnumerableType();

                foreach (var item in dtoPropertyValue as IEnumerable)
                {
                    var nestedConfiguration = Activator.CreateInstance
                    (
                        collectionType
                    );

                    nestedConfiguration = Map
                    (
                        item,
                        nestedConfiguration
                    );

                    nestedConfigList.Add
                    (
                        nestedConfiguration
                    );
                }

                if (collectionType.IsArray)
                {
                    // NOTE:
                    // We need to convert the nested configuration list 
                    // of objects to the correct array type. We do this 
                    // by creating a new array instance of the type expected
                    // and then setting its values to those in the list.

                    var newArray = Array.CreateInstance
                    (
                        collectionType,
                        nestedConfigList.Count
                    );

                    for (var i = 0; i < nestedConfigList.Count; i++)
                    {
                        newArray.SetValue(nestedConfigList[i], i);
                    }

                    convertedValue = newArray;
                }
                else
                {
                    // NOTE:
                    // We need to convert the nested configuration list 
                    // of objects to the correct type. We do this by 
                    // dynamically creating a new list of the type expected 
                    // and then populating it with the original list.

                    var listType = typeof(List<>).MakeGenericType
                    (
                        new[] { collectionType }
                    );

                    var convertedList = (IList)Activator.CreateInstance
                    (
                        listType
                    );

                    nestedConfigList.ForEach
                    (
                        item => convertedList.Add(item)
                    );

                    convertedValue = convertedList;
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

            return convertedValue;
        }
    }
}
