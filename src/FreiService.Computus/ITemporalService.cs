namespace FreiService.Computus;

/// <summary>
/// Interface for the Temporal Service that handles the moveable cycle of the church year.
/// </summary>
public interface ITemporalService : IComputusService
{
    /// <summary>
    /// Gets complete temporal day information for a specific date.
    /// </summary>
    /// <param name="date">The date to get information for.</param>
    /// <returns>A TemporalDay with full liturgical information.</returns>
    TemporalDay GetTemporalDay(DateTime date);

    /// <summary>
    /// Calculates all moveable feasts for a given year.
    /// </summary>
    /// <param name="year">The year to calculate feasts for.</param>
    /// <returns>Dictionary mapping feast names to their dates.</returns>
    Dictionary<string, DateTime> CalculateAllMoveableFeasts(int year);

    /// <summary>
    /// Calculates the date of a specific Sunday after Trinity.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="week">The week number (1-27).</param>
    /// <returns>The date of that Trinity Sunday.</returns>
    DateTime CalculateTrinityWeek(int year, int week);

    /// <summary>
    /// Calculates the number of Sundays after Trinity for a given year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns>The number of Sundays after Trinity (typically 23-27).</returns>
    int GetTrinitySundayCount(int year);

    /// <summary>
    /// Calculates the first Sunday of Advent for a given year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns>The date of Advent 1.</returns>
    DateTime CalculateAdvent1(int year);

    /// <summary>
    /// Calculates the number of Sundays after Epiphany for a given year.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <returns>The number of Sundays after Epiphany (1-6).</returns>
    int GetEpiphanySundayCount(int year);
}
