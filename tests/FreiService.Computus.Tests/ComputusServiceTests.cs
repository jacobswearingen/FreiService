namespace FreiService.Computus.Tests;

public class ComputusServiceTests
{
    private readonly ComputusService _service;

    public ComputusServiceTests()
    {
        _service = new ComputusService();
    }

    [Theory]
    [InlineData(2024, 3, 31)]  // Easter 2024
    [InlineData(2025, 4, 20)]  // Easter 2025
    [InlineData(2026, 4, 5)]   // Easter 2026
    [InlineData(2027, 3, 28)]  // Easter 2027
    [InlineData(2028, 4, 16)]  // Easter 2028
    [InlineData(2029, 4, 1)]   // Easter 2029
    [InlineData(2030, 4, 21)]  // Easter 2030
    [InlineData(2000, 4, 23)]  // Easter 2000 (Y2K)
    [InlineData(1999, 4, 4)]   // Easter 1999
    [InlineData(1990, 4, 15)]  // Easter 1990
    [InlineData(1980, 4, 6)]   // Easter 1980
    [InlineData(2100, 3, 28)]  // Easter 2100
    public void CalculateEaster_KnownDates_ReturnsCorrectDate(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateEaster(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void CalculateEaster_YearBefore1583_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalculateEaster(1582));
    }

    [Fact]
    public void CalculateEaster_Year1583_ReturnsValidDate()
    {
        // Act
        var result = _service.CalculateEaster(1583);

        // Assert
        Assert.Equal(new DateTime(1583, 4, 10), result);
    }

    [Fact]
    public void CalculateEasterRange_ValidRange_ReturnsCorrectDictionary()
    {
        // Arrange
        int startYear = 2024;
        int endYear = 2026;

        // Act
        var results = _service.CalculateEasterRange(startYear, endYear);

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal(new DateTime(2024, 3, 31), results[2024]);
        Assert.Equal(new DateTime(2025, 4, 20), results[2025]);
        Assert.Equal(new DateTime(2026, 4, 5), results[2026]);
    }

    [Fact]
    public void CalculateEasterRange_SingleYear_ReturnsOneDateInDictionary()
    {
        // Arrange
        int year = 2024;

        // Act
        var results = _service.CalculateEasterRange(year, year);

        // Assert
        Assert.Single(results);
        Assert.Equal(new DateTime(2024, 3, 31), results[2024]);
    }

    [Fact]
    public void CalculateEasterRange_StartYearGreaterThanEndYear_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.CalculateEasterRange(2026, 2024));
    }

    [Fact]
    public void CalculateEaster_AlwaysReturnsSunday()
    {
        // Test that Easter is always on a Sunday for a range of years
        for (int year = 2000; year <= 2030; year++)
        {
            // Act
            var easterDate = _service.CalculateEaster(year);

            // Assert
            Assert.Equal(DayOfWeek.Sunday, easterDate.DayOfWeek);
        }
    }

    [Fact]
    public void CalculateEaster_DateInMarchOrApril()
    {
        // Easter always falls in March or April
        for (int year = 2000; year <= 2030; year++)
        {
            // Act
            var easterDate = _service.CalculateEaster(year);

            // Assert
            Assert.True(easterDate.Month == 3 || easterDate.Month == 4, 
                $"Easter {year} should be in March or April, but was {easterDate.Month}");
        }
    }

    [Theory]
    [InlineData(2024, 2, 14)]  // Ash Wednesday 2024 (46 days before March 31)
    [InlineData(2025, 3, 5)]   // Ash Wednesday 2025 (46 days before April 20)
    [InlineData(2026, 2, 18)]  // Ash Wednesday 2026 (46 days before April 5)
    public void CalculateAshWednesday_KnownDates_ReturnsCorrectDate(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateAshWednesday(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void CalculateAshWednesday_AlwaysWednesday()
    {
        // Ash Wednesday is always on a Wednesday
        for (int year = 2000; year <= 2030; year++)
        {
            // Act
            var ashWednesday = _service.CalculateAshWednesday(year);

            // Assert
            Assert.Equal(DayOfWeek.Wednesday, ashWednesday.DayOfWeek);
        }
    }

    [Theory]
    [InlineData(2024, 5, 19)]  // Pentecost 2024 (49 days after March 31)
    [InlineData(2025, 6, 8)]   // Pentecost 2025 (49 days after April 20)
    [InlineData(2026, 5, 24)]  // Pentecost 2026 (49 days after April 5)
    public void CalculatePentecost_KnownDates_ReturnsCorrectDate(int year, int month, int day)
    {
        // Act
        var result = _service.CalculatePentecost(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void CalculatePentecost_AlwaysSunday()
    {
        // Pentecost is always on a Sunday
        for (int year = 2000; year <= 2030; year++)
        {
            // Act
            var pentecost = _service.CalculatePentecost(year);

            // Assert
            Assert.Equal(DayOfWeek.Sunday, pentecost.DayOfWeek);
        }
    }

    [Theory]
    [InlineData(2024, 5, 26)]  // Trinity Sunday 2024 (56 days after March 31)
    [InlineData(2025, 6, 15)]  // Trinity Sunday 2025 (56 days after April 20)
    [InlineData(2026, 5, 31)]  // Trinity Sunday 2026 (56 days after April 5)
    public void CalculateTrinity_KnownDates_ReturnsCorrectDate(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateTrinity(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void CalculateTrinity_AlwaysSunday()
    {
        // Trinity Sunday is always on a Sunday
        for (int year = 2000; year <= 2030; year++)
        {
            // Act
            var trinitySunday = _service.CalculateTrinity(year);

            // Assert
            Assert.Equal(DayOfWeek.Sunday, trinitySunday.DayOfWeek);
        }
    }

    [Theory]
    [InlineData(2024, 3, 25)]
    [InlineData(2025, 3, 25)]
    [InlineData(2026, 3, 25)]
    public void CalculateAnnunciation_AlwaysMarch25(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateAnnunciation(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void CalculateAllHolyDays_ReturnsAllFiveHolyDays()
    {
        // Arrange
        int year = 2024;

        // Act
        var holyDays = _service.CalculateAllHolyDays(year);

        // Assert
        Assert.Equal(11, holyDays.Count);
        Assert.True(holyDays.ContainsKey("Easter Sunday"));
        Assert.True(holyDays.ContainsKey("Ash Wednesday"));
        Assert.True(holyDays.ContainsKey("Good Friday"));
        Assert.True(holyDays.ContainsKey("Ascension Day"));
        Assert.True(holyDays.ContainsKey("Pentecost"));
        Assert.True(holyDays.ContainsKey("Trinity Sunday"));
        Assert.True(holyDays.ContainsKey("Annunciation of Mary"));
        Assert.True(holyDays.ContainsKey("Christmas"));
        Assert.True(holyDays.ContainsKey("Epiphany"));
        Assert.True(holyDays.ContainsKey("Reformation Day"));
        Assert.True(holyDays.ContainsKey("All Saints' Day"));
    }

    [Fact]
    public void CalculateAllHolyDays_ReturnsCorrectDatesFor2024()
    {
        // Arrange
        int year = 2024;

        // Act
        var holyDays = _service.CalculateAllHolyDays(year);

        // Assert
        Assert.Equal(new DateTime(2024, 3, 31), holyDays["Easter Sunday"]);
        Assert.Equal(new DateTime(2024, 2, 14), holyDays["Ash Wednesday"]);
        Assert.Equal(new DateTime(2024, 3, 29), holyDays["Good Friday"]);
        Assert.Equal(new DateTime(2024, 5, 9), holyDays["Ascension Day"]);
        Assert.Equal(new DateTime(2024, 5, 19), holyDays["Pentecost"]);
        Assert.Equal(new DateTime(2024, 5, 26), holyDays["Trinity Sunday"]);
        Assert.Equal(new DateTime(2024, 3, 25), holyDays["Annunciation of Mary"]);
        Assert.Equal(new DateTime(2024, 12, 25), holyDays["Christmas"]);
        Assert.Equal(new DateTime(2024, 1, 6), holyDays["Epiphany"]);
        Assert.Equal(new DateTime(2024, 10, 31), holyDays["Reformation Day"]);
        Assert.Equal(new DateTime(2024, 11, 1), holyDays["All Saints' Day"]);
    }

    [Theory]
    [InlineData(2024, 12, 25)]
    [InlineData(2025, 12, 25)]
    [InlineData(2026, 12, 25)]
    public void CalculateChristmas_AlwaysDecember25(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateChristmas(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Theory]
    [InlineData(2024, 1, 6)]
    [InlineData(2025, 1, 6)]
    [InlineData(2026, 1, 6)]
    public void CalculateEpiphany_AlwaysJanuary6(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateEpiphany(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Theory]
    [InlineData(2024, 3, 29)]  // Good Friday 2024 (2 days before March 31)
    [InlineData(2025, 4, 18)]  // Good Friday 2025 (2 days before April 20)
    [InlineData(2026, 4, 3)]   // Good Friday 2026 (2 days before April 5)
    public void CalculateGoodFriday_KnownDates_ReturnsCorrectDate(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateGoodFriday(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void CalculateGoodFriday_AlwaysFriday()
    {
        // Good Friday is always on a Friday
        for (int year = 2000; year <= 2030; year++)
        {
            // Act
            var goodFriday = _service.CalculateGoodFriday(year);

            // Assert
            Assert.Equal(DayOfWeek.Friday, goodFriday.DayOfWeek);
        }
    }

    [Theory]
    [InlineData(2024, 5, 9)]   // Ascension 2024 (39 days after March 31)
    [InlineData(2025, 5, 29)]  // Ascension 2025 (39 days after April 20)
    [InlineData(2026, 5, 14)]  // Ascension 2026 (39 days after April 5)
    public void CalculateAscension_KnownDates_ReturnsCorrectDate(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateAscension(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void CalculateAscension_AlwaysThursday()
    {
        // Ascension is always on a Thursday (40 days after Easter, counting Easter as day 1)
        for (int year = 2000; year <= 2030; year++)
        {
            // Act
            var ascension = _service.CalculateAscension(year);

            // Assert
            Assert.Equal(DayOfWeek.Thursday, ascension.DayOfWeek);
        }
    }

    [Theory]
    [InlineData(2024, 10, 31)]
    [InlineData(2025, 10, 31)]
    [InlineData(2026, 10, 31)]
    public void CalculateReformationDay_AlwaysOctober31(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateReformationDay(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Theory]
    [InlineData(2024, 11, 1)]
    [InlineData(2025, 11, 1)]
    [InlineData(2026, 11, 1)]
    public void CalculateAllSaintsDay_AlwaysNovember1(int year, int month, int day)
    {
        // Act
        var result = _service.CalculateAllSaintsDay(year);

        // Assert
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void GetChurchSeason_Advent_ReturnsAdvent()
    {
        // First Sunday of Advent 2024 is December 1
        var date = new DateTime(2024, 12, 1);

        // Act
        var season = _service.GetChurchSeason(date, 2024);

        // Assert
        Assert.Equal(ChurchSeason.Advent, season);
    }

    [Fact]
    public void GetChurchSeason_Christmas_ReturnsChristmas()
    {
        // December 25 is Christmas
        var date = new DateTime(2024, 12, 25);

        // Act
        var season = _service.GetChurchSeason(date, 2024);

        // Assert
        Assert.Equal(ChurchSeason.Christmas, season);
    }

    [Fact]
    public void GetChurchSeason_Epiphany_ReturnsEpiphany()
    {
        // January 6 is Epiphany
        var date = new DateTime(2024, 1, 6);

        // Act
        var season = _service.GetChurchSeason(date, 2024);

        // Assert
        Assert.Equal(ChurchSeason.Epiphany, season);
    }

    [Fact]
    public void GetChurchSeason_Lent_ReturnsLent()
    {
        // Ash Wednesday 2024 is February 14
        var date = new DateTime(2024, 2, 14);

        // Act
        var season = _service.GetChurchSeason(date, 2024);

        // Assert
        Assert.Equal(ChurchSeason.Lent, season);
    }

    [Fact]
    public void GetChurchSeason_Easter_ReturnsEaster()
    {
        // Easter Sunday 2024 is March 31
        var date = new DateTime(2024, 3, 31);

        // Act
        var season = _service.GetChurchSeason(date, 2024);

        // Assert
        Assert.Equal(ChurchSeason.Easter, season);
    }

    [Fact]
    public void GetChurchSeason_Pentecost_ReturnsPentecost()
    {
        // Pentecost 2024 is May 19
        var date = new DateTime(2024, 5, 19);

        // Act
        var season = _service.GetChurchSeason(date, 2024);

        // Assert
        Assert.Equal(ChurchSeason.Pentecost, season);
    }

    [Fact]
    public void GetChurchSeason_MidSummer_ReturnsPentecost()
    {
        // July 4, 2024 should be in Pentecost/Trinity season
        var date = new DateTime(2024, 7, 4);

        // Act
        var season = _service.GetChurchSeason(date, 2024);

        // Assert
        Assert.Equal(ChurchSeason.Pentecost, season);
    }
}
