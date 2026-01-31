using FreiService.Data.Models;

namespace FreiService.Data.Repositories;

/// <summary>
/// Repository interface for managing SanctoralDay entities.
/// </summary>
public interface ISanctoralDaysRepository
{
    /// <summary>
    /// Gets all sanctoral days for a specific date.
    /// </summary>
    /// <param name="month">The month (1-12).</param>
    /// <param name="day">The day (1-31).</param>
    /// <returns>List of sanctoral days for that date.</returns>
    Task<List<SanctoralDay>> GetByDateAsync(int month, int day);

    /// <summary>
    /// Gets a sanctoral day by its ID.
    /// </summary>
    /// <param name="id">The sanctoral day ID.</param>
    /// <returns>The sanctoral day, or null if not found.</returns>
    Task<SanctoralDay?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all sanctoral days.
    /// </summary>
    /// <returns>List of all sanctoral days.</returns>
    Task<List<SanctoralDay>> GetAllAsync();

    /// <summary>
    /// Gets all custom (user-added) sanctoral days.
    /// </summary>
    /// <returns>List of custom sanctoral days.</returns>
    Task<List<SanctoralDay>> GetCustomAsync();

    /// <summary>
    /// Adds a new sanctoral day.
    /// </summary>
    /// <param name="sanctoralDay">The sanctoral day to add.</param>
    /// <returns>The added sanctoral day with generated ID.</returns>
    Task<SanctoralDay> AddAsync(SanctoralDay sanctoralDay);

    /// <summary>
    /// Updates an existing sanctoral day.
    /// </summary>
    /// <param name="sanctoralDay">The sanctoral day to update.</param>
    /// <returns>The updated sanctoral day.</returns>
    Task<SanctoralDay> UpdateAsync(SanctoralDay sanctoralDay);

    /// <summary>
    /// Deletes a sanctoral day by ID.
    /// </summary>
    /// <param name="id">The ID of the sanctoral day to delete.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a sanctoral day exists for a specific date and name.
    /// </summary>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    /// <param name="name">The name of the observance.</param>
    /// <returns>True if exists, false otherwise.</returns>
    Task<bool> ExistsAsync(int month, int day, string name);
}
