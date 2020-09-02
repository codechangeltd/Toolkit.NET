namespace CodeChange.Toolkit.Domain.Mapping
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a contract for a DTO to configuration mapper
    /// </summary>
    public interface IDtoToConfigurationMapper
    {
        /// <summary>
        /// Maps a DTO to a new configuration object
        /// </summary>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <typeparam name="TConfiguration">The configuration type</typeparam>
        /// <param name="dto">The DTO to map</param>
        /// <returns>The mapped configuration</returns>
        TConfiguration Map<TDto, TConfiguration>(TDto dto) 
            where TConfiguration : class, new();

        /// <summary>
        /// Maps a collection of DTOs to a collection of configurations
        /// </summary>
        /// <typeparam name="TDto">The DTO type</typeparam>
        /// <typeparam name="TConfiguration">The configuration type</typeparam>
        /// <param name="dtos">The DTOs to map</param>
        /// <returns>The mapped configuration</returns>
        IEnumerable<TConfiguration> Map<TDto, TConfiguration>(IEnumerable<TDto> dtos) 
            where TConfiguration : class, new();
    }
}
