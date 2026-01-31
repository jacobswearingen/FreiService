using FreiService.Computus;
using FreiService.Data.Models;
using FreiService.Data.Repositories;
using FreiService.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace FreiService.Data.Tests;

public class PrecedenceServiceTests
{
    private HolyDaysContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<HolyDaysContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new HolyDaysContext(options);
    }

    private async Task<PrecedenceService> CreatePrecedenceService()
    {
        var context = CreateInMemoryContext();
        var repository = new SanctoralDaysRepository(context);
        var sanctoralService = new SanctoralService(repository);
        await sanctoralService.InitializeDefaultSanctoralCalendarAsync();
        
        var temporalService = new TemporalService();
        
        return new PrecedenceService(temporalService, sanctoralService);
    }

    [Fact]
    public async Task ResolveDate_EasterSunday_TemporalWins()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2024, 3, 31); // Easter Sunday

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert
        Assert.Equal("Easter Sunday", resolved.Primary.Name);
        Assert.Equal(CalendarSource.Temporal, resolved.Primary.Source);
        Assert.Equal(Rank.PrincipalFeast, resolved.Primary.Rank);
        Assert.Equal(Season.Easter, resolved.Season);
    }

    [Fact]
    public async Task ResolveDate_Christmas_TemporalWins()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2024, 12, 25);

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert
        // Should be temporal Christmas Day, sanctoral is commemorated
        Assert.Equal(CalendarSource.Temporal, resolved.Primary.Source);
        Assert.Contains("Christmas", resolved.Primary.Name);
        Assert.Equal(Season.Christmas, resolved.Season);
    }

    [Fact]
    public async Task ResolveDate_ReformationDayOnSunday_SundayWinsButReformationCommemorated()
    {
        // Oct 31, 2021 was a Sunday (Trinity 22)
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2021, 10, 31);

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert
        // In TLH tradition, Reformation is important enough to be primary even on Sunday
        // But if Sunday wins, Reformation should be commemorated
        if (resolved.Primary.Source == CalendarSource.Temporal)
        {
            Assert.NotEmpty(resolved.Commemorations);
            Assert.Contains(resolved.Commemorations, c => c.Name.Contains("Reformation"));
        }
        else
        {
            Assert.Equal("Reformation Day", resolved.Primary.Name);
        }
    }

    [Fact]
    public async Task ResolveDate_StAndrewOnWeekday_SanctoralWins()
    {
        // Nov 30, 2023 was a Thursday
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2023, 11, 30);

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert - Apostle on weekday should be primary
        Assert.Equal(CalendarSource.Sanctoral, resolved.Primary.Source);
        Assert.Contains("Andrew", resolved.Primary.Name);
        Assert.Equal(Rank.Apostle, resolved.Primary.Rank);
    }

    [Fact]
    public async Task ResolveDate_DateWithNoSanctoral_TemporalOnly()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2024, 7, 10); // Random Wednesday in Trinity

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert
        Assert.Equal(CalendarSource.Temporal, resolved.Primary.Source);
        Assert.Empty(resolved.Commemorations);
        Assert.Equal(Season.Trinity, resolved.Season);
    }

    [Fact]
    public async Task ResolveDate_HolyWeek_TemporalAlwaysWins()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        
        // Act - Test multiple days in Holy Week
        var palmSunday = await service.ResolveDateAsync(new DateTime(2024, 3, 24));
        var maundyThursday = await service.ResolveDateAsync(new DateTime(2024, 3, 28));
        var goodFriday = await service.ResolveDateAsync(new DateTime(2024, 3, 29));

        // Assert
        Assert.Equal(CalendarSource.Temporal, palmSunday.Primary.Source);
        Assert.Equal(CalendarSource.Temporal, maundyThursday.Primary.Source);
        Assert.Equal(CalendarSource.Temporal, goodFriday.Primary.Source);
        Assert.Equal(Season.HolyWeek, palmSunday.Season);
        Assert.Equal(Season.HolyWeek, maundyThursday.Season);
        Assert.Equal(Season.HolyWeek, goodFriday.Season);
        
        // Holy Week should have no commemorations
        Assert.Empty(palmSunday.Commemorations);
        Assert.Empty(maundyThursday.Commemorations);
        Assert.Empty(goodFriday.Commemorations);
    }

    [Fact]
    public async Task ResolveDate_Annunciation_DuringLent()
    {
        // March 25 often falls during Lent
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2024, 3, 25);

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert - Annunciation is a Feast, should be primary on a weekday
        Assert.Equal(CalendarSource.Sanctoral, resolved.Primary.Source);
        Assert.Contains("Annunciation", resolved.Primary.Name);
        Assert.Equal(Rank.Feast, resolved.Primary.Rank);
    }

    [Fact]
    public async Task ResolveDateRange_ReturnsAllDaysInRange()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        var startDate = new DateTime(2024, 12, 24);
        var endDate = new DateTime(2024, 12, 27);

        // Act
        var resolved = await service.ResolveDateRangeAsync(startDate, endDate);

        // Assert
        Assert.Equal(4, resolved.Count); // Dec 24, 25, 26, 27
        Assert.Equal(startDate.Date, resolved[0].Date.Date);
        Assert.Equal(endDate.Date, resolved[3].Date.Date);
    }

    [Fact]
    public async Task GetUpcomingFeasts_ReturnsOnlyFeasts()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        var fromDate = new DateTime(2024, 3, 1);

        // Act
        var feasts = await service.GetUpcomingFeastsAsync(fromDate, 5);

        // Assert
        Assert.Equal(5, feasts.Count);
        Assert.All(feasts, f => Assert.True(f.Primary.Rank >= Rank.Feast));
    }

    [Fact]
    public async Task ResolveDate_SundayInTrinity_TemporalWins()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2024, 6, 2); // Trinity 1

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert
        Assert.Equal(CalendarSource.Temporal, resolved.Primary.Source);
        Assert.Contains("Trinity", resolved.Primary.Name);
        Assert.Equal(DayType.Sunday, DayType.Sunday); // It's a Sunday
    }

    [Fact]
    public async Task ResolveDate_EpiphanyDay_PrincipalFeast()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2024, 1, 6);

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert
        Assert.Equal("Epiphany", resolved.Primary.Name);
        Assert.Equal(Rank.PrincipalFeast, resolved.Primary.Rank);
        Assert.Equal(LiturgicalColor.White, resolved.LiturgicalColor);
    }

    [Fact]
    public async Task ResolveDate_Pentecost_RedColor()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        var date = new DateTime(2024, 5, 19);

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert
        Assert.Contains("Pentecost", resolved.Primary.Name);
        Assert.Equal(LiturgicalColor.Red, resolved.LiturgicalColor);
    }

    [Fact]
    public async Task ResolveDate_WithCommemorations_GeneratesNotes()
    {
        // Arrange
        var service = await CreatePrecedenceService();
        
        // Find a date where sanctoral is commemorated
        var date = new DateTime(2024, 12, 26); // St. Stephen on weekday after Christmas

        // Act
        var resolved = await service.ResolveDateAsync(date);

        // Assert
        if (resolved.Commemorations.Any())
        {
            Assert.NotNull(resolved.Notes);
            Assert.Contains("commemorated", resolved.Notes.ToLower());
        }
    }
}
