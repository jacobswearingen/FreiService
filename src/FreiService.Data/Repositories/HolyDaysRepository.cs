using Microsoft.EntityFrameworkCore;
using FreiService.Data.Models;

namespace FreiService.Data.Repositories;

/// <summary>
/// Repository for managing holy days in the database.
/// </summary>
public class HolyDaysRepository : IHolyDaysRepository
{
    private readonly HolyDaysContext _context;

    /// <summary>
    /// Initializes a new instance of the HolyDaysRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public HolyDaysRepository(HolyDaysContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all holy days for a specific year.
    /// </summary>
    /// <param name="year">The year to retrieve holy days for.</param>
    /// <returns>A list of holy days for the specified year.</returns>
    public async Task<List<HolyDay>> GetHolyDaysByYearAsync(int year)
    {
        return await _context.HolyDays
            .Where(h => h.Year == year)
            .OrderBy(h => h.Date)
            .ToListAsync();
    }

    /// <summary>
    /// Gets all holy days in the database.
    /// </summary>
    /// <returns>A list of all holy days.</returns>
    public async Task<List<HolyDay>> GetAllHolyDaysAsync()
    {
        return await _context.HolyDays
            .OrderBy(h => h.Year)
            .ThenBy(h => h.Date)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new holy day to the database.
    /// </summary>
    /// <param name="holyDay">The holy day to add.</param>
    /// <returns>The added holy day with its generated ID.</returns>
    public async Task<HolyDay> AddHolyDayAsync(HolyDay holyDay)
    {
        holyDay.CreatedAt = DateTime.UtcNow;
        holyDay.UpdatedAt = DateTime.UtcNow;

        _context.HolyDays.Add(holyDay);
        await _context.SaveChangesAsync();

        return holyDay;
    }

    /// <summary>
    /// Adds multiple holy days to the database in a single transaction.
    /// </summary>
    /// <param name="holyDays">The holy days to add.</param>
    /// <returns>The number of holy days added.</returns>
    public async Task<int> AddHolyDaysAsync(IEnumerable<HolyDay> holyDays)
    {
        var now = DateTime.UtcNow;
        foreach (var holyDay in holyDays)
        {
            holyDay.CreatedAt = now;
            holyDay.UpdatedAt = now;
        }

        await _context.HolyDays.AddRangeAsync(holyDays);
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if holy days exist for a specific year.
    /// </summary>
    /// <param name="year">The year to check.</param>
    /// <returns>True if holy days exist for the year, false otherwise.</returns>
    public async Task<bool> HolyDaysExistForYearAsync(int year)
    {
        return await _context.HolyDays.AnyAsync(h => h.Year == year);
    }

    /// <summary>
    /// Deletes all holy days for a specific year.
    /// </summary>
    /// <param name="year">The year to delete holy days for.</param>
    /// <returns>The number of holy days deleted.</returns>
    public async Task<int> DeleteHolyDaysByYearAsync(int year)
    {
        var holyDays = await _context.HolyDays
            .Where(h => h.Year == year)
            .ToListAsync();

        _context.HolyDays.RemoveRange(holyDays);
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing holy day.
    /// </summary>
    /// <param name="holyDay">The holy day to update.</param>
    /// <returns>The updated holy day.</returns>
    public async Task<HolyDay> UpdateHolyDayAsync(HolyDay holyDay)
    {
        holyDay.UpdatedAt = DateTime.UtcNow;
        _context.HolyDays.Update(holyDay);
        await _context.SaveChangesAsync();

        return holyDay;
    }
}
