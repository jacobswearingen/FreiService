using FreiService.Computus;

namespace FreiService.Data.Services;

/// <summary>
/// Service for resolving conflicts between Temporal and Sanctoral calendars
/// according to Lutheran (TLH) precedence rules.
/// </summary>
public class PrecedenceService : IPrecedenceService
{
    private readonly ITemporalService _temporalService;
    private readonly ISanctoralService _sanctoralService;

    public PrecedenceService(ITemporalService temporalService, ISanctoralService sanctoralService)
    {
        _temporalService = temporalService ?? throw new ArgumentNullException(nameof(temporalService));
        _sanctoralService = sanctoralService ?? throw new ArgumentNullException(nameof(sanctoralService));
    }

    public async Task<ResolvedDay> ResolveDateAsync(DateTime date)
    {
        // Get both temporal and sanctoral information
        var temporal = _temporalService.GetTemporalDay(date);
        var sanctoralDays = await _sanctoralService.GetSanctoralDaysAsync(date);

        // If no sanctoral entries, temporal wins by default
        if (sanctoralDays == null || !sanctoralDays.Any())
        {
            return CreateResolvedDay(date, temporal, null, temporal.Season);
        }

        // Apply precedence rules
        var primary = DeterminePrimary(temporal, sanctoralDays, out var commemorations);

        return new ResolvedDay
        {
            Date = date,
            Primary = primary,
            Commemorations = commemorations,
            LiturgicalColor = primary.LiturgicalColor,
            Season = temporal.Season,
            Notes = GenerateNotes(primary, commemorations)
        };
    }

    public async Task<List<ResolvedDay>> ResolveDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var resolvedDays = new List<ResolvedDay>();
        var currentDate = startDate.Date;

        while (currentDate <= endDate.Date)
        {
            var resolved = await ResolveDateAsync(currentDate);
            resolvedDays.Add(resolved);
            currentDate = currentDate.AddDays(1);
        }

        return resolvedDays;
    }

    public async Task<List<ResolvedDay>> GetUpcomingFeastsAsync(DateTime fromDate, int count)
    {
        var feasts = new List<ResolvedDay>();
        var currentDate = fromDate.Date;
        var maxDays = 365; // Prevent infinite loop
        var daysChecked = 0;

        while (feasts.Count < count && daysChecked < maxDays)
        {
            var resolved = await ResolveDateAsync(currentDate);
            
            // Include if it's a feast or higher rank
            if (resolved.Primary.Rank >= Rank.Feast)
            {
                feasts.Add(resolved);
            }

            currentDate = currentDate.AddDays(1);
            daysChecked++;
        }

        return feasts.Take(count).ToList();
    }

    private CalendarDay DeterminePrimary(TemporalDay temporal, List<Models.SanctoralDay> sanctoralDays, 
        out List<CalendarDay> commemorations)
    {
        commemorations = new List<CalendarDay>();

        // Convert sanctoral days to CalendarDays
        var sanctoralCalendarDays = sanctoralDays
            .Select(s => new CalendarDay
            {
                Name = s.Name,
                Source = CalendarSource.Sanctoral,
                Rank = s.Rank,
                LiturgicalColor = s.LiturgicalColor,
                CollectId = s.ProperId,
                ReadingsId = s.ProperId
            })
            .ToList();

        // Convert temporal to CalendarDay
        var temporalCalendarDay = new CalendarDay
        {
            Name = temporal.DayName ?? $"{temporal.Season} - {temporal.Date.DayOfWeek}",
            Source = CalendarSource.Temporal,
            Rank = temporal.Rank,
            LiturgicalColor = temporal.LiturgicalColor
        };

        // Rule 1: Holy Week - temporal always wins, no commemorations
        if (temporal.Season == Season.HolyWeek)
        {
            return temporalCalendarDay;
        }

        // Rule 2: Principal Feasts always win
        var temporalIsPrincipal = temporal.Rank == Rank.PrincipalFeast;
        var sanctoralPrincipals = sanctoralCalendarDays.Where(s => s.Rank == Rank.PrincipalFeast).ToList();

        if (temporalIsPrincipal && sanctoralPrincipals.Any())
        {
            // Both are principal - temporal wins in TLH tradition
            commemorations.AddRange(sanctoralPrincipals);
            commemorations.AddRange(sanctoralCalendarDays.Except(sanctoralPrincipals));
            return temporalCalendarDay;
        }

        if (temporalIsPrincipal)
        {
            // Temporal is principal, sanctoral are not
            commemorations.AddRange(sanctoralCalendarDays);
            return temporalCalendarDay;
        }

        if (sanctoralPrincipals.Any())
        {
            // Sanctoral has principal feast(s), temporal does not
            var primary = sanctoralPrincipals.First();
            commemorations.Add(temporalCalendarDay);
            commemorations.AddRange(sanctoralPrincipals.Skip(1));
            commemorations.AddRange(sanctoralCalendarDays.Except(sanctoralPrincipals));
            return primary;
        }

        // Rule 3: Sundays generally take precedence over saints' days
        if (temporal.DayType == DayType.Sunday)
        {
            // Exception: High-rank sanctoral days (Feast or Apostle) may be observed
            var highRankSanctoral = sanctoralCalendarDays
                .Where(s => s.Rank >= Rank.Feast || s.Rank == Rank.Apostle)
                .ToList();

            if (highRankSanctoral.Any())
            {
                // In TLH tradition, typically the Sunday is still observed
                // but the saint is commemorated
                commemorations.AddRange(sanctoralCalendarDays);
                return temporalCalendarDay;
            }

            commemorations.AddRange(sanctoralCalendarDays);
            return temporalCalendarDay;
        }

        // Rule 4: On weekdays, sanctoral wins if Feast rank or higher
        if (temporal.DayType == DayType.Weekday)
        {
            var feastOrHigher = sanctoralCalendarDays
                .Where(s => s.Rank >= Rank.Feast || s.Rank == Rank.Apostle || s.Rank == Rank.Evangelist)
                .OrderByDescending(s => s.Rank)
                .ToList();

            if (feastOrHigher.Any())
            {
                var primary = feastOrHigher.First();
                commemorations.Add(temporalCalendarDay);
                commemorations.AddRange(feastOrHigher.Skip(1));
                commemorations.AddRange(sanctoralCalendarDays.Except(feastOrHigher));
                return primary;
            }

            // Lesser feasts on weekdays: sanctoral wins
            if (sanctoralCalendarDays.Any())
            {
                var primary = sanctoralCalendarDays
                    .OrderByDescending(s => s.Rank)
                    .First();
                commemorations.Add(temporalCalendarDay);
                commemorations.AddRange(sanctoralCalendarDays.Except(new[] { primary }));
                return primary;
            }
        }

        // Default: temporal wins
        commemorations.AddRange(sanctoralCalendarDays);
        return temporalCalendarDay;
    }

    private ResolvedDay CreateResolvedDay(DateTime date, TemporalDay temporal, 
        Models.SanctoralDay? sanctoral, Season season)
    {
        var primary = new CalendarDay
        {
            Name = temporal.DayName ?? $"{temporal.Season} - {temporal.Date.DayOfWeek}",
            Source = CalendarSource.Temporal,
            Rank = temporal.Rank,
            LiturgicalColor = temporal.LiturgicalColor
        };

        return new ResolvedDay
        {
            Date = date,
            Primary = primary,
            Commemorations = new List<CalendarDay>(),
            LiturgicalColor = primary.LiturgicalColor,
            Season = season
        };
    }

    private string? GenerateNotes(CalendarDay primary, List<CalendarDay> commemorations)
    {
        if (!commemorations.Any())
            return null;

        var commemorationNames = commemorations
            .Select(c => c.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n));

        if (!commemorationNames.Any())
            return null;

        if (commemorations.Count == 1)
        {
            return $"{commemorations[0].Name} is commemorated";
        }

        return $"Also commemorated: {string.Join(", ", commemorationNames)}";
    }
}
