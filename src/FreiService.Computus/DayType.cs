namespace FreiService.Computus;

/// <summary>
/// Represents the type of day in the liturgical calendar.
/// </summary>
public enum DayType
{
    /// <summary>
    /// A regular weekday.
    /// </summary>
    Weekday,

    /// <summary>
    /// A Sunday.
    /// </summary>
    Sunday,

    /// <summary>
    /// A principal feast day (Christmas, Easter, etc.).
    /// </summary>
    PrincipalFeast,

    /// <summary>
    /// A lesser feast or saint's day.
    /// </summary>
    LesserFeast,

    /// <summary>
    /// A vigil day (day before a major feast).
    /// </summary>
    Vigil
}
