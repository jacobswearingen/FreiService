namespace FreiService.Computus;

/// <summary>
/// Service for calculating and managing the Temporale (moveable cycle) of the church year
/// according to The Lutheran Hymnal (TLH) tradition.
/// </summary>
public class TemporalService : ComputusService, ITemporalService
{
    /// <summary>
    /// Gets complete temporal day information for a specific date.
    /// </summary>
    public TemporalDay GetTemporalDay(DateTime date)
    {
        var year = date.Year;
        
        // Determine which liturgical year we're in (Advent shifts the year)
        var advent1 = CalculateAdvent1(year);
        if (date < advent1)
        {
            // Check if we're in previous year's Advent
            var prevAdvent1 = CalculateAdvent1(year - 1);
            if (date >= prevAdvent1)
            {
                year = year - 1;
            }
        }

        var season = DetermineSeason(date, year);
        var dayName = DetermineDayName(date, year, season);
        var dayType = DetermineDayType(date, dayName);
        var color = DetermineLiturgicalColor(date, year, season, dayName);
        var rank = DetermineRank(dayName, dayType);
        var weekOfSeason = DetermineWeekOfSeason(date, year, season);

        return new TemporalDay
        {
            Date = date,
            Season = season,
            WeekOfSeason = weekOfSeason,
            DayName = dayName,
            DayType = dayType,
            LiturgicalColor = color,
            Rank = rank
        };
    }

    /// <summary>
    /// Calculates all moveable feasts for a given year according to TLH.
    /// </summary>
    public Dictionary<string, DateTime> CalculateAllMoveableFeasts(int year)
    {
        var easter = CalculateEaster(year);
        var feasts = new Dictionary<string, DateTime>();

        // Pre-Lent
        feasts["Septuagesima Sunday"] = easter.AddDays(-63);
        feasts["Sexagesima Sunday"] = easter.AddDays(-56);
        feasts["Quinquagesima Sunday"] = easter.AddDays(-49);

        // Lent
        feasts["Ash Wednesday"] = easter.AddDays(-46);
        feasts["Invocabit (Lent 1)"] = easter.AddDays(-42);
        feasts["Reminiscere (Lent 2)"] = easter.AddDays(-35);
        feasts["Oculi (Lent 3)"] = easter.AddDays(-28);
        feasts["Laetare (Lent 4)"] = easter.AddDays(-21);
        feasts["Judica (Lent 5)"] = easter.AddDays(-14);

        // Holy Week
        feasts["Palmarum (Palm Sunday)"] = easter.AddDays(-7);
        feasts["Maundy Thursday"] = easter.AddDays(-3);
        feasts["Good Friday"] = easter.AddDays(-2);
        feasts["Holy Saturday"] = easter.AddDays(-1);

        // Easter Season
        feasts["Easter Sunday"] = easter;
        feasts["Quasimodogeniti (Easter 1)"] = easter.AddDays(7);
        feasts["Misericordias Domini (Easter 2)"] = easter.AddDays(14);
        feasts["Jubilate (Easter 3)"] = easter.AddDays(21);
        feasts["Cantate (Easter 4)"] = easter.AddDays(28);
        feasts["Rogate (Easter 5)"] = easter.AddDays(35);
        feasts["Ascension"] = easter.AddDays(39);
        feasts["Exaudi (Easter 6)"] = easter.AddDays(42);

        // Pentecost
        feasts["Pentecost (Whitsunday)"] = easter.AddDays(49);
        feasts["Whit-Monday"] = easter.AddDays(50);
        feasts["Whit-Tuesday"] = easter.AddDays(51);
        feasts["Trinity Sunday"] = easter.AddDays(56);

        return feasts;
    }

    /// <summary>
    /// Calculates the date of a specific Sunday after Trinity.
    /// </summary>
    public DateTime CalculateTrinityWeek(int year, int week)
    {
        if (week < 1 || week > 27)
        {
            throw new ArgumentOutOfRangeException(nameof(week), "Trinity week must be between 1 and 27.");
        }

        var trinity = CalculateTrinity(year);
        return trinity.AddDays(7 * week);
    }

    /// <summary>
    /// Calculates the number of Sundays after Trinity for a given year.
    /// </summary>
    public int GetTrinitySundayCount(int year)
    {
        var trinity = CalculateTrinity(year);
        var advent1 = CalculateAdvent1(year);
        
        var days = (advent1 - trinity).Days;
        return days / 7;
    }

    /// <summary>
    /// Calculates the first Sunday of Advent (4th Sunday before Christmas).
    /// </summary>
    public DateTime CalculateAdvent1(int year)
    {
        var christmas = new DateTime(year, 12, 25);
        
        // Find the 4th Sunday before Christmas
        // This is the Sunday between Nov 27 and Dec 3
        var sunday = christmas;
        while (sunday.DayOfWeek != DayOfWeek.Sunday)
        {
            sunday = sunday.AddDays(-1);
        }
        
        // Now we're on the Sunday on or after Christmas, go back 4 weeks
        return sunday.AddDays(-28);
    }

    /// <summary>
    /// Calculates the number of Sundays after Epiphany for a given year.
    /// </summary>
    public int GetEpiphanySundayCount(int year)
    {
        var epiphany = CalculateEpiphany(year);
        var septuagesima = CalculateEaster(year).AddDays(-63);
        
        // Find the first Sunday after Epiphany
        var firstSunday = epiphany.AddDays(1);
        while (firstSunday.DayOfWeek != DayOfWeek.Sunday)
        {
            firstSunday = firstSunday.AddDays(1);
        }
        
        // Count Sundays until Septuagesima
        int count = 0;
        var current = firstSunday;
        while (current < septuagesima)
        {
            count++;
            current = current.AddDays(7);
        }
        
        return count;
    }

    private Season DetermineSeason(DateTime date, int year)
    {
        var easter = CalculateEaster(year);
        var christmas = new DateTime(year, 12, 25);
        var epiphany = CalculateEpiphany(year);
        var septuagesima = easter.AddDays(-63);
        var ashWednesday = easter.AddDays(-46);
        var palmSunday = easter.AddDays(-7);
        var pentecost = easter.AddDays(49);
        var trinity = easter.AddDays(56);
        var advent1 = CalculateAdvent1(year);

        // Check previous year's Christmas if we're in early January
        if (date.Month == 1 && date.Day <= 5)
        {
            var prevChristmas = new DateTime(year - 1, 12, 25);
            if (date >= prevChristmas)
            {
                return Season.Christmas;
            }
        }

        // Check next year's Advent if we're late in the year
        if (date.Month >= 11)
        {
            var nextAdvent1 = CalculateAdvent1(year + 1);
            if (date >= nextAdvent1)
            {
                return Season.Advent;
            }
        }

        if (date >= advent1 && date < christmas)
            return Season.Advent;
        
        if (date >= christmas && date.Month == 12)
            return Season.Christmas;
        
        if (date >= epiphany && date < septuagesima)
            return Season.Epiphany;
        
        if (date >= septuagesima && date < ashWednesday)
            return Season.Septuagesima;
        
        if (date >= ashWednesday && date < palmSunday)
            return Season.Lent;
        
        if (date >= palmSunday && date < easter)
            return Season.HolyWeek;
        
        if (date >= easter && date < pentecost)
            return Season.Easter;
        
        if (date >= pentecost)
            return Season.Trinity;

        return Season.Trinity; // Default
    }

    private string? DetermineDayName(DateTime date, int year, Season season)
    {
        var easter = CalculateEaster(year);
        var daysDiff = (date - easter).Days;

        // Check specific named days
        var moveableFeasts = CalculateAllMoveableFeasts(year);
        foreach (var feast in moveableFeasts)
        {
            if (feast.Value.Date == date.Date)
            {
                return feast.Key;
            }
        }

        // Check for numbered Sundays
        if (date.DayOfWeek == DayOfWeek.Sunday)
        {
            switch (season)
            {
                case Season.Advent:
                    var advent1 = CalculateAdvent1(year);
                    var adventWeek = ((date - advent1).Days / 7) + 1;
                    if (adventWeek >= 1 && adventWeek <= 4)
                        return $"Advent {adventWeek}";
                    break;

                case Season.Christmas:
                    var christmas = new DateTime(year, 12, 25);
                    if (date > christmas && date.Month == 12)
                        return "Christmas 1";
                    if (date.Month == 1 && date.Day >= 2 && date.Day <= 5)
                        return "Christmas 2";
                    break;

                case Season.Epiphany:
                    var epiphany = CalculateEpiphany(year);
                    if (date > epiphany)
                    {
                        var epiphanyWeeks = ((date - epiphany).Days / 7);
                        if (epiphanyWeeks >= 1 && epiphanyWeeks <= 6)
                        {
                            // Check if this is the last Sunday after Epiphany (Transfiguration)
                            var septuagesima = easter.AddDays(-63);
                            if (date.AddDays(7) >= septuagesima)
                                return "Transfiguration (Last Sunday after Epiphany)";
                            
                            return $"Epiphany {epiphanyWeeks}";
                        }
                    }
                    break;

                case Season.Trinity:
                    var trinity = CalculateTrinity(year);
                    if (date > trinity)
                    {
                        var trinityWeeks = ((date - trinity).Days / 7);
                        if (trinityWeeks >= 1 && trinityWeeks <= 27)
                            return $"Trinity {trinityWeeks}";
                    }
                    break;
            }
        }

        // Check fixed feasts
        if (date.Month == 12 && date.Day == 25)
            return "Christmas Day";
        if (date.Month == 1 && date.Day == 1)
            return "Circumcision and Name of Jesus";
        if (date.Month == 1 && date.Day == 6)
            return "Epiphany";

        return null;
    }

    private DayType DetermineDayType(DateTime date, string? dayName)
    {
        if (dayName != null)
        {
            var lowerName = dayName.ToLower();
            if (lowerName.Contains("easter") || lowerName.Contains("christmas") || 
                lowerName.Contains("epiphany") || lowerName.Contains("ascension") ||
                lowerName.Contains("pentecost") || lowerName.Contains("trinity sunday"))
            {
                return DayType.PrincipalFeast;
            }

            if (lowerName.Contains("sunday") || lowerName.Contains("advent") ||
                lowerName.Contains("lent") || lowerName.Contains("transfiguration"))
            {
                return DayType.Sunday;
            }

            return DayType.LesserFeast;
        }

        return date.DayOfWeek == DayOfWeek.Sunday ? DayType.Sunday : DayType.Weekday;
    }

    private LiturgicalColor DetermineLiturgicalColor(DateTime date, int year, Season season, string? dayName)
    {
        // Specific day overrides
        if (dayName != null)
        {
            var lowerName = dayName.ToLower();
            
            if (lowerName.Contains("good friday"))
                return LiturgicalColor.Black;
            
            if (lowerName.Contains("pentecost") && !lowerName.Contains("after"))
                return LiturgicalColor.Red;
            
            if (lowerName.Contains("reformation"))
                return LiturgicalColor.Red;
        }

        // Season-based colors
        switch (season)
        {
            case Season.Advent:
            case Season.Lent:
            case Season.Septuagesima:
                return LiturgicalColor.Violet;

            case Season.Christmas:
            case Season.Easter:
                return LiturgicalColor.White;

            case Season.HolyWeek:
                // Most of Holy Week is violet, except specific days
                if (dayName?.ToLower().Contains("good friday") == true)
                    return LiturgicalColor.Black;
                return LiturgicalColor.Violet;

            case Season.Epiphany:
                // Epiphany Day itself is white, after that green
                if (date.Month == 1 && date.Day == 6)
                    return LiturgicalColor.White;
                return LiturgicalColor.Green;

            case Season.Trinity:
                // Trinity Sunday itself is white, rest is green
                var trinity = CalculateTrinity(year);
                if (date.Date == trinity.Date)
                    return LiturgicalColor.White;
                return LiturgicalColor.Green;

            default:
                return LiturgicalColor.Green;
        }
    }

    private Rank DetermineRank(string? dayName, DayType dayType)
    {
        if (dayName == null)
        {
            return dayType == DayType.Sunday ? Rank.LesserFeast : Rank.Commemoration;
        }

        var lowerName = dayName.ToLower();

        // Principal Feasts
        if (lowerName.Contains("easter sunday") || lowerName.Contains("christmas day") ||
            lowerName == "epiphany" || lowerName.Contains("ascension") ||
            (lowerName.Contains("pentecost") && !lowerName.Contains("after")) ||
            lowerName.Contains("trinity sunday"))
        {
            return Rank.PrincipalFeast;
        }

        // Sundays are generally Feast rank
        if (dayType == DayType.Sunday)
        {
            return Rank.Feast;
        }

        // Named days in Lent are Feast rank
        if (lowerName.Contains("invocabit") || lowerName.Contains("reminiscere") ||
            lowerName.Contains("oculi") || lowerName.Contains("laetare") ||
            lowerName.Contains("judica") || lowerName.Contains("palmarum"))
        {
            return Rank.Feast;
        }

        // Holy Week special days
        if (lowerName.Contains("maundy thursday") || lowerName.Contains("good friday") ||
            lowerName.Contains("holy saturday"))
        {
            return Rank.Feast;
        }

        return Rank.LesserFeast;
    }

    private int? DetermineWeekOfSeason(DateTime date, int year, Season season)
    {
        if (date.DayOfWeek != DayOfWeek.Sunday)
            return null;

        switch (season)
        {
            case Season.Advent:
                var advent1 = CalculateAdvent1(year);
                return ((date - advent1).Days / 7) + 1;

            case Season.Epiphany:
                var epiphany = CalculateEpiphany(year);
                if (date > epiphany)
                    return (date - epiphany).Days / 7;
                break;

            case Season.Trinity:
                var trinity = CalculateTrinity(year);
                if (date > trinity)
                    return (date - trinity).Days / 7;
                break;
        }

        return null;
    }
}
