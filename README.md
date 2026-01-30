# FreiService - Holy Days Management System

A comprehensive C# application for calculating and managing Christian liturgical calendar dates using the Meeus/Jones/Butcher computus algorithm.

## Overview

FreiService provides a complete solution for calculating Easter and related movable feasts, storing them in a SQLite database, and managing them through an intuitive web interface. This is particularly useful for:
- Church calendar planning
- Religious holiday management
- Liturgical software development
- Database seeding for multi-year calendars

## Projects

### FreiService.Computus
Core library for calculating Easter dates and related holy days using the Meeus/Jones/Butcher algorithm.

### FreiService.Data
Data access layer with SQLite database support for storing and managing holy days.

### FreiService.Web
Blazor Server web application providing a user-friendly interface for calculating and managing holy days.

## Features

- **🗓️ Multiple Holy Days**: Calculate Easter Sunday, Ash Wednesday, Pentecost, Trinity Sunday, and Annunciation of Mary
- **👁️ Dry Run Preview**: Preview calculated holy days before committing to the database
- **💾 SQLite Storage**: Persistent storage with Entity Framework Core
- **🌐 Web Interface**: Modern, responsive Blazor Server UI
- **✅ Validation**: Prevents duplicate entries and provides overwrite options
- **📊 Accurate Algorithm**: Uses the Meeus/Jones/Butcher algorithm, accurate for all Gregorian calendar years (1583 onwards)
- **🧪 Well-Tested**: Comprehensive test suite with known dates from various years

## Supported Holy Days

| Holy Day | Calculation Method |
|----------|-------------------|
| **Easter Sunday** | Meeus/Jones/Butcher algorithm |
| **Ash Wednesday** | 46 days before Easter |
| **Pentecost** | 49 days after Easter (50th day inclusive) |
| **Trinity Sunday** | 56 days after Easter (first Sunday after Pentecost) |
| **Annunciation of Mary** | Fixed date: March 25 |

## Quick Start

### Running the Web Application

```bash
cd src/FreiService.Web
dotnet run
```

Then navigate to `http://localhost:5000` in your browser.

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
