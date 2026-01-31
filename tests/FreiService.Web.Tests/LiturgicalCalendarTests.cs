using Bunit;
using FreiService.Computus;
using FreiService.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FreiService.Web.Tests;

public class LiturgicalCalendarTests : TestContext
{
    private readonly Mock<ITemporalService> _mockTemporalService;

    public LiturgicalCalendarTests()
    {
        _mockTemporalService = new Mock<ITemporalService>();
        Services.AddSingleton(_mockTemporalService.Object);
    }

    [Fact]
    public void LoadSeasonInfo_Button_CalculatesSeasonDates()
    {
        // Arrange
        var testYear = 2024;
        var easterDate = new DateTime(2025, 4, 20);
        var advent1 = new DateTime(2024, 12, 1);
        
        var moveableFeasts = new Dictionary<string, DateTime>
        {
            { "Septuagesima Sunday", new DateTime(2025, 2, 9) },
            { "Ash Wednesday", new DateTime(2025, 3, 5) },
            { "Palmarum (Palm Sunday)", new DateTime(2025, 4, 13) },
            { "Holy Saturday", new DateTime(2025, 4, 19) },
            { "Pentecost (Whitsunday)", new DateTime(2025, 6, 8) }
        };

        _mockTemporalService
            .Setup(s => s.CalculateEaster(testYear + 1))
            .Returns(easterDate);

        _mockTemporalService
            .Setup(s => s.CalculateAdvent1(testYear))
            .Returns(advent1);

        _mockTemporalService
            .Setup(s => s.CalculateAdvent1(testYear + 1))
            .Returns(new DateTime(2025, 11, 30));

        _mockTemporalService
            .Setup(s => s.CalculateAllMoveableFeasts(testYear + 1))
            .Returns(moveableFeasts);

        _mockTemporalService
            .Setup(s => s.GetEpiphanySundayCount(testYear + 1))
            .Returns(5);

        _mockTemporalService
            .Setup(s => s.GetTrinitySundayCount(testYear + 1))
            .Returns(25);

        var cut = RenderComponent<LiturgicalCalendar>();

        // Set the year input
        var yearInput = cut.Find("#yearInput");
        yearInput.Change(testYear.ToString());

        // Act
        var calculateButton = cut.Find("button:contains('Calculate Season Dates')");
        calculateButton.Click();

        // Assert
        _mockTemporalService.Verify(s => s.CalculateEaster(testYear + 1), Times.Once);
        _mockTemporalService.Verify(s => s.CalculateAdvent1(testYear), Times.Once);
        _mockTemporalService.Verify(s => s.CalculateAllMoveableFeasts(testYear + 1), Times.Once);
        
        Assert.Contains("Liturgical Seasons for 2024", cut.Markup);
        Assert.Contains("Advent", cut.Markup);
        Assert.Contains("Christmas", cut.Markup);
        Assert.Contains("Epiphany", cut.Markup);
        Assert.Contains("Lent", cut.Markup);
        Assert.Contains("Easter", cut.Markup);
        Assert.Contains("Trinity (After Pentecost)", cut.Markup);
    }

    [Fact]
    public void Component_InitializesOnLoad()
    {
        // Arrange
        var currentYear = DateTime.Now.Year;
        var easterDate = new DateTime(currentYear + 1, 4, 20);
        var advent1 = new DateTime(currentYear, 12, 1);
        
        var moveableFeasts = new Dictionary<string, DateTime>
        {
            { "Septuagesima Sunday", new DateTime(currentYear + 1, 2, 9) },
            { "Ash Wednesday", new DateTime(currentYear + 1, 3, 5) },
            { "Palmarum (Palm Sunday)", new DateTime(currentYear + 1, 4, 13) },
            { "Holy Saturday", new DateTime(currentYear + 1, 4, 19) },
            { "Pentecost (Whitsunday)", new DateTime(currentYear + 1, 6, 8) }
        };

        _mockTemporalService
            .Setup(s => s.CalculateEaster(currentYear + 1))
            .Returns(easterDate);

        _mockTemporalService
            .Setup(s => s.CalculateAdvent1(currentYear))
            .Returns(advent1);

        _mockTemporalService
            .Setup(s => s.CalculateAdvent1(currentYear + 1))
            .Returns(new DateTime(currentYear + 1, 11, 30));

        _mockTemporalService
            .Setup(s => s.CalculateAllMoveableFeasts(currentYear + 1))
            .Returns(moveableFeasts);

        _mockTemporalService
            .Setup(s => s.GetEpiphanySundayCount(currentYear + 1))
            .Returns(5);

        _mockTemporalService
            .Setup(s => s.GetTrinitySundayCount(currentYear + 1))
            .Returns(25);

        // Act
        var cut = RenderComponent<LiturgicalCalendar>();

        // Assert - Component should call LoadSeasonInfo on initialization
        _mockTemporalService.Verify(s => s.CalculateEaster(currentYear + 1), Times.Once);
        Assert.Contains("Liturgical Calendar", cut.Markup);
    }

    [Fact]
    public void LoadSeasonInfo_ShowsEpiphanyAndTrinitySundayCounts()
    {
        // Arrange
        var testYear = 2024;
        var easterDate = new DateTime(2025, 4, 20);
        var advent1 = new DateTime(2024, 12, 1);
        
        var moveableFeasts = new Dictionary<string, DateTime>
        {
            { "Septuagesima Sunday", new DateTime(2025, 2, 9) },
            { "Ash Wednesday", new DateTime(2025, 3, 5) },
            { "Palmarum (Palm Sunday)", new DateTime(2025, 4, 13) },
            { "Holy Saturday", new DateTime(2025, 4, 19) },
            { "Pentecost (Whitsunday)", new DateTime(2025, 6, 8) }
        };

        _mockTemporalService
            .Setup(s => s.CalculateEaster(testYear + 1))
            .Returns(easterDate);

        _mockTemporalService
            .Setup(s => s.CalculateAdvent1(testYear))
            .Returns(advent1);

        _mockTemporalService
            .Setup(s => s.CalculateAdvent1(testYear + 1))
            .Returns(new DateTime(2025, 11, 30));

        _mockTemporalService
            .Setup(s => s.CalculateAllMoveableFeasts(testYear + 1))
            .Returns(moveableFeasts);

        _mockTemporalService
            .Setup(s => s.GetEpiphanySundayCount(testYear + 1))
            .Returns(6);

        _mockTemporalService
            .Setup(s => s.GetTrinitySundayCount(testYear + 1))
            .Returns(27);

        var cut = RenderComponent<LiturgicalCalendar>();

        // Act
        var yearInput = cut.Find("#yearInput");
        yearInput.Change(testYear.ToString());
        var calculateButton = cut.Find("button:contains('Calculate Season Dates')");
        calculateButton.Click();

        // Assert
        Assert.Contains("Sundays after Epiphany:", cut.Markup);
        Assert.Contains("6", cut.Markup);
        Assert.Contains("Sundays after Trinity:", cut.Markup);
        Assert.Contains("27", cut.Markup);
    }
}
