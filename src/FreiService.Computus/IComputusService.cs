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

    /// <summary>
    /// Calculates the date of Christmas for a given year (December 25, fixed).
    /// </summary>
    /// <param name="year">The year to calculate Christmas for.</param>
    /// <returns>The date of Christmas (December 25).</returns>
    DateTime CalculateChristmas(int year);

    /// <summary>
    /// Calculates the date of Epiphany for a given year (January 6, fixed).
    /// </summary>
    /// <param name="year">The year to calculate Epiphany for.</param>
    /// <returns>The date of Epiphany (January 6).</returns>
    DateTime CalculateEpiphany(int year);

    /// <summary>
    /// Calculates the date of Good Friday for a given year (2 days before Easter).
    /// </summary>
    /// <param name="year">The year to calculate Good Friday for.</param>
    /// <returns>The date of Good Friday.</returns>
    DateTime CalculateGoodFriday(int year);

    /// <summary>
    /// Calculates the date of Ascension Day for a given year (39 days after Easter, always Thursday).
    /// </summary>
    /// <param name="year">The year to calculate Ascension Day for.</param>
    /// <returns>The date of Ascension Day.</returns>
    DateTime CalculateAscension(int year);

    /// <summary>
    /// Calculates the date of Reformation Day for a given year (October 31, fixed).
    /// </summary>
    /// <param name="year">The year to calculate Reformation Day for.</param>
    /// <returns>The date of Reformation Day (October 31).</returns>
    DateTime CalculateReformationDay(int year);

    /// <summary>
    /// Calculates the date of All Saints' Day for a given year (November 1, fixed).
    /// </summary>
    /// <param name="year">The year to calculate All Saints' Day for.</param>
    /// <returns>The date of All Saints' Day (November 1).</returns>
    DateTime CalculateAllSaintsDay(int year);

    /// <summary>
    /// Determines which liturgical season a given date falls within in the Confessional Lutheran church calendar.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="year">The liturgical year (typically the year containing the date, but may need adjustment for Advent).</param>
    /// <returns>The church season the date falls within.</returns>
    ChurchSeason GetChurchSeason(DateTime date, int year);
}
