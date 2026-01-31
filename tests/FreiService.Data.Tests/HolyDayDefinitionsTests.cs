using Microsoft.EntityFrameworkCore;
using FreiService.Computus;
using FreiService.Data;
using FreiService.Data.Models;
using FreiService.Data.Repositories;
using FreiService.Data.Services;

namespace FreiService.Data.Tests;

public class HolyDayDefinitionsTests : IDisposable
{
    private readonly HolyDaysContext _context;
    private readonly HolyDayDefinitionsRepository _repository;
    private readonly IComputusService _computusService;

    public HolyDayDefinitionsTests()
    {
        var options = new DbContextOptionsBuilder<HolyDaysContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new HolyDaysContext(options);
        _repository = new HolyDayDefinitionsRepository(_context);
        _computusService = new ComputusService();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task AddDefinitionAsync_StaticHolyDay_AddsSuccessfully()
    {
        // Arrange
        var definition = new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13,
            Description = "Feast of St. Lucy"
        };

        // Act
        var result = await _repository.AddDefinitionAsync(definition);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("St. Lucy's Day", result.Name);
        Assert.Equal(HolyDayType.Static, result.Type);
        Assert.Equal(12, result.Month);
        Assert.Equal(13, result.Day);
    }

    [Fact]
    public async Task AddDefinitionAsync_SanctoralHolyDay_AddsSuccessfully()
    {
        // Arrange
        var definition = new HolyDayDefinition
        {
            Name = "Corpus Christi",
            Type = HolyDayType.Sanctoral,
            DaysFromEaster = 60,
            Description = "Feast of the Body of Christ"
        };

        // Act
        var result = await _repository.AddDefinitionAsync(definition);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("Corpus Christi", result.Name);
        Assert.Equal(HolyDayType.Sanctoral, result.Type);
        Assert.Equal(60, result.DaysFromEaster);
    }

    [Fact]
    public async Task GetAllDefinitionsAsync_ReturnsAllDefinitions()
    {
        // Arrange
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13
        });
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "Corpus Christi",
            Type = HolyDayType.Sanctoral,
            DaysFromEaster = 60
        });

        // Act
        var definitions = await _repository.GetAllDefinitionsAsync();

        // Assert
        Assert.Equal(2, definitions.Count);
    }

    [Fact]
    public async Task GetDefinitionByNameAsync_ExistingDefinition_ReturnsDefinition()
    {
        // Arrange
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13
        });

        // Act
        var definition = await _repository.GetDefinitionByNameAsync("St. Lucy's Day");

        // Assert
        Assert.NotNull(definition);
        Assert.Equal("St. Lucy's Day", definition.Name);
    }

    [Fact]
    public async Task GetDefinitionByNameAsync_NonExistingDefinition_ReturnsNull()
    {
        // Act
        var definition = await _repository.GetDefinitionByNameAsync("Non-existent Day");

        // Assert
        Assert.Null(definition);
    }

    [Fact]
    public async Task UpdateDefinitionAsync_UpdatesSuccessfully()
    {
        // Arrange
        var definition = await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13
        });

        definition.Description = "Updated description";

        // Act
        var updated = await _repository.UpdateDefinitionAsync(definition);

        // Assert
        Assert.Equal("Updated description", updated.Description);
    }

    [Fact]
    public async Task DeleteDefinitionAsync_ExistingDefinition_DeletesSuccessfully()
    {
        // Arrange
        var definition = await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13
        });

        // Act
        var result = await _repository.DeleteDefinitionAsync(definition.Id);

        // Assert
        Assert.True(result);
        var definitions = await _repository.GetAllDefinitionsAsync();
        Assert.Empty(definitions);
    }

    [Fact]
    public async Task DeleteDefinitionAsync_NonExistingDefinition_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteDefinitionAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DefinitionExistsAsync_ExistingDefinition_ReturnsTrue()
    {
        // Arrange
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13
        });

        // Act
        var exists = await _repository.DefinitionExistsAsync("St. Lucy's Day");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task DefinitionExistsAsync_NonExistingDefinition_ReturnsFalse()
    {
        // Act
        var exists = await _repository.DefinitionExistsAsync("Non-existent Day");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task SaveHolyDaysForYearWithCustomAsync_IncludesCustomDefinitions()
    {
        // Arrange
        var holyDaysRepository = new HolyDaysRepository(_context);
        var service = new HolyDaysService(_computusService, holyDaysRepository, _repository);

        // Add custom definition
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13
        });

        // Act
        var holyDays = await service.SaveHolyDaysForYearWithCustomAsync(2024, includeCustomDefinitions: true);

        // Assert
        Assert.Contains(holyDays, hd => hd.Name == "St. Lucy's Day");
        var stLucyDay = holyDays.First(hd => hd.Name == "St. Lucy's Day");
        Assert.Equal(new DateTime(2024, 12, 13), stLucyDay.Date);
        Assert.Equal(HolyDayType.Static, stLucyDay.Type);
    }

    [Fact]
    public async Task SaveHolyDaysForYearWithCustomAsync_SanctoralDefinition_CalculatesCorrectly()
    {
        // Arrange
        var holyDaysRepository = new HolyDaysRepository(_context);
        var service = new HolyDaysService(_computusService, holyDaysRepository, _repository);

        // Add custom definition (Corpus Christi - 60 days after Easter)
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "Corpus Christi",
            Type = HolyDayType.Sanctoral,
            DaysFromEaster = 60
        });

        // Act
        var holyDays = await service.SaveHolyDaysForYearWithCustomAsync(2024, includeCustomDefinitions: true);

        // Assert
        Assert.Contains(holyDays, hd => hd.Name == "Corpus Christi");
        var corpusChristi = holyDays.First(hd => hd.Name == "Corpus Christi");
        
        // Easter 2024 is March 31, so Corpus Christi should be May 30
        Assert.Equal(new DateTime(2024, 5, 30), corpusChristi.Date);
        Assert.Equal(HolyDayType.Sanctoral, corpusChristi.Type);
    }

    [Fact]
    public async Task SaveHolyDaysForYearWithCustomAsync_WithoutCustomDefinitions_OnlyIncludesBuiltIn()
    {
        // Arrange
        var holyDaysRepository = new HolyDaysRepository(_context);
        var service = new HolyDaysService(_computusService, holyDaysRepository, _repository);

        // Add custom definition
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13
        });

        // Act
        var holyDays = await service.SaveHolyDaysForYearWithCustomAsync(2024, includeCustomDefinitions: false);

        // Assert
        Assert.DoesNotContain(holyDays, hd => hd.Name == "St. Lucy's Day");
        Assert.Equal(11, holyDays.Count); // Only built-in holy days
    }

    [Fact]
    public async Task SaveHolyDaysForYearWithCustomAsync_MultipleCustomDefinitions_IncludesAll()
    {
        // Arrange
        var holyDaysRepository = new HolyDaysRepository(_context);
        var service = new HolyDaysService(_computusService, holyDaysRepository, _repository);

        // Add multiple custom definitions
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Lucy's Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 13
        });
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "St. Nicholas Day",
            Type = HolyDayType.Static,
            Month = 12,
            Day = 6
        });
        await _repository.AddDefinitionAsync(new HolyDayDefinition
        {
            Name = "Corpus Christi",
            Type = HolyDayType.Sanctoral,
            DaysFromEaster = 60
        });

        // Act
        var holyDays = await service.SaveHolyDaysForYearWithCustomAsync(2024, includeCustomDefinitions: true);

        // Assert
        Assert.Contains(holyDays, hd => hd.Name == "St. Lucy's Day");
        Assert.Contains(holyDays, hd => hd.Name == "St. Nicholas Day");
        Assert.Contains(holyDays, hd => hd.Name == "Corpus Christi");
        Assert.Equal(14, holyDays.Count); // 11 built-in + 3 custom
    }
}
