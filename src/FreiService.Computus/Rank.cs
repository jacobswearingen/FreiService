namespace FreiService.Computus;

/// <summary>
/// Represents the rank hierarchy for liturgical observances in TLH tradition.
/// Higher values indicate higher precedence.
/// </summary>
public enum Rank
{
    /// <summary>
    /// Minor observances that do not displace the temporal day.
    /// </summary>
    Commemoration = 1,

    /// <summary>
    /// Other saints, martyrs, commemorations.
    /// </summary>
    LesserFeast = 2,

    /// <summary>
    /// Days of the Twelve Apostles, St. Paul, and the four Evangelists.
    /// </summary>
    Apostle = 3,

    /// <summary>
    /// Evangelist rank (same as Apostle but separate for categorization).
    /// </summary>
    Evangelist = 3,

    /// <summary>
    /// Major celebrations (Reformation, All Saints, Candlemas, etc.).
    /// </summary>
    Feast = 4,

    /// <summary>
    /// Christmas, Easter, Epiphany, Ascension, Pentecost, Trinity.
    /// </summary>
    PrincipalFeast = 5
}
