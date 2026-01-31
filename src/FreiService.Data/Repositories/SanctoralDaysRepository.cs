using FreiService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FreiService.Data.Repositories;

/// <summary>
/// Repository implementation for managing SanctoralDay entities.
/// </summary>
public class SanctoralDaysRepository : ISanctoralDaysRepository
{
    private readonly HolyDaysContext _context;

    public SanctoralDaysRepository(HolyDaysContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<SanctoralDay>> GetByDateAsync(int month, int day)
    {
        return await _context.SanctoralDays
            .Where(s => s.Month == month && s.Day == day)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<SanctoralDay?> GetByIdAsync(Guid id)
    {
        return await _context.SanctoralDays.FindAsync(id);
    }

    public async Task<List<SanctoralDay>> GetAllAsync()
    {
        return await _context.SanctoralDays
            .OrderBy(s => s.Month)
            .ThenBy(s => s.Day)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<SanctoralDay>> GetCustomAsync()
    {
        return await _context.SanctoralDays
            .Where(s => s.IsCustom)
            .OrderBy(s => s.Month)
            .ThenBy(s => s.Day)
            .ToListAsync();
    }

    public async Task<SanctoralDay> AddAsync(SanctoralDay sanctoralDay)
    {
        sanctoralDay.CreatedAt = DateTime.UtcNow;
        sanctoralDay.UpdatedAt = DateTime.UtcNow;
        
        _context.SanctoralDays.Add(sanctoralDay);
        await _context.SaveChangesAsync();
        
        return sanctoralDay;
    }

    public async Task<SanctoralDay> UpdateAsync(SanctoralDay sanctoralDay)
    {
        sanctoralDay.UpdatedAt = DateTime.UtcNow;
        
        _context.SanctoralDays.Update(sanctoralDay);
        await _context.SaveChangesAsync();
        
        return sanctoralDay;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sanctoralDay = await GetByIdAsync(id);
        if (sanctoralDay == null)
        {
            return false;
        }

        _context.SanctoralDays.Remove(sanctoralDay);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(int month, int day, string name)
    {
        return await _context.SanctoralDays
            .AnyAsync(s => s.Month == month && s.Day == day && s.Name == name);
    }
}
