using FreiService.Computus;
using FreiService.Data;
using FreiService.Data.Repositories;
using FreiService.Data.Services;
using Microsoft.EntityFrameworkCore;

var separator = new string('=', 80);

Console.WriteLine(separator);
Console.WriteLine("FreiService - TLH Liturgical Calendar System Demo");
Console.WriteLine(separator);
Console.WriteLine();

// Setup services
var options = new DbContextOptionsBuilder<HolyDaysContext>()
    .UseInMemoryDatabase("CalendarDemo")
    .Options;

var context = new HolyDaysContext(options);
var sanctoralRepo = new SanctoralDaysRepository(context);
var sanctoralService = new SanctoralService(sanctoralRepo);
var temporalService = new TemporalService();
var precedenceService = new PrecedenceService(temporalService, sanctoralService);

// Initialize the sanctoral calendar
Console.WriteLine("Initializing TLH Sanctoral Calendar...");
var count = await sanctoralService.InitializeDefaultSanctoralCalendarAsync();
Console.WriteLine($"Added {count} sanctoral days to the calendar");
Console.WriteLine();

// Demo 1: Show key dates for 2024
Console.WriteLine(separator);
Console.WriteLine("Key Liturgical Dates for 2024");
Console.WriteLine(separator);

var year = 2024;
var moveableFeasts = temporalService.CalculateAllMoveableFeasts(year);

Console.WriteLine("\nPre-Lent:");
Console.WriteLine($"  Septuagesima Sunday:  {moveableFeasts["Septuagesima Sunday"]:dddd, MMMM d}");
Console.WriteLine($"  Sexagesima Sunday:    {moveableFeasts["Sexagesima Sunday"]:dddd, MMMM d}");
Console.WriteLine($"  Quinquagesima Sunday: {moveableFeasts["Quinquagesima Sunday"]:dddd, MMMM d}");

Console.WriteLine("\nLent:");
Console.WriteLine($"  Ash Wednesday:        {moveableFeasts["Ash Wednesday"]:dddd, MMMM d}");
Console.WriteLine($"  Laetare (Lent 4):     {moveableFeasts["Laetare (Lent 4)"]:dddd, MMMM d}");

Console.WriteLine("\nHoly Week & Easter:");
Console.WriteLine($"  Palm Sunday:          {moveableFeasts["Palmarum (Palm Sunday)"]:dddd, MMMM d}");
Console.WriteLine($"  Good Friday:          {moveableFeasts["Good Friday"]:dddd, MMMM d}");
Console.WriteLine($"  Easter Sunday:        {moveableFeasts["Easter Sunday"]:dddd, MMMM d}");
Console.WriteLine($"  Ascension:            {moveableFeasts["Ascension"]:dddd, MMMM d}");
Console.WriteLine($"  Pentecost:            {moveableFeasts["Pentecost (Whitsunday)"]:dddd, MMMM d}");
Console.WriteLine($"  Trinity Sunday:       {moveableFeasts["Trinity Sunday"]:dddd, MMMM d}");

var advent1 = temporalService.CalculateAdvent1(year);
Console.WriteLine($"\n  Advent 1:             {advent1:dddd, MMMM d}");
Console.WriteLine();

// Demo 2: Season and color information
Console.WriteLine(separator);
Console.WriteLine("Liturgical Seasons and Colors for Selected Dates");
Console.WriteLine(separator);
Console.WriteLine();

var testDates = new[]
{
    new DateTime(2024, 12, 1),   // Advent 1
    new DateTime(2024, 12, 25),  // Christmas
    new DateTime(2024, 3, 29),   // Good Friday
    new DateTime(2024, 3, 31),   // Easter
    new DateTime(2024, 5, 19),   // Pentecost
    new DateTime(2024, 7, 14),   // Trinity 7
    new DateTime(2024, 10, 31),  // Reformation Day
};

foreach (var date in testDates)
{
    var temporal = temporalService.GetTemporalDay(date);
    Console.WriteLine($"{date:yyyy-MM-dd} ({date:dddd}):");
    Console.WriteLine($"  Season: {temporal.Season}");
    Console.WriteLine($"  Day Name: {temporal.DayName ?? "Weekday"}");
    Console.WriteLine($"  Color: {temporal.LiturgicalColor}");
    Console.WriteLine();
}

// Demo 3: Precedence resolution examples
Console.WriteLine(separator);
Console.WriteLine("Precedence Resolution Examples");
Console.WriteLine(separator);
Console.WriteLine();

var precedenceDates = new[]
{
    new DateTime(2024, 12, 25),  // Christmas
    new DateTime(2024, 11, 30),  // St. Andrew
    new DateTime(2024, 10, 31),  // Reformation Day
    new DateTime(2024, 3, 25),   // Annunciation (during Lent)
};

foreach (var date in precedenceDates)
{
    var resolved = await precedenceService.ResolveDateAsync(date);
    Console.WriteLine($"{date:yyyy-MM-dd} ({date:dddd}):");
    Console.WriteLine($"  Primary: {resolved.Primary.Name} ({resolved.Primary.Source})");
    Console.WriteLine($"  Color: {resolved.LiturgicalColor}");
    if (resolved.Commemorations.Any())
    {
        Console.WriteLine($"  Commemorations: {string.Join(", ", resolved.Commemorations.Select(c => c.Name))}");
    }
    Console.WriteLine();
}

// Demo 4: Holy Week calendar
Console.WriteLine(separator);
Console.WriteLine("Holy Week 2024 Calendar");
Console.WriteLine(separator);
Console.WriteLine();

var holyWeekStart = new DateTime(2024, 3, 24);
var holyWeekEnd = new DateTime(2024, 3, 31);
var holyWeek = await precedenceService.ResolveDateRangeAsync(holyWeekStart, holyWeekEnd);

foreach (var day in holyWeek)
{
    Console.WriteLine($"{day.Date:yyyy-MM-dd} ({day.Date:dddd}) - {day.Primary.Name} ({day.LiturgicalColor})");
}
Console.WriteLine();

// Demo 5: Trinity season statistics
Console.WriteLine(separator);
Console.WriteLine("Trinity Season Statistics 2024-2027");
Console.WriteLine(separator);
Console.WriteLine();

for (int testYear = 2024; testYear <= 2027; testYear++)
{
    var trinityCount = temporalService.GetTrinitySundayCount(testYear);
    var epiphanyCount = temporalService.GetEpiphanySundayCount(testYear);
    Console.WriteLine($"{testYear}: {epiphanyCount} Sundays after Epiphany, {trinityCount} Sundays after Trinity");
}
Console.WriteLine();

Console.WriteLine(separator);
Console.WriteLine("Demo Complete!");
Console.WriteLine(separator);
