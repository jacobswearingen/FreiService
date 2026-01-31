namespace FreiService.Computus;

/// <summary>
/// Represents a fully resolved calendar day with primary observance and any commemorations.
/// </summary>
public class ResolvedDay
{
    /// <summary>
    /// The date being resolved.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The primary observance for this day (what is actually celebrated).
    /// </summary>
    public CalendarDay Primary { get; set; } = new CalendarDay();

    /// <summary>
    /// Any displaced observances that are commemorated but not celebrated.
    /// </summary>
    public List<CalendarDay> Commemorations { get; set; } = new List<CalendarDay>();

    /// <summary>
    /// The liturgical color for this day (taken from the primary observance).
    /// </summary>
    public LiturgicalColor LiturgicalColor { get; set; }

    /// <summary>
    /// The liturgical season (always from Temporal).
    /// </summary>
    public Season Season { get; set; }

    /// <summary>
    /// Optional notes about the resolution (e.g., "St. Andrew is commemorated").
    /// </summary>
    public string? Notes { get; set; }
}
