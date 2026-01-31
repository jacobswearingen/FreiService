using FreiService.Computus;
using FreiService.Data.Models;
using FreiService.Data.Repositories;
using FreiService.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace FreiService.Data.Tests;

public class SanctoralServiceTests
{
    private HolyDaysContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<HolyDaysContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new HolyDaysContext(options);
    }

    [Fact]
    public async Task InitializeDefaultSanctoralCalendar_AddsAllTLHDays()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);

        // Act
        var count = await service.InitializeDefaultSanctoralCalendarAsync();

        // Assert
        Assert.True(count > 30, "Should add at least 30 TLH sanctoral days");
        
        var allDays = await service.ListAllSanctoralDaysAsync();
        Assert.Equal(count, allDays.Count);
    }

    [Fact]
    public async Task InitializeDefaultSanctoralCalendar_DoesNotAddDuplicates()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);

        // Act - initialize twice
        var count1 = await service.InitializeDefaultSanctoralCalendarAsync();
        var count2 = await service.InitializeDefaultSanctoralCalendarAsync();

        // Assert
        Assert.True(count1 > 0);
        Assert.Equal(0, count2); // Second time should add nothing
    }

    [Fact]
    public async Task GetSanctoralDays_ChristmasDay_ReturnsNativity()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);
        await service.InitializeDefaultSanctoralCalendarAsync();

        // Act
        var date = new DateTime(2024, 12, 25);
        var days = await service.GetSanctoralDaysAsync(date);

        // Assert
        Assert.NotEmpty(days);
        Assert.Contains(days, d => d.Name.Contains("Nativity"));
        var christmas = days.First(d => d.Name.Contains("Nativity"));
        Assert.Equal(Rank.PrincipalFeast, christmas.Rank);
        Assert.Equal(LiturgicalColor.White, christmas.LiturgicalColor);
    }

    [Fact]
    public async Task GetSanctoralDays_ReformationDay_ReturnsReformationDay()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);
        await service.InitializeDefaultSanctoralCalendarAsync();

        // Act
        var date = new DateTime(2024, 10, 31);
        var days = await service.GetSanctoralDaysAsync(date);

        // Assert
        Assert.NotEmpty(days);
        var reformation = days.First(d => d.Name == "Reformation Day");
        Assert.Equal(Rank.Feast, reformation.Rank);
        Assert.Equal(LiturgicalColor.Red, reformation.LiturgicalColor);
    }

    [Fact]
    public async Task GetSanctoralDays_StAndrewDay_ReturnsApostle()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);
        await service.InitializeDefaultSanctoralCalendarAsync();

        // Act
        var date = new DateTime(2024, 11, 30);
        var days = await service.GetSanctoralDaysAsync(date);

        // Assert
        Assert.NotEmpty(days);
        var andrew = days.First();
        Assert.Contains("Andrew", andrew.Name);
        Assert.Equal(Rank.Apostle, andrew.Rank);
    }

    [Fact]
    public async Task AddSanctoralDay_CustomDay_MarksAsCustom()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);

        var customDay = new SanctoralDay
        {
            Month = 8,
            Day = 28,
            Name = "St. Augustine of Hippo",
            Rank = Rank.LesserFeast,
            LiturgicalColor = LiturgicalColor.White,
            Notes = "Doctor of the Church"
        };

        // Act
        var result = await service.AddSanctoralDayAsync(customDay);

        // Assert
        Assert.True(result.IsCustom);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task UpdateSanctoralDay_UpdatesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);

        var day = new SanctoralDay
        {
            Month = 7,
            Day = 15,
            Name = "Test Saint",
            Rank = Rank.LesserFeast,
            LiturgicalColor = LiturgicalColor.White
        };
        var added = await service.AddSanctoralDayAsync(day);

        // Act
        added.Name = "Updated Saint";
        var updated = await service.UpdateSanctoralDayAsync(added.Id, added);

        // Assert
        Assert.Equal("Updated Saint", updated.Name);
        
        var retrieved = await service.GetSanctoralDayByIdAsync(added.Id);
        Assert.Equal("Updated Saint", retrieved!.Name);
    }

    [Fact]
    public async Task DeleteSanctoralDay_DeletesSuccessfully()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);

        var day = new SanctoralDay
        {
            Month = 7,
            Day = 20,
            Name = "Test Saint to Delete",
            Rank = Rank.LesserFeast,
            LiturgicalColor = LiturgicalColor.White
        };
        var added = await service.AddSanctoralDayAsync(day);

        // Act
        var deleted = await service.DeleteSanctoralDayAsync(added.Id);

        // Assert
        Assert.True(deleted);
        var retrieved = await service.GetSanctoralDayByIdAsync(added.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task ListCustomSanctoralDays_ReturnsOnlyCustomDays()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);

        await service.InitializeDefaultSanctoralCalendarAsync();
        
        await service.AddSanctoralDayAsync(new SanctoralDay
        {
            Month = 8,
            Day = 28,
            Name = "Custom Saint 1",
            Rank = Rank.LesserFeast,
            LiturgicalColor = LiturgicalColor.White
        });

        await service.AddSanctoralDayAsync(new SanctoralDay
        {
            Month = 9,
            Day = 15,
            Name = "Custom Saint 2",
            Rank = Rank.LesserFeast,
            LiturgicalColor = LiturgicalColor.White
        });

        // Act
        var customDays = await service.ListCustomSanctoralDaysAsync();

        // Assert
        Assert.Equal(2, customDays.Count);
        Assert.All(customDays, d => Assert.True(d.IsCustom));
    }

    [Fact]
    public async Task GetSanctoralDays_DateWithNoDays_ReturnsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var service = new SanctoralService(repository);
        await service.InitializeDefaultSanctoralCalendarAsync();

        // Act - July 1 has no sanctoral day in TLH
        var date = new DateTime(2024, 7, 1);
        var days = await service.GetSanctoralDaysAsync(date);

        // Assert
        Assert.Empty(days);
    }
}
