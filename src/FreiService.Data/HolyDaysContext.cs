using Microsoft.EntityFrameworkCore;
using FreiService.Data.Models;

namespace FreiService.Data;

/// <summary>
/// Database context for managing holy days.
/// </summary>
public class HolyDaysContext : DbContext
{
    /// <summary>
    /// Gets or sets the HolyDays DbSet.
    /// </summary>
    public DbSet<HolyDay> HolyDays { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the HolyDaysContext class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public HolyDaysContext(DbContextOptions<HolyDaysContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure HolyDay entity
        modelBuilder.Entity<HolyDay>(entity =>
        {
            // Create a unique index on Year + Name to prevent duplicates
            entity.HasIndex(e => new { e.Year, e.Name })
                  .IsUnique();

            // Create an index on Year for faster year-based queries
            entity.HasIndex(e => e.Year);

            // Create an index on Date for date-based queries
            entity.HasIndex(e => e.Date);
        });
    }
}
