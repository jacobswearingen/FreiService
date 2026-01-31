namespace FreiService.Data.Models;

/// <summary>
/// Represents the type of holy day based on how its date is determined.
/// </summary>
public enum HolyDayType
{
    /// <summary>
    /// A holy day that falls on a fixed calendar date every year (e.g., Christmas on December 25).
    /// </summary>
    Static = 0,

    /// <summary>
    /// A holy day whose date is calculated based on Easter and the sanctoral calendar (e.g., Ash Wednesday, Pentecost).
    /// </summary>
    Sanctoral = 1
}
