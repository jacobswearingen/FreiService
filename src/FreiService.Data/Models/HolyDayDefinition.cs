using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreiService.Data.Models;

/// <summary>
/// Represents a definition for a holy day that can be used to calculate dates for any year.
/// </summary>
public class HolyDayDefinition
{
    /// <summary>
    /// Primary key for the holy day definition record.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Name of the holy day (e.g., "St. Lucy's Day", "Corpus Christi").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this holy day is static (fixed date) or part of the sanctoral calendar (calculated based on Easter).
    /// </summary>
    [Required]
    public HolyDayType Type { get; set; }

    /// <summary>
    /// For static holy days: the month (1-12). Null for sanctoral holy days.
    /// </summary>
    public int? Month { get; set; }

    /// <summary>
    /// For static holy days: the day of month (1-31). Null for sanctoral holy days.
    /// </summary>
    public int? Day { get; set; }

    /// <summary>
    /// For sanctoral holy days: the number of days offset from Easter (can be negative for days before Easter).
    /// Null for static holy days.
    /// </summary>
    public int? DaysFromEaster { get; set; }

    /// <summary>
    /// Optional description or notes about this holy day.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// When this record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
