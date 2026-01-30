namespace FreiService.Computus;

/// <summary>
/// Service that calculates Easter dates using the Meeus/Jones/Butcher algorithm.
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
}
