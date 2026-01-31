namespace FreiService.Computus;

/// <summary>
/// Represents a day in the Temporale (moveable cycle based on Easter).
/// </summary>
public class TemporalDay
{
    /// <summary>
    /// The date of this temporal day.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The liturgical season this day falls in.
    /// </summary>
    public Season Season { get; set; }

    /// <summary>
    /// The week number within the season (e.g., Trinity 14 would have WeekOfSeason = 14).
    /// Null for days that aren't part of a numbered week.
    /// </summary>
    public int? WeekOfSeason { get; set; }

    /// <summary>
    /// The specific name of the day (e.g., "Laetare", "Trinity 14", "Easter Sunday").
    /// </summary>
    public string? DayName { get; set; }

    /// <summary>
    /// The type of day (Sunday, Weekday, Principal Feast, etc.).
    /// </summary>
    public DayType DayType { get; set; }

    /// <summary>
    /// The liturgical color for this day.
    /// </summary>
    public LiturgicalColor LiturgicalColor { get; set; }

    /// <summary>
    /// The rank of this observance for precedence determination.
    /// </summary>
    public Rank Rank { get; set; }
}
