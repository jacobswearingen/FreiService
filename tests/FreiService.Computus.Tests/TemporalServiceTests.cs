using FreiService.Computus;

namespace FreiService.Computus.Tests;

public class TemporalServiceTests
{
    private readonly TemporalService _service;

    public TemporalServiceTests()
    {
        _service = new TemporalService();
    }

    [Fact]
    public void CalculateAllMoveableFeasts_2024_ReturnsCorrectDates()
    {
        // Act
        var feasts = _service.CalculateAllMoveableFeasts(2024);

        // Assert - spot check key dates
        // Easter 2024 is March 31
        // 63 days before = Jan 28 (Septuagesima)
        Assert.Equal(new DateTime(2024, 1, 28), feasts["Septuagesima Sunday"]);
        Assert.Equal(new DateTime(2024, 2, 4), feasts["Sexagesima Sunday"]);
        Assert.Equal(new DateTime(2024, 2, 11), feasts["Quinquagesima Sunday"]);
        Assert.Equal(new DateTime(2024, 2, 14), feasts["Ash Wednesday"]);
        Assert.Equal(new DateTime(2024, 3, 24), feasts["Palmarum (Palm Sunday)"]);
        Assert.Equal(new DateTime(2024, 3, 28), feasts["Maundy Thursday"]);
        Assert.Equal(new DateTime(2024, 3, 29), feasts["Good Friday"]);
        Assert.Equal(new DateTime(2024, 3, 30), feasts["Holy Saturday"]);
        Assert.Equal(new DateTime(2024, 3, 31), feasts["Easter Sunday"]);
        Assert.Equal(new DateTime(2024, 5, 9), feasts["Ascension"]);
        Assert.Equal(new DateTime(2024, 5, 19), feasts["Pentecost (Whitsunday)"]);
        Assert.Equal(new DateTime(2024, 5, 26), feasts["Trinity Sunday"]);
    }

    [Fact]
    public void CalculateAdvent1_2024_ReturnsCorrectDate()
    {
        // Advent 1 in 2024 should be December 1
        var result = _service.CalculateAdvent1(2024);
        Assert.Equal(new DateTime(2024, 12, 1), result);
        Assert.Equal(DayOfWeek.Sunday, result.DayOfWeek);
    }

    [Fact]
    public void CalculateAdvent1_AlwaysFourSundaysBeforeChristmas()
    {
        for (int year = 2020; year <= 2030; year++)
        {
            var advent1 = _service.CalculateAdvent1(year);
            var christmas = new DateTime(year, 12, 25);
            
            // Should be a Sunday
            Assert.Equal(DayOfWeek.Sunday, advent1.DayOfWeek);
            
            // Should be 4 Sundays before Christmas (28 days)
            var daysBetween = (christmas - advent1).Days;
            Assert.True(daysBetween >= 22 && daysBetween <= 28, 
                $"Advent 1 for {year} should be 22-28 days before Christmas, was {daysBetween}");
        }
    }

    [Theory]
    [InlineData(2024, 3)] // 2024 has 3 Sundays after Epiphany before Septuagesima
    [InlineData(2027, 2)] // 2027 has 2 Sundays after Epiphany (early Easter)
    public void GetEpiphanySundayCount_ReturnsCorrectCount(int year, int expectedCount)
    {
        var count = _service.GetEpiphanySundayCount(year);
        Assert.InRange(count, 1, 6); // Should always be 1-6
        Assert.Equal(expectedCount, count);
    }

    [Theory]
    [InlineData(2024, 27)] // 2024 has 27 Sundays after Trinity
    [InlineData(2025, 24)] // 2025 has 24 Sundays after Trinity
    public void GetTrinitySundayCount_ReturnsCorrectCount(int year, int expectedCount)
    {
        var count = _service.GetTrinitySundayCount(year);
        Assert.InRange(count, 23, 27); // Should typically be 23-27
        Assert.Equal(expectedCount, count);
    }

    [Fact]
    public void GetTemporalDay_EasterSunday2024_ReturnsCorrectInfo()
    {
        var date = new DateTime(2024, 3, 31);
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(date, result.Date);
        Assert.Equal(Season.Easter, result.Season);
        Assert.Equal("Easter Sunday", result.DayName);
        Assert.Equal(DayType.PrincipalFeast, result.DayType);
        Assert.Equal(LiturgicalColor.White, result.LiturgicalColor);
        Assert.Equal(Rank.PrincipalFeast, result.Rank);
    }

    [Fact]
    public void GetTemporalDay_Advent1_2024_ReturnsCorrectInfo()
    {
        var date = new DateTime(2024, 12, 1);
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(Season.Advent, result.Season);
        Assert.Equal("Advent 1", result.DayName);
        Assert.Equal(DayType.Sunday, result.DayType);
        Assert.Equal(LiturgicalColor.Violet, result.LiturgicalColor);
        Assert.Equal(1, result.WeekOfSeason);
    }

    [Fact]
    public void GetTemporalDay_GoodFriday2024_ReturnsBlackColor()
    {
        var date = new DateTime(2024, 3, 29);
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(Season.HolyWeek, result.Season);
        Assert.Equal("Good Friday", result.DayName);
        Assert.Equal(LiturgicalColor.Black, result.LiturgicalColor);
    }

    [Fact]
    public void GetTemporalDay_Pentecost2024_ReturnsRedColor()
    {
        var date = new DateTime(2024, 5, 19);
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(Season.Trinity, result.Season);
        Assert.Contains("Pentecost", result.DayName);
        Assert.Equal(LiturgicalColor.Red, result.LiturgicalColor);
    }

    [Fact]
    public void GetTemporalDay_TrinitySunday2024_ReturnsWhiteColor()
    {
        var date = new DateTime(2024, 5, 26);
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(Season.Trinity, result.Season);
        Assert.Equal("Trinity Sunday", result.DayName);
        Assert.Equal(LiturgicalColor.White, result.LiturgicalColor);
    }

    [Fact]
    public void GetTemporalDay_Trinity5_2024_ReturnsGreenColor()
    {
        var date = new DateTime(2024, 6, 30); // Trinity 5
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(Season.Trinity, result.Season);
        Assert.Equal(LiturgicalColor.Green, result.LiturgicalColor);
        Assert.Equal("Trinity 5", result.DayName);
    }

    [Fact]
    public void GetTemporalDay_LentSunday_ReturnsVioletColor()
    {
        var date = new DateTime(2024, 2, 18); // Sexagesima or early Lent
        
        var result = _service.GetTemporalDay(date);
        
        Assert.True(result.Season == Season.Septuagesima || result.Season == Season.Lent);
        Assert.Equal(LiturgicalColor.Violet, result.LiturgicalColor);
    }

    [Fact]
    public void CalculateTrinityWeek_2024_Week1_ReturnsCorrectDate()
    {
        var result = _service.CalculateTrinityWeek(2024, 1);
        
        // Trinity 1 should be 7 days after Trinity Sunday (May 26 + 7 = June 2)
        Assert.Equal(new DateTime(2024, 6, 2), result);
        Assert.Equal(DayOfWeek.Sunday, result.DayOfWeek);
    }

    [Fact]
    public void CalculateTrinityWeek_InvalidWeek_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalculateTrinityWeek(2024, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalculateTrinityWeek(2024, 28));
    }

    [Fact]
    public void GetTemporalDay_ChristmasDay_ReturnsCorrectInfo()
    {
        var date = new DateTime(2024, 12, 25);
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(Season.Christmas, result.Season);
        Assert.Equal("Christmas Day", result.DayName);
        Assert.Equal(LiturgicalColor.White, result.LiturgicalColor);
    }

    [Fact]
    public void GetTemporalDay_EpiphanyDay_ReturnsCorrectInfo()
    {
        var date = new DateTime(2024, 1, 6);
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(Season.Epiphany, result.Season);
        Assert.Equal("Epiphany", result.DayName);
        Assert.Equal(LiturgicalColor.White, result.LiturgicalColor);
    }

    [Fact]
    public void GetTemporalDay_WeekdayInTrinity_ReturnsWeekday()
    {
        var date = new DateTime(2024, 7, 10); // Wednesday in Trinity season
        
        var result = _service.GetTemporalDay(date);
        
        Assert.Equal(Season.Trinity, result.Season);
        Assert.Equal(DayType.Weekday, result.DayType);
        Assert.Equal(LiturgicalColor.Green, result.LiturgicalColor);
    }
}
