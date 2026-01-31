using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FreiService.Computus;

namespace FreiService.Data.Models;

/// <summary>
/// Represents a fixed commemoration in the Sanctorale (saints' days, feasts tied to calendar dates).
/// </summary>
public class SanctoralDay
{
    /// <summary>
    /// Primary key for the sanctoral day record.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// The month (1-12) when this commemoration occurs.
    /// </summary>
    [Required]
    public int Month { get; set; }

    /// <summary>
    /// The day of month (1-31) when this commemoration occurs.
    /// </summary>
    [Required]
    public int Day { get; set; }

    /// <summary>
    /// Name of the commemoration (e.g., "St. Andrew, Apostle", "Reformation Day").
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The rank of this observance (determines precedence).
    /// </summary>
    [Required]
    public Rank Rank { get; set; }

    /// <summary>
    /// The liturgical color for this day.
    /// </summary>
    [Required]
    public LiturgicalColor LiturgicalColor { get; set; }

    /// <summary>
    /// Indicates if this is a moveable date (e.g., Thanksgiving - 4th Thursday of November).
    /// </summary>
    public bool IsMoveable { get; set; }

    /// <summary>
    /// Rule for calculating moveable dates (e.g., "fourth Thursday of November").
    /// </summary>
    [MaxLength(200)]
    public string? MoveableRule { get; set; }

    /// <summary>
    /// Reference ID for propers/lectionary.
    /// </summary>
    [MaxLength(100)]
    public string? ProperId { get; set; }

    /// <summary>
    /// Indicates if this is a user-added custom entry.
    /// </summary>
    public bool IsCustom { get; set; }

    /// <summary>
    /// Optional notes or description about this day.
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// When this record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
