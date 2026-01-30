namespace FreiService.Computus;

/// <summary>
/// Interface for computus service that calculates Easter dates and related holy days.
/// </summary>
public interface IComputusService
{
    /// <summary>
    /// Calculates the date of Easter Sunday for a given year.
    /// </summary>
    /// <param name="year">The year to calculate Easter for.</param>
    /// <returns>The date of Easter Sunday.</returns>
    DateTime CalculateEaster(int year);

    /// <summary>
    /// Calculates Easter dates for a range of years.
    /// </summary>
    /// <param name="startYear">The starting year (inclusive).</param>
    /// <param name="endYear">The ending year (inclusive).</param>
    /// <returns>A dictionary mapping years to their Easter dates.</returns>
    Dictionary<int, DateTime> CalculateEasterRange(int startYear, int endYear);

    /// <summary>
    /// Calculates the date of Ash Wednesday for a given year (46 days before Easter).
    /// </summary>
    /// <param name="year">The year to calculate Ash Wednesday for.</param>
    /// <returns>The date of Ash Wednesday.</returns>
    DateTime CalculateAshWednesday(int year);

    /// <summary>
    /// Calculates the date of Pentecost for a given year.
    /// Pentecost occurs 49 days after Easter Sunday, which is the 50th day when counting Easter as day 1.
    /// </summary>
    /// <param name="year">The year to calculate Pentecost for.</param>
    /// <returns>The date of Pentecost.</returns>
    DateTime CalculatePentecost(int year);

    /// <summary>
    /// Calculates the date of Trinity Sunday for a given year (56 days after Easter).
    /// </summary>
    /// <param name="year">The year to calculate Trinity Sunday for.</param>
    /// <returns>The date of Trinity Sunday.</returns>
    DateTime CalculateTrinity(int year);

    /// <summary>
    /// Calculates the date of the Annunciation of Mary for a given year (March 25, fixed).
    /// </summary>
    /// <param name="year">The year to calculate the Annunciation for.</param>
    /// <returns>The date of the Annunciation (March 25).</returns>
    DateTime CalculateAnnunciation(int year);

    /// <summary>
    /// Calculates all holy days for a given year.
    /// </summary>
    /// <param name="year">The year to calculate holy days for.</param>
    /// <returns>A dictionary mapping holy day names to their dates.</returns>
    Dictionary<string, DateTime> CalculateAllHolyDays(int year);
}
