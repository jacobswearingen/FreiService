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
}
