using Bunit;
using FreiService.Data.Models;
using FreiService.Data.Services;
using FreiService.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FreiService.Web.Tests;

public class HolyDaysTests : TestContext
{
    private readonly Mock<IHolyDaysService> _mockHolyDaysService;

    public HolyDaysTests()
    {
        _mockHolyDaysService = new Mock<IHolyDaysService>();
        Services.AddSingleton(_mockHolyDaysService.Object);
    }

    [Fact]
    public void CalculateYear_Button_CalculatesHolyDays()
    {
        // Arrange
        var testYear = 2024;
        var testHolyDays = new Dictionary<string, DateTime>
        {
            { "Easter Sunday", new DateTime(2024, 3, 31) },
            { "Ash Wednesday", new DateTime(2024, 2, 14) }
        };

        _mockHolyDaysService
            .Setup(s => s.CalculateHolyDaysForYear(testYear))
            .Returns(testHolyDays);

        _mockHolyDaysService
            .Setup(s => s.HolyDaysExistForYearAsync(testYear))
            .ReturnsAsync(false);

        var cut = RenderComponent<HolyDays>();

        // Set the year input
        var yearInput = cut.Find("#yearInput");
        yearInput.Change(testYear.ToString());

        // Act
        var calculateButton = cut.Find("button:contains('Dry Run - Calculate')");
        calculateButton.Click();

        // Assert
        _mockHolyDaysService.Verify(s => s.CalculateHolyDaysForYear(testYear), Times.Once);
        Assert.Contains("Easter Sunday", cut.Markup);
        Assert.Contains("March 31, 2024", cut.Markup);
    }

    [Fact]
    public void SaveYear_Button_SavesHolyDaysToDatabase()
    {
        // Arrange
        var testYear = 2024;
        var testHolyDays = new Dictionary<string, DateTime>
        {
            { "Easter Sunday", new DateTime(2024, 3, 31) }
        };

        _mockHolyDaysService
            .Setup(s => s.CalculateHolyDaysForYear(testYear))
            .Returns(testHolyDays);

        _mockHolyDaysService
            .Setup(s => s.HolyDaysExistForYearAsync(testYear))
            .ReturnsAsync(false);

        _mockHolyDaysService
            .Setup(s => s.SaveHolyDaysForYearAsync(testYear))
            .ReturnsAsync(new List<HolyDay>());

        var cut = RenderComponent<HolyDays>();

        // First calculate to show the save button
        var yearInput = cut.Find("#yearInput");
        yearInput.Change(testYear.ToString());
        var calculateButton = cut.Find("button:contains('Dry Run - Calculate')");
        calculateButton.Click();

        // Act
        var saveButton = cut.Find("button:contains('Save to Database')");
        saveButton.Click();

        // Assert
        _mockHolyDaysService.Verify(s => s.SaveHolyDaysForYearAsync(testYear), Times.Once);
        Assert.Contains("Successfully saved holy days for year 2024", cut.Markup);
    }

    [Fact]
    public void ConfirmOverwrite_Button_OverwritesExistingData()
    {
        // Arrange
        var testYear = 2024;
        var testHolyDays = new Dictionary<string, DateTime>
        {
            { "Easter Sunday", new DateTime(2024, 3, 31) }
        };

        _mockHolyDaysService
            .Setup(s => s.CalculateHolyDaysForYear(testYear))
            .Returns(testHolyDays);

        _mockHolyDaysService
            .Setup(s => s.HolyDaysExistForYearAsync(testYear))
            .ReturnsAsync(true);

        _mockHolyDaysService
            .Setup(s => s.DeleteHolyDaysByYearAsync(testYear))
            .ReturnsAsync(5);

        _mockHolyDaysService
            .Setup(s => s.SaveHolyDaysForYearAsync(testYear))
            .ReturnsAsync(new List<HolyDay>());

        var cut = RenderComponent<HolyDays>();

        // First calculate to show the overwrite button
        var yearInput = cut.Find("#yearInput");
        yearInput.Change(testYear.ToString());
        var calculateButton = cut.Find("button:contains('Dry Run - Calculate')");
        calculateButton.Click();

        // Act
        var overwriteButton = cut.Find("button:contains('Overwrite Existing Data')");
        overwriteButton.Click();

        // Assert
        _mockHolyDaysService.Verify(s => s.DeleteHolyDaysByYearAsync(testYear), Times.Once);
        _mockHolyDaysService.Verify(s => s.SaveHolyDaysForYearAsync(testYear), Times.Once);
        Assert.Contains("Successfully overwritten holy days for year 2024", cut.Markup);
    }

    [Fact]
    public void LoadYear_Button_LoadsSavedHolyDays()
    {
        // Arrange
        var testYear = 2024;
        var testSavedHolyDays = new List<HolyDay>
        {
            new HolyDay { Name = "Easter Sunday", Date = new DateTime(2024, 3, 31), Year = 2024 },
            new HolyDay { Name = "Ash Wednesday", Date = new DateTime(2024, 2, 14), Year = 2024 }
        };

        _mockHolyDaysService
            .Setup(s => s.GetHolyDaysByYearAsync(testYear))
            .ReturnsAsync(testSavedHolyDays);

        var cut = RenderComponent<HolyDays>();

        // Set the view year input
        var viewYearInput = cut.Find("#viewYear");
        viewYearInput.Change(testYear.ToString());

        // Act
        var loadButton = cut.Find("button:contains('Load')");
        loadButton.Click();

        // Assert
        _mockHolyDaysService.Verify(s => s.GetHolyDaysByYearAsync(testYear), Times.AtLeastOnce);
        Assert.Contains("Easter Sunday", cut.Markup);
        Assert.Contains("Ash Wednesday", cut.Markup);
    }

    [Fact]
    public void DeleteYear_Button_DeletesHolyDaysForYear()
    {
        // Arrange
        var testYear = 2024;
        var testSavedHolyDays = new List<HolyDay>
        {
            new HolyDay { Name = "Easter Sunday", Date = new DateTime(2024, 3, 31), Year = 2024 }
        };

        _mockHolyDaysService
            .Setup(s => s.GetHolyDaysByYearAsync(testYear))
            .ReturnsAsync(testSavedHolyDays);

        _mockHolyDaysService
            .Setup(s => s.DeleteHolyDaysByYearAsync(testYear))
            .ReturnsAsync(5);

        var cut = RenderComponent<HolyDays>();

        // First load the year to show the delete button
        var viewYearInput = cut.Find("#viewYear");
        viewYearInput.Change(testYear.ToString());
        var loadButton = cut.Find("button:contains('Load')");
        loadButton.Click();

        // Act
        var deleteButton = cut.Find($"button:contains('Delete Year {testYear}')");
        deleteButton.Click();

        // Assert - Verify the service was called
        _mockHolyDaysService.Verify(s => s.DeleteHolyDaysByYearAsync(testYear), Times.Once);
    }
}
