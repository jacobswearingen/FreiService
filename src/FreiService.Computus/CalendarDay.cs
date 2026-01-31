namespace FreiService.Computus;

/// <summary>
/// Represents a generic calendar day that can come from either Temporal or Sanctoral sources.
/// </summary>
public class CalendarDay
{
    /// <summary>
    /// The name of this observance.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The source of this observance (Temporal or Sanctoral).
    /// </summary>
    public CalendarSource Source { get; set; }

    /// <summary>
    /// The rank of this observance.
    /// </summary>
    public Rank Rank { get; set; }

    /// <summary>
    /// The liturgical color for this observance.
    /// </summary>
    public LiturgicalColor LiturgicalColor { get; set; }

    /// <summary>
    /// Reference ID for the collect (optional).
    /// </summary>
    public string? CollectId { get; set; }

    /// <summary>
    /// Reference ID for the readings (optional).
    /// </summary>
    public string? ReadingsId { get; set; }
}

/// <summary>
/// Indicates whether a CalendarDay comes from Temporal or Sanctoral sources.
/// </summary>
public enum CalendarSource
{
    /// <summary>
    /// From the Temporale (moveable cycle).
    /// </summary>
    Temporal,

    /// <summary>
    /// From the Sanctorale (fixed commemorations).
    /// </summary>
    Sanctoral
}
