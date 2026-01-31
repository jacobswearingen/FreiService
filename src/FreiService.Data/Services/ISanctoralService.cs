using FreiService.Data.Models;

namespace FreiService.Data.Services;

/// <summary>
/// Service interface for managing the Sanctorale (fixed commemorations).
/// </summary>
public interface ISanctoralService
{
    /// <summary>
    /// Gets all sanctoral days for a specific date.
    /// </summary>
    /// <param name="date">The date to look up.</param>
    /// <returns>List of sanctoral days for that date.</returns>
    Task<List<SanctoralDay>> GetSanctoralDaysAsync(DateTime date);

    /// <summary>
    /// Gets a sanctoral day by its ID.
    /// </summary>
    /// <param name="id">The sanctoral day ID.</param>
    /// <returns>The sanctoral day, or null if not found.</returns>
    Task<SanctoralDay?> GetSanctoralDayByIdAsync(Guid id);

    /// <summary>
    /// Lists all sanctoral days in the calendar.
    /// </summary>
    /// <returns>List of all sanctoral days.</returns>
    Task<List<SanctoralDay>> ListAllSanctoralDaysAsync();

    /// <summary>
    /// Lists only custom (user-added) sanctoral days.
    /// </summary>
    /// <returns>List of custom sanctoral days.</returns>
    Task<List<SanctoralDay>> ListCustomSanctoralDaysAsync();

    /// <summary>
    /// Adds a new sanctoral day to the calendar.
    /// </summary>
    /// <param name="sanctoralDay">The sanctoral day to add.</param>
    /// <returns>The added sanctoral day.</returns>
    Task<SanctoralDay> AddSanctoralDayAsync(SanctoralDay sanctoralDay);

    /// <summary>
    /// Updates an existing sanctoral day.
    /// </summary>
    /// <param name="id">The ID of the sanctoral day to update.</param>
    /// <param name="sanctoralDay">The updated sanctoral day data.</param>
    /// <returns>The updated sanctoral day.</returns>
    Task<SanctoralDay> UpdateSanctoralDayAsync(Guid id, SanctoralDay sanctoralDay);

    /// <summary>
    /// Deletes a sanctoral day from the calendar.
    /// </summary>
    /// <param name="id">The ID of the sanctoral day to delete.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteSanctoralDayAsync(Guid id);

    /// <summary>
    /// Initializes the database with the default TLH sanctoral calendar.
    /// </summary>
    /// <returns>Number of entries added.</returns>
    Task<int> InitializeDefaultSanctoralCalendarAsync();
}
