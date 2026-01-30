using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreiService.Data.Models;

/// <summary>
/// Represents a holy day in the church calendar.
/// </summary>
public class HolyDay
{
    /// <summary>
    /// Primary key for the holy day record.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Name of the holy day (e.g., "Easter Sunday", "Ash Wednesday").
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The year this holy day occurs in.
    /// </summary>
    [Required]
    public int Year { get; set; }

    /// <summary>
    /// The date when this holy day occurs.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

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
