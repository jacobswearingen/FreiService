using FreiService.Computus;
using FreiService.Data.Models;
using FreiService.Data.Repositories;

namespace FreiService.Data.Services;

/// <summary>
/// Service for managing holy days with integration to Computus calculations.
/// </summary>
public interface IHolyDaysService
{
    /// <summary>
    /// Calculates holy days for a year (dry run - does not save to database).
    /// </summary>
    /// <param name="year">The year to calculate holy days for.</param>
    /// <returns>A dictionary of holy day names to dates.</returns>
    Dictionary<string, DateTime> CalculateHolyDaysForYear(int year);

    /// <summary>
    /// Calculates and saves holy days for a year to the database.
    /// </summary>
    /// <param name="year">The year to calculate and save holy days for.</param>
    /// <returns>The list of saved holy days.</returns>
    Task<List<HolyDay>> SaveHolyDaysForYearAsync(int year);

    /// <summary>
    /// Gets all holy days for a specific year from the database.
    /// </summary>
    /// <param name="year">The year to retrieve holy days for.</param>
    /// <returns>A list of holy days for the specified year.</returns>
    Task<List<HolyDay>> GetHolyDaysByYearAsync(int year);

    /// <summary>
    /// Gets all holy days in the database.
    /// </summary>
    /// <returns>A list of all holy days.</returns>
    Task<List<HolyDay>> GetAllHolyDaysAsync();

    /// <summary>
    /// Checks if holy days exist for a specific year.
    /// </summary>
    /// <param name="year">The year to check.</param>
    /// <returns>True if holy days exist for the year, false otherwise.</returns>
    Task<bool> HolyDaysExistForYearAsync(int year);

    /// <summary>
    /// Deletes all holy days for a specific year.
    /// </summary>
    /// <param name="year">The year to delete holy days for.</param>
    /// <returns>The number of holy days deleted.</returns>
    Task<int> DeleteHolyDaysByYearAsync(int year);

    /// <summary>
    /// Calculates and saves holy days for a year including custom holy day definitions.
    /// </summary>
    /// <param name="year">The year to calculate and save holy days for.</param>
    /// <param name="includeCustomDefinitions">Whether to include custom holy day definitions.</param>
    /// <returns>The list of saved holy days.</returns>
    Task<List<HolyDay>> SaveHolyDaysForYearWithCustomAsync(int year, bool includeCustomDefinitions = true);
}
