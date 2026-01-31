namespace FreiService.Computus;

/// <summary>
/// Represents the liturgical seasons in The Lutheran Hymnal tradition.
/// Extends the basic ChurchSeason enum with more detailed season information.
/// </summary>
public enum Season
{
    /// <summary>
    /// The season of Advent (four Sundays before Christmas).
    /// </summary>
    Advent,

    /// <summary>
    /// The Christmas season (December 25 through January 5).
    /// </summary>
    Christmas,

    /// <summary>
    /// The season of Epiphany (January 6 through day before Septuagesima or Ash Wednesday).
    /// </summary>
    Epiphany,

    /// <summary>
    /// Septuagesima season (pre-Lent, three Sundays before Ash Wednesday).
    /// </summary>
    Septuagesima,

    /// <summary>
    /// The season of Lent (Ash Wednesday through Saturday before Palm Sunday).
    /// </summary>
    Lent,

    /// <summary>
    /// Holy Week (Palm Sunday through Holy Saturday).
    /// </summary>
    HolyWeek,

    /// <summary>
    /// The Easter season (Easter Sunday through day before Pentecost).
    /// </summary>
    Easter,

    /// <summary>
    /// The season after Pentecost/Trinity (Pentecost through day before Advent).
    /// </summary>
    Trinity
}
