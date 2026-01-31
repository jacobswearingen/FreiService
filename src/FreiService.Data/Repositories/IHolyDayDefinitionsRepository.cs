using FreiService.Data.Models;

namespace FreiService.Data.Repositories;

/// <summary>
/// Interface for managing holy day definitions in the database.
/// </summary>
public interface IHolyDayDefinitionsRepository
{
    /// <summary>
    /// Gets all holy day definitions.
    /// </summary>
    /// <returns>A list of all holy day definitions.</returns>
    Task<List<HolyDayDefinition>> GetAllDefinitionsAsync();

    /// <summary>
    /// Gets a holy day definition by its ID.
    /// </summary>
    /// <param name="id">The ID of the definition.</param>
    /// <returns>The holy day definition, or null if not found.</returns>
    Task<HolyDayDefinition?> GetDefinitionByIdAsync(int id);

    /// <summary>
    /// Gets a holy day definition by its name.
    /// </summary>
    /// <param name="name">The name of the holy day.</param>
    /// <returns>The holy day definition, or null if not found.</returns>
    Task<HolyDayDefinition?> GetDefinitionByNameAsync(string name);

    /// <summary>
    /// Adds a new holy day definition.
    /// </summary>
    /// <param name="definition">The holy day definition to add.</param>
    /// <returns>The added definition with its generated ID.</returns>
    Task<HolyDayDefinition> AddDefinitionAsync(HolyDayDefinition definition);

    /// <summary>
    /// Updates an existing holy day definition.
    /// </summary>
    /// <param name="definition">The holy day definition to update.</param>
    /// <returns>The updated definition.</returns>
    Task<HolyDayDefinition> UpdateDefinitionAsync(HolyDayDefinition definition);

    /// <summary>
    /// Deletes a holy day definition by its ID.
    /// </summary>
    /// <param name="id">The ID of the definition to delete.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteDefinitionAsync(int id);

    /// <summary>
    /// Checks if a holy day definition with the given name exists.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if exists, false otherwise.</returns>
    Task<bool> DefinitionExistsAsync(string name);
}
