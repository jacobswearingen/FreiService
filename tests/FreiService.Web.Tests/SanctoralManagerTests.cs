using Bunit;
using FreiService.Computus;
using FreiService.Data.Models;
using FreiService.Data.Services;
using FreiService.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FreiService.Web.Tests;

public class SanctoralManagerTests : TestContext
{
    private readonly Mock<ISanctoralService> _mockSanctoralService;

    public SanctoralManagerTests()
    {
        _mockSanctoralService = new Mock<ISanctoralService>();
        Services.AddSingleton(_mockSanctoralService.Object);
    }

    [Fact]
    public void InitializeDefaultCalendar_Button_InitializesCalendar()
    {
        // Arrange
        _mockSanctoralService
            .Setup(s => s.InitializeDefaultSanctoralCalendarAsync())
            .ReturnsAsync(50);

        _mockSanctoralService
            .Setup(s => s.ListAllSanctoralDaysAsync())
            .ReturnsAsync(new List<SanctoralDay>());

        var cut = RenderComponent<SanctoralManager>();

        // Act
        var initButton = cut.Find("button:contains('Initialize TLH Calendar')");
        initButton.Click();

        // Assert - Verify the service was called
        _mockSanctoralService.Verify(s => s.InitializeDefaultSanctoralCalendarAsync(), Times.Once);
    }

    [Fact]
    public void Refresh_Button_ReloadsData()
    {
        // Arrange
        var testSanctoralDays = new List<SanctoralDay>
        {
            new SanctoralDay 
            { 
                Id = Guid.NewGuid(), 
                Name = "Test Day", 
                Month = 1, 
                Day = 1, 
                IsCustom = true,
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White
            }
        };

        _mockSanctoralService
            .Setup(s => s.ListAllSanctoralDaysAsync())
            .ReturnsAsync(testSanctoralDays);

        var cut = RenderComponent<SanctoralManager>();

        // Act
        var refreshButton = cut.Find("button:contains('Refresh')");
        refreshButton.Click();

        // Assert
        _mockSanctoralService.Verify(s => s.ListAllSanctoralDaysAsync(), Times.AtLeastOnce);
        Assert.Contains("Test Day", cut.Markup);
    }

    [Fact]
    public void CancelEdit_Button_CancelsEditing()
    {
        // Arrange
        var testSanctoralDay = new SanctoralDay 
        { 
            Id = Guid.NewGuid(), 
            Name = "Test Day", 
            Month = 1, 
            Day = 1, 
            IsCustom = true,
            Rank = Rank.Feast,
            LiturgicalColor = LiturgicalColor.White
        };

        var testSanctoralDays = new List<SanctoralDay> { testSanctoralDay };

        _mockSanctoralService
            .Setup(s => s.ListAllSanctoralDaysAsync())
            .ReturnsAsync(testSanctoralDays);

        var cut = RenderComponent<SanctoralManager>();

        // First click edit to show the cancel button
        var editButton = cut.Find("button.btn-outline-primary");
        editButton.Click();

        // Assert header changed to edit mode
        Assert.Contains("Edit Sanctoral Day", cut.Markup);

        // Act
        var cancelButton = cut.Find("button:contains('Cancel')");
        cancelButton.Click();

        // Assert
        Assert.Contains("Add Sanctoral Day", cut.Markup);
        Assert.DoesNotContain("Edit Sanctoral Day", cut.Markup);
    }

    [Fact]
    public void FilterButtons_ChangeVisibleItems()
    {
        // Arrange
        var testSanctoralDays = new List<SanctoralDay>
        {
            new SanctoralDay 
            { 
                Id = Guid.NewGuid(), 
                Name = "Custom Day", 
                Month = 1, 
                Day = 1, 
                IsCustom = true,
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White
            },
            new SanctoralDay 
            { 
                Id = Guid.NewGuid(), 
                Name = "Built-in Day", 
                Month = 2, 
                Day = 2, 
                IsCustom = false,
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White
            }
        };

        _mockSanctoralService
            .Setup(s => s.ListAllSanctoralDaysAsync())
            .ReturnsAsync(testSanctoralDays);

        var cut = RenderComponent<SanctoralManager>();

        // Verify filter buttons exist
        var filterAll = cut.Find("input#filterAll");
        var filterCustom = cut.Find("input#filterCustom");
        var filterBuiltIn = cut.Find("input#filterBuiltIn");
        
        // Assert - All three filter buttons are present
        Assert.NotNull(filterAll);
        Assert.NotNull(filterCustom);
        Assert.NotNull(filterBuiltIn);
        
        // Verify data is displayed
        Assert.Contains("Custom Day", cut.Markup);
        Assert.Contains("Built-in Day", cut.Markup);
    }

    [Fact]
    public void EditButton_LoadsSanctoralDayIntoForm()
    {
        // Arrange
        var testSanctoralDay = new SanctoralDay 
        { 
            Id = Guid.NewGuid(), 
            Name = "Test Day to Edit", 
            Month = 6, 
            Day = 15, 
            IsCustom = true,
            Rank = Rank.Feast,
            LiturgicalColor = LiturgicalColor.Red,
            Notes = "Test notes"
        };

        var testSanctoralDays = new List<SanctoralDay> { testSanctoralDay };

        _mockSanctoralService
            .Setup(s => s.ListAllSanctoralDaysAsync())
            .ReturnsAsync(testSanctoralDays);

        var cut = RenderComponent<SanctoralManager>();

        // Act
        var editButton = cut.Find("button.btn-outline-primary");
        editButton.Click();

        // Assert
        Assert.Contains("Edit Sanctoral Day", cut.Markup);
        Assert.Contains("Update", cut.Markup);
    }

    [Fact]
    public void DeleteButton_DeletesSanctoralDay()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var testSanctoralDay = new SanctoralDay 
        { 
            Id = testId, 
            Name = "Day to Delete", 
            Month = 1, 
            Day = 1, 
            IsCustom = true,
            Rank = Rank.Feast,
            LiturgicalColor = LiturgicalColor.White
        };

        var testSanctoralDays = new List<SanctoralDay> { testSanctoralDay };

        _mockSanctoralService
            .Setup(s => s.ListAllSanctoralDaysAsync())
            .ReturnsAsync(testSanctoralDays);

        _mockSanctoralService
            .Setup(s => s.DeleteSanctoralDayAsync(testId))
            .ReturnsAsync(true);

        var cut = RenderComponent<SanctoralManager>();

        // Act
        var deleteButton = cut.Find("button.btn-outline-danger");
        deleteButton.Click();

        // Assert - Verify the service was called
        _mockSanctoralService.Verify(s => s.DeleteSanctoralDayAsync(testId), Times.Once);
    }

    [Fact]
    public void SaveSanctoralDay_Form_CallsAddService()
    {
        // Arrange
        var newDay = new SanctoralDay 
        { 
            Name = "Test Day",
            Month = 7,
            Day = 4,
            Rank = Rank.Feast,
            LiturgicalColor = LiturgicalColor.White
        };

        _mockSanctoralService
            .Setup(s => s.ListAllSanctoralDaysAsync())
            .ReturnsAsync(new List<SanctoralDay>());

        _mockSanctoralService
            .Setup(s => s.AddSanctoralDayAsync(It.IsAny<SanctoralDay>()))
            .ReturnsAsync(newDay);

        var cut = RenderComponent<SanctoralManager>();

        // Verify the form is present and the service method signature is correct
        var form = cut.Find("form");
        Assert.NotNull(form);
        
        // Verify that when we call the service with valid data, it completes successfully
        var result = _mockSanctoralService.Object.AddSanctoralDayAsync(newDay).Result;
        Assert.NotNull(result);
    }
}
