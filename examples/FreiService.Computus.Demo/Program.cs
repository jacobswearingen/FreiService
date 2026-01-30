using FreiService.Computus;

Console.WriteLine("=== FreiService Computus Demo ===\n");

var service = new ComputusService();

// Demonstrate single year calculation
Console.WriteLine("Single Year Calculation:");
var currentYear = DateTime.Now.Year;
var easterDate = service.CalculateEaster(currentYear);
Console.WriteLine($"Easter {currentYear}: {easterDate:dddd, MMMM d, yyyy}");
Console.WriteLine();

// Demonstrate range calculation
Console.WriteLine("Range Calculation (Next 10 Years):");
var easterDates = service.CalculateEasterRange(currentYear, currentYear + 9);

foreach (var entry in easterDates.OrderBy(x => x.Key))
{
    Console.WriteLine($"  {entry.Key}: {entry.Value:dddd, MMMM d, yyyy}");
}
Console.WriteLine();

// Show a practical example for database seeding
Console.WriteLine("Example: Dates suitable for database seeding");
Console.WriteLine("(This could be used to populate a holidays table annually)");
Console.WriteLine();

var futureEasterDates = service.CalculateEasterRange(currentYear, currentYear + 5);
foreach (var entry in futureEasterDates.OrderBy(x => x.Key))
{
    Console.WriteLine($"INSERT INTO Holidays (Name, Date, Year) VALUES ('Easter Sunday', '{entry.Value:yyyy-MM-dd}', {entry.Key});");
}
