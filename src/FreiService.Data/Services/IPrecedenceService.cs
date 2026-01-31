using FreiService.Computus;

namespace FreiService.Data.Services;

/// <summary>
/// Service interface for resolving calendar conflicts between Temporal and Sanctoral calendars.
/// </summary>
public interface IPrecedenceService
{
    /// <summary>
    /// Resolves the calendar for a specific date, determining what should be celebrated.
    /// </summary>
    /// <param name="date">The date to resolve.</param>
    /// <returns>A fully resolved calendar day with primary observance and commemorations.</returns>
    Task<ResolvedDay> ResolveDateAsync(DateTime date);

    /// <summary>
    /// Resolves the calendar for a range of dates.
    /// </summary>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <returns>List of resolved days for the range.</returns>
    Task<List<ResolvedDay>> ResolveDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets the next N feast days starting from a given date.
    /// </summary>
    /// <param name="fromDate">The date to start from.</param>
    /// <param name="count">The number of feast days to return.</param>
    /// <returns>List of resolved days for upcoming feasts.</returns>
    Task<List<ResolvedDay>> GetUpcomingFeastsAsync(DateTime fromDate, int count);
}
