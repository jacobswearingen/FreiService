using Microsoft.EntityFrameworkCore;
using FreiService.Data.Models;

namespace FreiService.Data.Repositories;

/// <summary>
/// Repository for managing holy day definitions in the database.
/// </summary>
public class HolyDayDefinitionsRepository : IHolyDayDefinitionsRepository
{
    private readonly HolyDaysContext _context;

    /// <summary>
    /// Initializes a new instance of the HolyDayDefinitionsRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public HolyDayDefinitionsRepository(HolyDaysContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all holy day definitions.
    /// </summary>
    /// <returns>A list of all holy day definitions.</returns>
    public async Task<List<HolyDayDefinition>> GetAllDefinitionsAsync()
    {
        return await _context.HolyDayDefinitions
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a holy day definition by its ID.
    /// </summary>
    /// <param name="id">The ID of the definition.</param>
    /// <returns>The holy day definition, or null if not found.</returns>
    public async Task<HolyDayDefinition?> GetDefinitionByIdAsync(int id)
    {
        return await _context.HolyDayDefinitions.FindAsync(id);
    }

    /// <summary>
    /// Gets a holy day definition by its name.
    /// </summary>
    /// <param name="name">The name of the holy day.</param>
    /// <returns>The holy day definition, or null if not found.</returns>
    public async Task<HolyDayDefinition?> GetDefinitionByNameAsync(string name)
    {
        return await _context.HolyDayDefinitions
            .FirstOrDefaultAsync(d => d.Name == name);
    }

    /// <summary>
    /// Adds a new holy day definition.
    /// </summary>
    /// <param name="definition">The holy day definition to add.</param>
    /// <returns>The added definition with its generated ID.</returns>
    public async Task<HolyDayDefinition> AddDefinitionAsync(HolyDayDefinition definition)
    {
        definition.CreatedAt = DateTime.UtcNow;
        definition.UpdatedAt = DateTime.UtcNow;

        _context.HolyDayDefinitions.Add(definition);
        await _context.SaveChangesAsync();

        return definition;
    }

    /// <summary>
    /// Updates an existing holy day definition.
    /// </summary>
    /// <param name="definition">The holy day definition to update.</param>
    /// <returns>The updated definition.</returns>
    public async Task<HolyDayDefinition> UpdateDefinitionAsync(HolyDayDefinition definition)
    {
        definition.UpdatedAt = DateTime.UtcNow;
        _context.HolyDayDefinitions.Update(definition);
        await _context.SaveChangesAsync();

        return definition;
    }

    /// <summary>
    /// Deletes a holy day definition by its ID.
    /// </summary>
    /// <param name="id">The ID of the definition to delete.</param>
    /// <returns>True if deleted, false if not found.</returns>
    public async Task<bool> DeleteDefinitionAsync(int id)
    {
        var definition = await _context.HolyDayDefinitions.FindAsync(id);
        if (definition == null)
        {
            return false;
        }

        _context.HolyDayDefinitions.Remove(definition);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Checks if a holy day definition with the given name exists.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if exists, false otherwise.</returns>
    public async Task<bool> DefinitionExistsAsync(string name)
    {
        return await _context.HolyDayDefinitions.AnyAsync(d => d.Name == name);
    }
}
