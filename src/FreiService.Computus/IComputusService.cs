namespace FreiService.Computus;

/// <summary>
/// Interface for computus service that calculates Easter dates.
/// </summary>
public interface IComputusService
{
    /// <summary>
    /// Calculates the date of Easter Sunday for a given year.
    /// </summary>
    /// <param name="year">The year to calculate Easter for.</param>
    /// <returns>The date of Easter Sunday.</returns>
    DateTime CalculateEaster(int year);

    /// <summary>
    /// Calculates Easter dates for a range of years.
    /// </summary>
    /// <param name="startYear">The starting year (inclusive).</param>
    /// <param name="endYear">The ending year (inclusive).</param>
    /// <returns>A dictionary mapping years to their Easter dates.</returns>
    Dictionary<int, DateTime> CalculateEasterRange(int startYear, int endYear);
}
