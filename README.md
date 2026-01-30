# FreiService - Computus Service

A C# service library for calculating Easter dates using the Meeus/Jones/Butcher computus algorithm.

## Overview

The Computus Service provides a reliable way to calculate the date of Easter Sunday for any year in the Gregorian calendar (1583 onwards). This is particularly useful for applications that need to:
- Calculate religious holidays
- Seed database tables with future dates
- Generate calendars with movable feast days

## Features

- **Accurate Algorithm**: Uses the Meeus/Jones/Butcher algorithm, which is accurate for all Gregorian calendar years
- **Range Calculations**: Calculate Easter dates for multiple years at once
- **Well-Tested**: Comprehensive test suite with known Easter dates from various years
- **Simple Interface**: Clean, easy-to-use API

## Usage

### Basic Usage

```csharp
using FreiService.Computus;

var service = new ComputusService();

// Calculate Easter for a single year
DateTime easter2024 = service.CalculateEaster(2024);
Console.WriteLine($"Easter 2024: {easter2024:yyyy-MM-dd}");
// Output: Easter 2024: 2024-03-31
```

### Range Calculations

```csharp
// Calculate Easter for multiple years
var easterDates = service.CalculateEasterRange(2024, 2030);

foreach (var entry in easterDates)
{
    Console.WriteLine($"Easter {entry.Key}: {entry.Value:yyyy-MM-dd}");
}
```

### Database Seeding Example

```csharp
// Example: Populate a database with future Easter dates
var service = new ComputusService();
var currentYear = DateTime.Now.Year;
var futureYears = 10;

var easterDates = service.CalculateEasterRange(currentYear, currentYear + futureYears);

// Insert dates into your database
foreach (var entry in easterDates)
{
    // await dbContext.Holidays.AddAsync(new Holiday
    // {
    //     Name = "Easter Sunday",
    //     Date = entry.Value,
    //     Year = entry.Key
    // });
}
// await dbContext.SaveChangesAsync();
```

## Building

```bash
dotnet build
```

## Testing

```bash
dotnet test
```

## Algorithm

The service uses the Meeus/Jones/Butcher algorithm, which is one of the most widely used methods for calculating Easter dates. The algorithm:

1. Works for any year in the Gregorian calendar (1583 and later)
2. Is computationally efficient
3. Has been verified against historical Easter dates
4. Always returns a Sunday in March or April

## Known Easter Dates (for verification)

| Year | Easter Date |
|------|-------------|
| 2024 | March 31    |
| 2025 | April 20    |
| 2026 | April 5     |
| 2027 | March 28    |
| 2028 | April 16    |
| 2029 | April 1     |
| 2030 | April 21    |

## License

This project is licensed under the GNU General Public License v3.0 - see the LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
