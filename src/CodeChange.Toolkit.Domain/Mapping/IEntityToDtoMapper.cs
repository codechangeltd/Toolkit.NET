namespace CodeChange.Toolkit.Domain.Mapping
{
    using CodeChange.Toolkit.Domain.Aggregate;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a contract for an aggregate entity to DTO mapper
    /// </summary>
    /// <remarks>
    /// There are some conventions to use with DTOs, these are:
    /// 
    /// - DTO class names must end with "Dto" and have an empty constructor
    /// - Nested collections of DTOs must be of type List<>
    /// - Nested collection type names must begin with the type name being mapped to
    /// - Properties named "Key" will be mapped to the entities lookup key
    /// </remarks>
    public interface IEntityToDtoMapper
    {
        /// <summary>
        /// Maps a single entity to a DTO
        /// </summary>
        /// <typeparam name="TEntity">The aggregate entity type</typeparam>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <param name="entity">The entity to map</param>
        /// <param name="mapNestedDtos">If true, all nested DTOs are mapped</param>
        /// <returns>The mapped DTO</returns>
        TDto Map<TEntity, TDto>(TEntity entity, bool mapNestedDtos = true)
            where TEntity : IAggregateEntity
            where TDto : class, new();

        /// <summary>
        /// Maps a collection of entities to a collection of DTOs
        /// </summary>
        /// <typeparam name="TEntity">The aggregate entity type</typeparam>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <param name="entities">The collection of entities to map</param>
        /// <param name="mapNestedDtos">If true, all nested DTOs are mapped</param>
        /// <returns>A collection of mapped DTOs</returns>
        IEnumerable<TDto> Map<TEntity, TDto>(IEnumerable<TEntity> entities, bool mapNestedDtos = false)
            where TEntity : IAggregateEntity
            where TDto : class, new();
    }
}
