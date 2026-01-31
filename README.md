# FreiService - TLH Liturgical Calendar System

A comprehensive C# application for managing the complete liturgical calendar according to The Lutheran Hymnal (TLH, 1941) tradition, including computus calculations, sanctoral commemorations, and precedence resolution.

## Overview

FreiService provides a complete three-service architecture for the liturgical calendar:
- **TemporalService**: Calculates all moveable feasts based on Easter (Temporale)
- **SanctoralService**: Manages fixed commemorations tied to calendar dates (Sanctorale)
- **PrecedenceService**: Resolves conflicts between Temporal and Sanctoral observances

This is particularly useful for:
- Church calendar planning and liturgy preparation
- Confessional Lutheran parishes following TLH tradition
- Liturgical software development
- Religious education and catechesis

## Projects

### FreiService.Computus
Core library for calculating Easter dates and the complete Temporale cycle using the Meeus/Jones/Butcher algorithm. Now includes:
- All TLH moveable feasts (Septuagesima through Trinity Sundays)
- Season determination with liturgical colors
- Advent and Epiphany calculations
- Support for up to 27 Sundays after Trinity

### FreiService.Data
Data access layer with Entity Framework Core support for:
- Sanctoral calendar storage (saints' days, feasts, apostles)
- Pre-populated TLH sanctoral calendar (30+ commemorations)
- CRUD operations for custom sanctoral days
- Precedence resolution service

### FreiService.Web
Blazor Server web application providing a user-friendly interface for calculating and managing holy days.

## Features

- **🗓️ Complete Temporale**: All moveable feasts from Septuagesima through Trinity Sundays
- **📅 Sanctoral Calendar**: Pre-populated with TLH commemorations (apostles, evangelists, martyrs, feasts)
- **⚖️ Precedence Resolution**: Lutheran precedence rules for resolving Temporal/Sanctoral conflicts
- **🎨 Liturgical Colors**: White, Red, Violet, Black, Green based on season and observance
- **📊 Accurate Algorithm**: Meeus/Jones/Butcher computus, accurate for all Gregorian calendar years (1583+)
- **✝️ TLH Tradition**: Faithful to The Lutheran Hymnal (1941) liturgical calendar
- **🧪 Well-Tested**: 82+ tests covering calculations, precedence, and edge cases
- **💾 Database Support**: SQLite/SQL Server storage for sanctoral days and custom additions

## Liturgical Calendar System

### Temporale (Moveable Cycle)

The TemporalService calculates all dates based on Easter:

| Period | Observances |
|--------|-------------|
| **Pre-Lent** | Septuagesima, Sexagesima, Quinquagesima |
| **Lent** | Ash Wednesday, Invocabit through Judica (Lent 1-5) |
| **Holy Week** | Palm Sunday, Maundy Thursday, Good Friday, Holy Saturday |
| **Easter** | Easter Sunday, Quasimodogeniti through Exaudi (Easter 1-6) |
| **Post-Easter** | Ascension (Thursday), Pentecost, Whit-Monday, Whit-Tuesday |
| **Trinity** | Trinity Sunday, Trinity 1-27 (variable based on Easter date) |
| **Advent** | Advent 1-4 (4 Sundays before Christmas) |
| **Christmas** | Christmas Day, Christmas 1-2 |
| **Epiphany** | Epiphany Day, Epiphany 1-6, Transfiguration |

### Sanctorale (Fixed Commemorations)

Pre-populated with The Lutheran Hymnal (TLH) calendar:

| Date | Commemoration | Rank |
|------|---------------|------|
| Nov 30 | St. Andrew, Apostle | Apostle |
| Dec 21 | St. Thomas, Apostle | Apostle |
| Dec 25 | The Nativity of Our Lord | Principal Feast |
| Dec 26 | St. Stephen, Martyr | Lesser Feast |
| Dec 27 | St. John, Apostle and Evangelist | Apostle |
| Jan 1 | Circumcision and Name of Jesus | Feast |
| Jan 6 | The Epiphany of Our Lord | Principal Feast |
| Jan 18 | The Confession of St. Peter | Lesser Feast |
| Jan 25 | The Conversion of St. Paul | Apostle |
| Feb 2 | The Purification of Mary (Candlemas) | Feast |
| Feb 24 | St. Matthias, Apostle | Apostle |
| Mar 25 | The Annunciation | Feast |
| ... | *(30+ total commemorations)* | ... |
| Oct 31 | Reformation Day | Feast |
| Nov 1 | All Saints' Day | Feast |

### Precedence Rules

The PrecedenceService applies Lutheran (TLH) precedence rules:

1. **Principal Feasts** always take precedence (Christmas, Easter, Epiphany, Ascension, Pentecost, Trinity)
2. **Holy Week** temporal always wins, no commemorations
3. **Sundays** generally take precedence over saints' days
4. **Feast-rank sanctoral** days may be observed even on Sundays (e.g., Reformation)
5. **Weekdays with Apostle/Feast** sanctoral days: sanctoral wins
6. **Lesser feasts** on weekdays: sanctoral wins
7. **Commemorations**: Displaced observances noted but not celebrated

### Liturgical Colors

Colors follow TLH tradition:
- **White**: Christmas, Epiphany, Easter, Trinity Sunday, Christ-centered feasts
- **Red**: Pentecost, Reformation, Apostles/Martyrs
- **Violet**: Advent, Lent, Septuagesima
- **Black**: Good Friday
- **Green**: Epiphany season (after Epiphany 1), Trinity season

## Quick Start

### Running the Web Application

#### Option 1: Using Visual Studio

**Prerequisites:**
- Visual Studio 2022 (version 17.8 or later)
- .NET 10.0 SDK

**Steps:**
1. Open the solution file `FreiService.slnx` in Visual Studio
2. In the Solution Explorer, right-click on the `FreiService.Web` project and select "Set as Startup Project"
3. Press `F5` or click the "Start Debugging" button to run the application
4. The web application will launch in your default browser at:
   - HTTPS: `https://localhost:7119`
   - HTTP: `http://localhost:5174`

**Tips:**
- You can change the launch profile (http/https) in the project properties under Debug > Launch Profiles
- The SQLite database file (`holydays.db`) will be created automatically in the `FreiService.Web` project directory

#### Option 2: Using .NET CLI

```bash
cd src/FreiService.Web
dotnet run
```

Then navigate to `http://localhost:5174` or `https://localhost:7119` in your browser.

### Using the Complete Liturgical Calendar System

```csharp
using FreiService.Computus;
using FreiService.Data;
using FreiService.Data.Repositories;
using FreiService.Data.Services;
using Microsoft.EntityFrameworkCore;

// Setup services
var options = new DbContextOptionsBuilder<HolyDaysContext>()
    .UseSqlite("Data Source=calendar.db")
    .Options;

var context = new HolyDaysContext(options);
await context.Database.EnsureCreatedAsync();

// Initialize services
var sanctoralRepo = new SanctoralDaysRepository(context);
var sanctoralService = new SanctoralService(sanctoralRepo);
var temporalService = new TemporalService();
var precedenceService = new PrecedenceService(temporalService, sanctoralService);

// Initialize TLH sanctoral calendar
await sanctoralService.InitializeDefaultSanctoralCalendarAsync();

// Get complete temporal information for a date
var date = new DateTime(2024, 3, 31); // Easter Sunday
var temporalDay = temporalService.GetTemporalDay(date);

Console.WriteLine($"Date: {temporalDay.Date:dddd, MMMM d, yyyy}");
Console.WriteLine($"Season: {temporalDay.Season}");
Console.WriteLine($"Day Name: {temporalDay.DayName}");
Console.WriteLine($"Color: {temporalDay.LiturgicalColor}");
Console.WriteLine($"Rank: {temporalDay.Rank}");

// Resolve precedence for a date with both temporal and sanctoral
var christmas = new DateTime(2024, 12, 25);
var resolved = await precedenceService.ResolveDateAsync(christmas);

Console.WriteLine($"\nDate: {resolved.Date:dddd, MMMM d, yyyy}");
Console.WriteLine($"Primary Observance: {resolved.Primary.Name}");
Console.WriteLine($"Season: {resolved.Season}");
Console.WriteLine($"Color: {resolved.LiturgicalColor}");

if (resolved.Commemorations.Any())
{
    Console.WriteLine("Commemorations:");
    foreach (var comm in resolved.Commemorations)
    {
        Console.WriteLine($"  - {comm.Name}");
    }
}

// Calculate all moveable feasts for a year
var feasts = temporalService.CalculateAllMoveableFeasts(2024);
foreach (var (name, feastDate) in feasts.Take(5))
{
    Console.WriteLine($"{name}: {feastDate:dddd, MMMM d}");
}

// Get a range of resolved days (e.g., Holy Week)
var holyWeekStart = new DateTime(2024, 3, 24);
var holyWeekEnd = new DateTime(2024, 3, 31);
var holyWeek = await precedenceService.ResolveDateRangeAsync(holyWeekStart, holyWeekEnd);

foreach (var day in holyWeek)
{
    Console.WriteLine($"{day.Date:MM/dd} {day.Primary.Name} ({day.LiturgicalColor})");
}
```

### Using the Computus Service Library

```csharp
using FreiService.Computus;

var service = new ComputusService();

// Calculate all holy days for a year
var holyDays = service.CalculateAllHolyDays(2024);

foreach (var (name, date) in holyDays)
{
    Console.WriteLine($"{name}: {date:dddd, MMMM d, yyyy}");
}

// Output:
// Easter Sunday: Sunday, March 31, 2024
// Ash Wednesday: Wednesday, February 14, 2024
// Pentecost: Sunday, May 19, 2024
// Trinity Sunday: Sunday, May 26, 2024
// Annunciation of Mary: Monday, March 25, 2024
```

### Using the TemporalService

```csharp
using FreiService.Computus;

var temporalService = new TemporalService();

// Get all moveable feasts for a year
var feasts = temporalService.CalculateAllMoveableFeasts(2024);
Console.WriteLine($"Septuagesima: {feasts["Septuagesima Sunday"]:MMMM d}");
Console.WriteLine($"Ash Wednesday: {feasts["Ash Wednesday"]:MMMM d}");
Console.WriteLine($"Easter: {feasts["Easter Sunday"]:MMMM d}");

// Calculate Advent
var advent1 = temporalService.CalculateAdvent1(2024);
Console.WriteLine($"Advent 1: {advent1:dddd, MMMM d}");

// Get season information
var trinitySundays = temporalService.GetTrinitySundayCount(2024);
var epiphanySundays = temporalService.GetEpiphanySundayCount(2024);
Console.WriteLine($"Sundays after Trinity: {trinitySundays}");
Console.WriteLine($"Sundays after Epiphany: {epiphanySundays}");

// Get complete day information
var day = temporalService.GetTemporalDay(new DateTime(2024, 7, 14));
Console.WriteLine($"{day.DayName}: {day.Season}, {day.LiturgicalColor}");
```

### Using the Data Service

```csharp
using FreiService.Data;
using FreiService.Data.Services;
using FreiService.Computus;
using Microsoft.EntityFrameworkCore;

// Configure services
var options = new DbContextOptionsBuilder<HolyDaysContext>()
    .UseSqlite("Data Source=holydays.db")
    .Options;

var context = new HolyDaysContext(options);
var repository = new HolyDaysRepository(context);
var computusService = new ComputusService();
var holyDaysService = new HolyDaysService(computusService, repository);

// Calculate and save holy days for a year
await holyDaysService.SaveHolyDaysForYearAsync(2024);

// Retrieve saved holy days
var savedDays = await holyDaysService.GetHolyDaysByYearAsync(2024);
```

## Web Interface

The web application provides an intuitive interface for managing holy days:

### Home Page
![Home Page](https://github.com/user-attachments/assets/418da672-237d-4bb4-8b24-02081e937bf1)

### Dry Run Calculation
![Dry Run](https://github.com/user-attachments/assets/268c92dd-e4c9-487c-8d3c-f7c961a394c9)

### Saved to Database
![Saved Data](https://github.com/user-attachments/assets/920de13c-822d-46d2-8ebe-fd860860a993)

### Features of the Web Interface:
1. **Calculate Holy Days**: Enter any year (1583-9999) and preview the calculated dates
2. **Dry Run Preview**: Review dates before saving to ensure accuracy
3. **Save to Database**: Persist calculated dates for future reference
4. **View Saved Data**: Browse holy days saved in the database by year
5. **Manage Data**: Delete or overwrite existing year data as needed
6. **Validation**: Automatic detection of duplicate years with confirmation prompts

## Building

```bash
dotnet build
```

## Testing

```bash
dotnet test
```

All 36 tests validate:
- Easter calculation accuracy for known dates
- Ash Wednesday always falls on Wednesday
- Pentecost and Trinity Sunday always fall on Sunday
- Annunciation is always March 25
- All holy days are calculated correctly for multiple years

## Demo Application

A console demo application is available:

```bash
cd examples/FreiService.Computus.Demo
dotnet run
```

The demo shows:
- Single year Easter calculation
- Range calculations for multiple years
- Example SQL INSERT statements for database seeding

## Algorithm Details

The service uses the **Meeus/Jones/Butcher algorithm** for Easter calculation:

1. Works for any year in the Gregorian calendar (1583 and later)
2. Computationally efficient
3. Verified against historical Easter dates
4. Always returns a Sunday in March or April

### Related Holy Days:
- **Ash Wednesday**: Calculated as 46 days before Easter
- **Pentecost**: Calculated as 49 days after Easter (50th day counting Easter as day 1)
- **Trinity Sunday**: Calculated as 56 days after Easter (first Sunday after Pentecost)
- **Annunciation of Mary**: Fixed date of March 25

## Project Structure

```
FreiService/
├── src/
│   ├── FreiService.Computus/       # Core calculation library
│   ├── FreiService.Data/            # Data access layer
│   └── FreiService.Web/             # Blazor Server web app
├── tests/
│   └── FreiService.Computus.Tests/  # Unit tests
└── examples/
    └── FreiService.Computus.Demo/   # Console demo app
```

## Known Easter Dates (for verification)

| Year | Easter Date | Ash Wednesday | Pentecost | Trinity Sunday |
|------|-------------|---------------|-----------|----------------|
| 2024 | March 31    | February 14   | May 19    | May 26         |
| 2025 | April 20    | March 5       | June 8    | June 15        |
| 2026 | April 5     | February 18   | May 24    | May 31         |
| 2027 | March 28    | February 10   | May 16    | May 23         |
| 2028 | April 16    | March 1       | June 4    | June 11        |
| 2029 | April 1     | February 14   | May 20    | May 27         |
| 2030 | April 21    | March 6       | June 9    | June 16        |

## Database Schema

The SQLite database stores holy days with the following schema:

```sql
CREATE TABLE HolyDays (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Year INTEGER NOT NULL,
    Date TEXT NOT NULL,
    Description TEXT,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    UNIQUE(Year, Name)
);
```

## License

This project is licensed under the GNU General Public License v3.0 - see the LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
