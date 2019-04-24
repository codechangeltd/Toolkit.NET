namespace CodeChange.Toolkit.Domain.Mapping
{
    using CodeChange.Toolkit.Culture;
    using CodeChange.Toolkit.Domain.Aggregate;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a simple implementation of an entity to DTO mapper
    /// </summary>
    /// <remarks>
    /// There are some conventions to use with DTOs, these are:
    /// 
    /// - DTO class names must end with "Dto" and have an empty constructor
    /// - Nested collections of DTOs must be of type List<>
    /// - Properties named "Key" will be mapped to the entities lookup key
    /// </remarks>
    public class SimpleEntityToDtoMapper : IEntityToDtoMapper
    {
        private static Dictionary<Type, PropertyInfo[]> _propertyCache
            = new Dictionary<Type, PropertyInfo[]>();

        private static Dictionary<Type, MethodInfo[]> _methodCache
            = new Dictionary<Type, MethodInfo[]>();

        /// <summary>
        /// Constructs the mapper with default configuration
        /// </summary>
        public SimpleEntityToDtoMapper()
        {
            this.LocaleConfiguration = new DefaultLocaleConfiguration();
        }

        /// <summary>
        /// Gets the locale configuration for the culture
        /// </summary>
        protected ILocaleConfiguration LocaleConfiguration { get; set; }

        /// <summary>
        /// Maps a single entity to a DTO
        /// </summary>
        /// <typeparam name="TEntity">The aggregate entity type</typeparam>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <param name="entity">The entity to map</param>
        /// <returns>The mapped DTO</returns>
        public TDto Map<TEntity, TDto>
            (
                TEntity entity
            )

            where TEntity : IAggregateEntity
            where TDto : class, new()
        {
            if (entity == null)
            {
                return null;
            }
            else
            {
                var dto = (object)new TDto();

                Map(entity, ref dto, true);

                return (TDto)dto;
            }
        }

        /// <summary>
        /// Maps a single entity to a DTO
        /// </summary>
        /// <param name="entity">The entity to map</param>
        /// <param name="dto">The DTO to map to</param>
        /// <param name="mapNestedDtos">If true, all nested DTOs are mapped</param>
        /// <returns>The mapped DTO</returns>
        protected virtual void Map
            (
                IAggregateEntity entity,
                ref object dto,
                bool mapNestedDtos
            )
        {
            if (entity == null)
            {
                dto = null;

                return;
            }
            else
            {
                var entityProperties = GetMappableProperties(entity);
                var dtoProperties = GetMappableProperties(dto);
                var mappedNames = new List<string>();

                foreach (var dtoProperty in dtoProperties)
                {
                    if (dtoProperty.Name == "Key")
                    {
                        dtoProperty.SetValue
                        (
                            dto,
                            entity.LookupKey
                        );
                    }
                    else
                    {
                        var alreadyMapped = mappedNames.Contains
                        (
                            dtoProperty.Name
                        );

                        if (alreadyMapped)
                        {
                            continue;
                        }

                        var entityProperty = entityProperties.FirstOrDefault
                        (
                            p => p.Name == dtoProperty.Name
                        );

                        if (entityProperty != null)
                        {
                            TryMapProperty
                            (
                                entity,
                                ref dto,
                                entityProperty,
                                dtoProperty,
                                mapNestedDtos
                            );
                        }
                        else
                        {
                            TryMapToMethod
                            (
                                entity,
                                ref dto,
                                dtoProperty,
                                mapNestedDtos
                            );
                        }

                        mappedNames.Add
                        (
                            dtoProperty.Name
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Tries to map a property on a DTO to a property on an entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="dto">The DTO</param>
        /// <param name="entityProperty">The entity property</param>
        /// <param name="dtoProperty">The DTO property</param>
        /// <param name="mapNestedDtos">If true, all nested DTOs are mapped</param>
        protected virtual void TryMapProperty
            (
                IAggregateEntity entity,
                ref object dto,
                PropertyInfo entityProperty,
                PropertyInfo dtoProperty,
                bool mapNestedDtos
            )
        {
            var dtoPropertyType = dtoProperty.PropertyType;
            var entityPropertyValue = default(object);
            
            if (false == (dtoPropertyType == typeof(ICollection<>)))
            {
                entityPropertyValue = entityProperty.GetValue
	            (
	                entity
	            );
            }

            if (dtoPropertyType == entityProperty.PropertyType)
            {
                if (dtoPropertyType == typeof(DateTime))
                {
                    var date = (DateTime)entityPropertyValue;

                    entityPropertyValue = date.ToLocalTime
                    (
                        this.LocaleConfiguration
                    );
                }

                if (dtoPropertyType == typeof(DateTime?))
                {
                    var date = (DateTime?)entityPropertyValue;

                    entityPropertyValue = date.ToLocalTime
                    (
                        this.LocaleConfiguration
                    );
                }

                dtoProperty.SetValue
                (
                    dto,
                    entityPropertyValue
                );
            }
            else if (dtoPropertyType == typeof(string))
            {
                if (entityPropertyValue != null)
                {
                    dtoProperty.SetValue
                    (
                        dto,
                        entityPropertyValue.ToString()
                    );
                }
            }
            else if (mapNestedDtos)
            {
                var isList =
                (
                    dtoPropertyType.IsGenericType
                        && dtoPropertyType.GetGenericTypeDefinition() == typeof(List<>)
                );

                if (isList)
                {
                    var dtoListType = dtoPropertyType.GetGenericArguments()[0];
                    var entityListType = entityProperty.PropertyType.GetGenericArguments()[0];

                    var isValidDto = IsValidDtoType
                    (
                        dtoListType,
                        entityListType
                    );

                    if (false == isValidDto)
                    {
                        throw new InvalidOperationException
                        (
                            "The list type is not a valid DTO."
                        );
                    }

                    var isValidEntityCollection = entityListType.ImplementsInterface
                    (
                        typeof(IAggregateEntity)
                    );

                    if (false == isValidEntityCollection)
                    {
                        var typeName = entityListType.Name;
                        var propertyName = entityProperty.Name;

                        throw new InvalidOperationException
                        (
                            $"{typeName} on {propertyName} does not implement {nameof(IAggregateEntity)}."
                        );
                    }

                    var dtoList = (IList)Activator.CreateInstance
                    (
                        dtoPropertyType
                    );

                    var entityCollection = (IEnumerable)entityProperty.GetValue
                    (
                        entity
                    );

                    foreach (IAggregateEntity childEntity in entityCollection)
                    {
                        var childDto = Activator.CreateInstance
                        (
                            dtoListType
                        );

                        Map(childEntity, ref childDto, mapNestedDtos);

                        dtoList.Add(childDto);
                    }

                    dtoProperty.SetValue(dto, dtoList);
                }
                else
                {
                    var isValidDto = IsValidDtoType
                    (
                        dtoPropertyType,
                        entityProperty.PropertyType
                    );

                    if (isValidDto)
                    {
                        var childEntity = (IAggregateEntity)entityProperty.GetValue
                        (
                            entity
                        );

                        var childDto = Activator.CreateInstance
                        (
                            dtoPropertyType
                        );

                        Map(childEntity, ref childDto, mapNestedDtos);

                        dtoProperty.SetValue(dto, childDto);
                    }
                    else
                    {
                        throw new InvalidOperationException
                        (
                            $"The property {dtoProperty.Name} could not be mapped."
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Tries to map a property on a DTO to a method on an entity
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="dto">The DTO</param>
        /// <param name="dtoProperty">The DTO property</param>
        /// <param name="mapNestedDtos">If true, all nested DTOs are mapped</param>
        protected virtual void TryMapToMethod
            (
                IAggregateEntity entity,
                ref object dto,
                PropertyInfo dtoProperty,
                bool mapNestedDtos = false
            )
        {
            var methods = GetMappableMethods(entity);

            var candidates = methods.Where
            (
                m => m.ReturnType.IsAssignableFrom
                (
                    dtoProperty.PropertyType
                )
                ||
                m.ReturnType.ImplementsInterface
                (
                    typeof(IAggregateEntity)
                )
            );

            foreach (var method in candidates)
            {
                var matches = method.Name.Contains
                (
                    dtoProperty.Name
                );

                if (matches)
                {
                    var result = default(object);
                    var success = default(bool);
                    var isAggregate = false;

                    try
                    {
                        result = method.Invoke
                        (
                            entity,
                            null
                        );

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        // NOTE: we are ignoring errors when mapping to methods
                        success = false;

                        Debug.WriteLine
                        (
                            $"Entity Mapping Error: {ex.Message}"
                        );
                    }
                    
                    if (success)
                    {
                        if (result != null)
                        {
                            isAggregate = result.GetType().ImplementsInterface
                            (
                                typeof(IAggregateEntity)
                            );
                        }

                        if (isAggregate)
                        {
                            if (mapNestedDtos)
                            {
                                var childDto = Activator.CreateInstance
                                (
                                    dtoProperty.PropertyType
                                );

                                Map
                                (
                                    (IAggregateEntity)result,
                                    ref childDto,
                                    mapNestedDtos
                                );

                                dtoProperty.SetValue
                                (
                                    dto,
                                    childDto
                                );
                            }
                        }
                        else
                        {
                            dtoProperty.SetValue
                            (
                                dto,
                                result
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the a DTOs property type is a valid DTO type
        /// </summary>
        /// <param name="dtoPropertyType">The DTo property type to check</param>
        /// <param name="entityPropertyType">The matching entity type</param>
        /// <returns>True, if the type is a valid DTO type; otherwise false</returns>
        protected virtual bool IsValidDtoType
            (
                Type dtoPropertyType,
                Type entityPropertyType
            )
        {
            var dtoPropertyName = dtoPropertyType.Name;
            var entityPropertyName = entityPropertyType.Name;

            if (dtoPropertyType.IsValueType)
            {
                return false;
            }

            if (dtoPropertyType.GetConstructor(Type.EmptyTypes) == null)
            {
                return false;
            }

            if (false == dtoPropertyName.EndsWith("Dto"))
            {
                return false;
            }

            if (false == dtoPropertyName.StartsWith(entityPropertyName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Maps a collection of entities to a collection of DTOs
        /// </summary>
        /// <typeparam name="TEntity">The aggregate entity type</typeparam>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <param name="entities">The collection of entities to map</param>
        /// <param name="mapNestedDtos">If true, all nested DTOs are mapped</param>
        /// <returns>A collection of mapped DTOs</returns>
        public IEnumerable<TDto> Map<TEntity, TDto>
            (
                IEnumerable<TEntity> entities,
                bool mapNestedDtos = false
            )

            where TEntity : IAggregateEntity
            where TDto : class, new()
        {
            if (entities == null)
            {
                return null;
            }
            else
            {
                var mappedDtos = new List<TDto>();

                foreach (var entity in entities.ToList())
                {
                    var dto = (object)new TDto();

                    Map(entity, ref dto, mapNestedDtos);

                    mappedDtos.Add
                    (
                        (TDto)dto
                    );
                }

                return mappedDtos;
            }
        }

        /// <summary>
        /// Gets a collection of "map-able" properties from an object
        /// </summary>
        /// <param name="o">The object</param>
        /// <returns>A collection of matching properties</returns>
        protected virtual IEnumerable<PropertyInfo> GetMappableProperties
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
        /// Gets a collection of "map-able" methods from an object
        /// </summary>
        /// <param name="o">The object</param>
        /// <returns>A collection of matching methods</returns>
        protected virtual IEnumerable<MethodInfo> GetMappableMethods
            (
                object o
            )
        {
            var type = o.GetType();

            if (_methodCache.ContainsKey(type))
            {
                return _methodCache[type];
            }
            else
            {
                var methods = type.GetMethods
                (
                    BindingFlags.Public | BindingFlags.Instance
                );

                methods = methods.Where
                (
                    m => m.GetParameters().Length == 0
                )
                .ToArray();

                _methodCache[type] = methods;

                return methods;
            }
        }
    }
}
