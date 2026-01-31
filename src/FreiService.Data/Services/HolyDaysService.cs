using FreiService.Computus;
using FreiService.Data.Models;
using FreiService.Data.Repositories;

namespace FreiService.Data.Services;

/// <summary>
/// Service for managing holy days with integration to Computus calculations.
/// </summary>
public class HolyDaysService : IHolyDaysService
{
    private readonly IComputusService _computusService;
    private readonly IHolyDaysRepository _repository;

    /// <summary>
    /// Initializes a new instance of the HolyDaysService class.
    /// </summary>
    /// <param name="computusService">The Computus service for calculating dates.</param>
    /// <param name="repository">The repository for data access.</param>
    public HolyDaysService(IComputusService computusService, IHolyDaysRepository repository)
    {
        _computusService = computusService;
        _repository = repository;
    }

    /// <summary>
    /// Calculates holy days for a year (dry run - does not save to database).
    /// </summary>
    /// <param name="year">The year to calculate holy days for.</param>
    /// <returns>A dictionary of holy day names to dates.</returns>
    public Dictionary<string, DateTime> CalculateHolyDaysForYear(int year)
    {
        return _computusService.CalculateAllHolyDays(year);
    }

    /// <summary>
    /// Calculates and saves holy days for a year to the database.
    /// </summary>
    /// <param name="year">The year to calculate and save holy days for.</param>
    /// <returns>The list of saved holy days.</returns>
    public async Task<List<HolyDay>> SaveHolyDaysForYearAsync(int year)
    {
        // Calculate the holy days
        var calculatedHolyDays = _computusService.CalculateAllHolyDays(year);

        // Define which holy days are static vs sanctoral
        var staticHolyDays = new HashSet<string>
        {
            "Annunciation of Mary",
            "Christmas",
            "Epiphany",
            "Reformation Day",
            "All Saints' Day"
        };

        // Convert to database entities
        var holyDays = calculatedHolyDays.Select(kvp => new HolyDay
        {
            Name = kvp.Key,
            Year = year,
            Date = kvp.Value,
            Type = staticHolyDays.Contains(kvp.Key) ? HolyDayType.Static : HolyDayType.Sanctoral
        }).ToList();

        // Save to database
        await _repository.AddHolyDaysAsync(holyDays);

        return holyDays;
    }

    /// <summary>
    /// Gets all holy days for a specific year from the database.
    /// </summary>
    /// <param name="year">The year to retrieve holy days for.</param>
    /// <returns>A list of holy days for the specified year.</returns>
    public async Task<List<HolyDay>> GetHolyDaysByYearAsync(int year)
    {
        return await _repository.GetHolyDaysByYearAsync(year);
    }

    /// <summary>
    /// Gets all holy days in the database.
    /// </summary>
    /// <returns>A list of all holy days.</returns>
    public async Task<List<HolyDay>> GetAllHolyDaysAsync()
    {
        return await _repository.GetAllHolyDaysAsync();
    }

    /// <summary>
    /// Checks if holy days exist for a specific year.
    /// </summary>
    /// <param name="year">The year to check.</param>
    /// <returns>True if holy days exist for the year, false otherwise.</returns>
    public async Task<bool> HolyDaysExistForYearAsync(int year)
    {
        return await _repository.HolyDaysExistForYearAsync(year);
    }

    /// <summary>
    /// Deletes all holy days for a specific year.
    /// </summary>
    /// <param name="year">The year to delete holy days for.</param>
    /// <returns>The number of holy days deleted.</returns>
    public async Task<int> DeleteHolyDaysByYearAsync(int year)
    {
        return await _repository.DeleteHolyDaysByYearAsync(year);
    }
}
