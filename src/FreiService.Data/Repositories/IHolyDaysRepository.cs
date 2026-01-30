using FreiService.Data.Models;

namespace FreiService.Data.Repositories;

/// <summary>
/// Interface for holy days repository operations.
/// </summary>
public interface IHolyDaysRepository
{
    /// <summary>
    /// Gets all holy days for a specific year.
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
    /// Adds a new holy day to the database.
    /// </summary>
    /// <param name="holyDay">The holy day to add.</param>
    /// <returns>The added holy day with its generated ID.</returns>
    Task<HolyDay> AddHolyDayAsync(HolyDay holyDay);

    /// <summary>
    /// Adds multiple holy days to the database in a single transaction.
    /// </summary>
    /// <param name="holyDays">The holy days to add.</param>
    /// <returns>The number of holy days added.</returns>
    Task<int> AddHolyDaysAsync(IEnumerable<HolyDay> holyDays);

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
    /// Updates an existing holy day.
    /// </summary>
    /// <param name="holyDay">The holy day to update.</param>
    /// <returns>The updated holy day.</returns>
    Task<HolyDay> UpdateHolyDayAsync(HolyDay holyDay);
}
