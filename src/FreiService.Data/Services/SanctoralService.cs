using FreiService.Computus;
using FreiService.Data.Models;
using FreiService.Data.Repositories;

namespace FreiService.Data.Services;

/// <summary>
/// Service for managing the Sanctorale (fixed commemorations) in The Lutheran Hymnal tradition.
/// </summary>
public class SanctoralService : ISanctoralService
{
    private readonly ISanctoralDaysRepository _repository;

    public SanctoralService(ISanctoralDaysRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<List<SanctoralDay>> GetSanctoralDaysAsync(DateTime date)
    {
        var days = await _repository.GetByDateAsync(date.Month, date.Day);
        
        // Handle moveable days (like Thanksgiving)
        foreach (var day in days.Where(d => d.IsMoveable).ToList())
        {
            if (!IsCorrectMoveableDate(day, date))
            {
                days.Remove(day);
            }
        }

        return days;
    }

    public async Task<SanctoralDay?> GetSanctoralDayByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<SanctoralDay>> ListAllSanctoralDaysAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<List<SanctoralDay>> ListCustomSanctoralDaysAsync()
    {
        return await _repository.GetCustomAsync();
    }

    public async Task<SanctoralDay> AddSanctoralDayAsync(SanctoralDay sanctoralDay)
    {
        sanctoralDay.IsCustom = true;
        return await _repository.AddAsync(sanctoralDay);
    }

    public async Task<SanctoralDay> UpdateSanctoralDayAsync(Guid id, SanctoralDay sanctoralDay)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new InvalidOperationException($"Sanctoral day with ID {id} not found.");
        }

        sanctoralDay.Id = id;
        sanctoralDay.CreatedAt = existing.CreatedAt;
        return await _repository.UpdateAsync(sanctoralDay);
    }

    public async Task<bool> DeleteSanctoralDayAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<int> InitializeDefaultSanctoralCalendarAsync()
    {
        var defaultCalendar = GetDefaultTLHSanctoralCalendar();
        int addedCount = 0;

        foreach (var day in defaultCalendar)
        {
            // Check if it already exists
            var exists = await _repository.ExistsAsync(day.Month, day.Day, day.Name);
            if (!exists)
            {
                await _repository.AddAsync(day);
                addedCount++;
            }
        }

        return addedCount;
    }

    private bool IsCorrectMoveableDate(SanctoralDay day, DateTime date)
    {
        if (day.MoveableRule == null)
            return true;

        // Handle Thanksgiving - 4th Thursday of November
        if (day.MoveableRule.Contains("fourth Thursday", StringComparison.OrdinalIgnoreCase))
        {
            if (date.Month != 11 || date.DayOfWeek != DayOfWeek.Thursday)
                return false;

            // Find the 4th Thursday
            var firstDay = new DateTime(date.Year, 11, 1);
            var firstThursday = firstDay;
            while (firstThursday.DayOfWeek != DayOfWeek.Thursday)
            {
                firstThursday = firstThursday.AddDays(1);
            }
            
            var fourthThursday = firstThursday.AddDays(21); // 3 weeks later
            return date.Date == fourthThursday.Date;
        }

        return true;
    }

    private List<SanctoralDay> GetDefaultTLHSanctoralCalendar()
    {
        return new List<SanctoralDay>
        {
            new SanctoralDay
            {
                Month = 11,
                Day = 30,
                Name = "St. Andrew, Apostle",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 12,
                Day = 21,
                Name = "St. Thomas, Apostle",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 12,
                Day = 25,
                Name = "The Nativity of Our Lord",
                Rank = Rank.PrincipalFeast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 12,
                Day = 26,
                Name = "St. Stephen, Martyr",
                Rank = Rank.LesserFeast,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 12,
                Day = 27,
                Name = "St. John, Apostle and Evangelist",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 12,
                Day = 28,
                Name = "The Holy Innocents",
                Rank = Rank.LesserFeast,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 1,
                Day = 1,
                Name = "Circumcision and Name of Jesus",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 1,
                Day = 6,
                Name = "The Epiphany of Our Lord",
                Rank = Rank.PrincipalFeast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 1,
                Day = 18,
                Name = "The Confession of St. Peter",
                Rank = Rank.LesserFeast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 1,
                Day = 25,
                Name = "The Conversion of St. Paul",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 2,
                Day = 2,
                Name = "The Purification of Mary (Candlemas)",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 2,
                Day = 24,
                Name = "St. Matthias, Apostle",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 3,
                Day = 25,
                Name = "The Annunciation",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 4,
                Day = 25,
                Name = "St. Mark, Evangelist",
                Rank = Rank.Evangelist,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 5,
                Day = 1,
                Name = "St. Philip and St. James, Apostles",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 6,
                Day = 11,
                Name = "St. Barnabas, Apostle",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 6,
                Day = 24,
                Name = "The Nativity of St. John the Baptist",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 6,
                Day = 25,
                Name = "Presentation of the Augsburg Confession",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 6,
                Day = 29,
                Name = "St. Peter and St. Paul, Apostles",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 7,
                Day = 22,
                Name = "St. Mary Magdalene",
                Rank = Rank.LesserFeast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 7,
                Day = 25,
                Name = "St. James the Elder, Apostle",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 8,
                Day = 10,
                Name = "St. Lawrence, Martyr",
                Rank = Rank.LesserFeast,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 8,
                Day = 15,
                Name = "St. Mary, Mother of Our Lord",
                Rank = Rank.LesserFeast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 8,
                Day = 24,
                Name = "St. Bartholomew, Apostle",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 9,
                Day = 14,
                Name = "Holy Cross Day",
                Rank = Rank.LesserFeast,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 9,
                Day = 21,
                Name = "St. Matthew, Apostle and Evangelist",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 9,
                Day = 29,
                Name = "St. Michael and All Angels",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 10,
                Day = 18,
                Name = "St. Luke, Evangelist",
                Rank = Rank.Evangelist,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 10,
                Day = 28,
                Name = "St. Simon and St. Jude, Apostles",
                Rank = Rank.Apostle,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 10,
                Day = 31,
                Name = "Reformation Day",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.Red,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 11,
                Day = 1,
                Name = "All Saints' Day",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White,
                IsCustom = false
            },
            new SanctoralDay
            {
                Month = 11,
                Day = 1, // Placeholder - actual date varies
                Name = "Thanksgiving Day (USA)",
                Rank = Rank.Feast,
                LiturgicalColor = LiturgicalColor.White,
                IsMoveable = true,
                MoveableRule = "fourth Thursday of November",
                IsCustom = false
            }
        };
    }
}
