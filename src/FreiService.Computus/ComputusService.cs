namespace FreiService.Computus;

/// <summary>
/// Service that calculates Easter dates and related holy days using the Meeus/Jones/Butcher algorithm.
/// This algorithm is accurate for all Gregorian calendar years (1583 and later).
/// </summary>
public class ComputusService : IComputusService
{
    /// <summary>
    /// Calculates the date of Easter Sunday for a given year using the Meeus/Jones/Butcher algorithm.
    /// </summary>
    /// <param name="year">The year to calculate Easter for (must be 1583 or later for Gregorian calendar).</param>
    /// <returns>The date of Easter Sunday.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when year is before 1583.</exception>
    public DateTime CalculateEaster(int year)
    {
        if (year < 1583)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be 1583 or later for Gregorian calendar.");
        }

        // Meeus/Jones/Butcher algorithm for Gregorian calendar
        int a = year % 19;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;

        return new DateTime(year, month, day);
    }

    /// <summary>
    /// Calculates Easter dates for a range of years.
    /// </summary>
    /// <param name="startYear">The starting year (inclusive).</param>
    /// <param name="endYear">The ending year (inclusive).</param>
    /// <returns>A dictionary mapping years to their Easter dates.</returns>
    /// <exception cref="ArgumentException">Thrown when startYear is greater than endYear.</exception>
    public Dictionary<int, DateTime> CalculateEasterRange(int startYear, int endYear)
    {
        if (startYear > endYear)
        {
            throw new ArgumentException("Start year must be less than or equal to end year.", nameof(startYear));
        }

        var results = new Dictionary<int, DateTime>();
        for (int year = startYear; year <= endYear; year++)
        {
            results[year] = CalculateEaster(year);
        }

        return results;
    }

    /// <summary>
    /// Calculates the date of Ash Wednesday for a given year (46 days before Easter).
    /// </summary>
    /// <param name="year">The year to calculate Ash Wednesday for.</param>
    /// <returns>The date of Ash Wednesday.</returns>
    public DateTime CalculateAshWednesday(int year)
    {
        var easter = CalculateEaster(year);
        return easter.AddDays(-46);
    }

    /// <summary>
    /// Calculates the date of Pentecost for a given year (50 days after Easter).
    /// </summary>
    /// <param name="year">The year to calculate Pentecost for.</param>
    /// <returns>The date of Pentecost.</returns>
    public DateTime CalculatePentecost(int year)
    {
        var easter = CalculateEaster(year);
        return easter.AddDays(49); // 49 days after Easter = 50th day (inclusive of Easter)
    }

    /// <summary>
    /// Calculates the date of Trinity Sunday for a given year (56 days after Easter).
    /// </summary>
    /// <param name="year">The year to calculate Trinity Sunday for.</param>
    /// <returns>The date of Trinity Sunday.</returns>
    public DateTime CalculateTrinity(int year)
    {
        var easter = CalculateEaster(year);
        return easter.AddDays(56);
    }

    /// <summary>
    /// Calculates the date of the Annunciation of Mary for a given year (March 25, fixed).
    /// </summary>
    /// <param name="year">The year to calculate the Annunciation for.</param>
    /// <returns>The date of the Annunciation (March 25).</returns>
    public DateTime CalculateAnnunciation(int year)
    {
        if (year < 1583)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be 1583 or later for Gregorian calendar.");
        }
        return new DateTime(year, 3, 25);
    }

    /// <summary>
    /// Calculates all holy days for a given year.
    /// </summary>
    /// <param name="year">The year to calculate holy days for.</param>
    /// <returns>A dictionary mapping holy day names to their dates.</returns>
    public Dictionary<string, DateTime> CalculateAllHolyDays(int year)
    {
        return new Dictionary<string, DateTime>
        {
            { "Easter Sunday", CalculateEaster(year) },
            { "Ash Wednesday", CalculateAshWednesday(year) },
            { "Pentecost", CalculatePentecost(year) },
            { "Trinity Sunday", CalculateTrinity(year) },
            { "Annunciation of Mary", CalculateAnnunciation(year) }
        };
    }
}
