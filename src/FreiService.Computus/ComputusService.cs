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
    /// Calculates the date of Pentecost for a given year.
    /// Pentecost occurs 49 days after Easter Sunday, which is the 50th day when counting Easter as day 1.
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
            { "Good Friday", CalculateGoodFriday(year) },
            { "Ascension Day", CalculateAscension(year) },
            { "Pentecost", CalculatePentecost(year) },
            { "Trinity Sunday", CalculateTrinity(year) },
            { "Annunciation of Mary", CalculateAnnunciation(year) },
            { "Christmas", CalculateChristmas(year) },
            { "Epiphany", CalculateEpiphany(year) },
            { "Reformation Day", CalculateReformationDay(year) },
            { "All Saints' Day", CalculateAllSaintsDay(year) }
        };
    }

    /// <summary>
    /// Calculates the date of Christmas for a given year (December 25, fixed).
    /// </summary>
    /// <param name="year">The year to calculate Christmas for.</param>
    /// <returns>The date of Christmas (December 25).</returns>
    public DateTime CalculateChristmas(int year)
    {
        if (year < 1583)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be 1583 or later for Gregorian calendar.");
        }
        return new DateTime(year, 12, 25);
    }

    /// <summary>
    /// Calculates the date of Epiphany for a given year (January 6, fixed).
    /// </summary>
    /// <param name="year">The year to calculate Epiphany for.</param>
    /// <returns>The date of Epiphany (January 6).</returns>
    public DateTime CalculateEpiphany(int year)
    {
        if (year < 1583)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be 1583 or later for Gregorian calendar.");
        }
        return new DateTime(year, 1, 6);
    }

    /// <summary>
    /// Calculates the date of Good Friday for a given year (2 days before Easter).
    /// </summary>
    /// <param name="year">The year to calculate Good Friday for.</param>
    /// <returns>The date of Good Friday.</returns>
    public DateTime CalculateGoodFriday(int year)
    {
        var easter = CalculateEaster(year);
        return easter.AddDays(-2);
    }

    /// <summary>
    /// Calculates the date of Ascension Day for a given year (39 days after Easter, always Thursday).
    /// </summary>
    /// <param name="year">The year to calculate Ascension Day for.</param>
    /// <returns>The date of Ascension Day.</returns>
    public DateTime CalculateAscension(int year)
    {
        var easter = CalculateEaster(year);
        return easter.AddDays(39);
    }

    /// <summary>
    /// Calculates the date of Reformation Day for a given year (October 31, fixed).
    /// </summary>
    /// <param name="year">The year to calculate Reformation Day for.</param>
    /// <returns>The date of Reformation Day (October 31).</returns>
    public DateTime CalculateReformationDay(int year)
    {
        if (year < 1583)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be 1583 or later for Gregorian calendar.");
        }
        return new DateTime(year, 10, 31);
    }

    /// <summary>
    /// Calculates the date of All Saints' Day for a given year (November 1, fixed).
    /// </summary>
    /// <param name="year">The year to calculate All Saints' Day for.</param>
    /// <returns>The date of All Saints' Day (November 1).</returns>
    public DateTime CalculateAllSaintsDay(int year)
    {
        if (year < 1583)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be 1583 or later for Gregorian calendar.");
        }
        return new DateTime(year, 11, 1);
    }

    /// <summary>
    /// Determines which liturgical season a given date falls within in the Confessional Lutheran church calendar.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="year">The liturgical year (typically the year containing the date, but may need adjustment for Advent).</param>
    /// <returns>The church season the date falls within.</returns>
    public ChurchSeason GetChurchSeason(DateTime date, int year)
    {
        // Get key dates for the current year
        var christmas = CalculateChristmas(year);
        var epiphany = CalculateEpiphany(year);
        var ashWednesday = CalculateAshWednesday(year);
        var easter = CalculateEaster(year);
        var pentecost = CalculatePentecost(year);

        // Calculate first Sunday of Advent (4 Sundays before Christmas)
        var adventStart = christmas;
        while (adventStart.DayOfWeek != DayOfWeek.Sunday || (christmas - adventStart).Days < 22)
        {
            adventStart = adventStart.AddDays(-1);
        }

        // Also need to check previous year's Christmas if date is in early January
        var previousChristmas = CalculateChristmas(year - 1);
        
        // Determine the season
        // Christmas season: Dec 25 - Jan 5
        if ((date >= christmas && date.Month == 12) || 
            (date.Month == 1 && date.Day <= 5))
        {
            return ChurchSeason.Christmas;
        }

        // Epiphany season: Jan 6 - day before Ash Wednesday
        if (date >= epiphany && date < ashWednesday)
        {
            return ChurchSeason.Epiphany;
        }

        // Lent season: Ash Wednesday - day before Easter
        if (date >= ashWednesday && date < easter)
        {
            return ChurchSeason.Lent;
        }

        // Easter season: Easter - day before Pentecost
        if (date >= easter && date < pentecost)
        {
            return ChurchSeason.Easter;
        }

        // Pentecost/Trinity season: Pentecost - day before Advent
        if (date >= pentecost && date < adventStart)
        {
            return ChurchSeason.Pentecost;
        }

        // Advent season: First Sunday of Advent - Dec 24
        if (date >= adventStart && date < christmas)
        {
            return ChurchSeason.Advent;
        }

        // If we haven't matched yet, it might be late year Pentecost season
        // or we need to check against next year's Advent
        var nextYearChristmas = CalculateChristmas(year + 1);
        var nextAdventStart = nextYearChristmas;
        while (nextAdventStart.DayOfWeek != DayOfWeek.Sunday || (nextYearChristmas - nextAdventStart).Days < 22)
        {
            nextAdventStart = nextAdventStart.AddDays(-1);
        }

        if (date >= nextAdventStart && date < nextYearChristmas)
        {
            return ChurchSeason.Advent;
        }

        // Default to Pentecost season for remaining dates
        return ChurchSeason.Pentecost;
    }
}
