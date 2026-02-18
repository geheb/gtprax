namespace GtPrax.Infrastructure.Repositories;

using GtPrax.Application.Converter;
using GtPrax.Application.Models;
using GtPrax.Application.Repositories;
using GtPrax.Infrastructure.Database;
using GtPrax.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

internal sealed class WaitlistRepository : IWaitlistRepository
{
    private readonly AppDbContext _dbContext;

    public WaitlistRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WaitlistDto[]> GetAll(CancellationToken cancellationToken)
    {
        var users = await _dbContext.Waitlists
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .Select(e => new { entity = e, count = e.WaitlistPatients!.Count })
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return users.Select(e => e.entity.ToDto(e.count)).ToArray();
    }

    public async Task<WaitlistDto?> Find(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return null;
        }

        var entity = await _dbContext.Waitlists
             .AsNoTracking()
             .Select(e => new { entity = e, count = e.WaitlistPatients!.Count })
             .FirstOrDefaultAsync(e => e.entity.Id == id, cancellationToken);

        return entity?.entity.ToDto(entity.count);
    }

    public async Task<bool> Create(string name, CancellationToken cancellationToken)
    {
        var entity = new Waitlist
        {
            Id = _dbContext.GeneratePk(),
            Name = name
        };

        _dbContext.Waitlists.Add(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<WaitlistPatientDto?> FindPatient(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return null;
        }

        var entity = await _dbContext.WaitlistPatients
             .AsNoTracking()
             .Include(e => e.User)
             .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entity?.ToDto(dc);
    }

    public async Task<WaitlistPatientDto[]> GetPatients(Guid? waitlistId, string? searchKey, CancellationToken cancellationToken)
    {
        var query = _dbContext.WaitlistPatients
            .AsNoTracking()
            .Include(e => e.User)
            .AsQueryable();

        if (waitlistId.HasValue)
        {
            query = query.Where(e => e.WaitlistId == waitlistId);
        }

        searchKey = searchKey?.Trim();
        if (!string.IsNullOrEmpty(searchKey))
        {
            // check for birthday
            const string datePattern = "(\\d{1,2}\\.\\d{1,2}\\.\\d{4})";
            var match = Regex.Match(searchKey, datePattern);
            if (match.Success)
            {
                var splitDate = match.Groups[0].Value.Split('.');
                var tempDate = splitDate[0].PadLeft(2, '0') + '.' + splitDate[1].PadLeft(2, '0') + '.' + splitDate[2];
                if (DateOnly.TryParseExact(tempDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    searchKey = searchKey.Remove(match.Index, match.Length);
                    query = query.Where(e => e.Birthday == date);
                }
            }
        }

        searchKey = searchKey?.Trim();
        if (!string.IsNullOrEmpty(searchKey))
        {
            // check for phone number
            const string numberPattern = "(\\d+)";
            var match = Regex.Match(searchKey, numberPattern);
            if (match.Success)
            {
                var number = "%" + match.Value + "%";
                searchKey = searchKey.Remove(match.Index, match.Length);
                query = query.Where(e => EF.Functions.Like(e.PhoneNumber!, number));
            }
        }

        searchKey = searchKey?.Trim();
        if (!string.IsNullOrEmpty(searchKey))
        {
            foreach (var q in searchKey.Split(' '))
            {
                // see also SqliteConnectionInterceptor
                query = query.Where(e => Regex.IsMatch(e.Name!, q));
            }
        }

        var entities = await query.OrderBy(e => e.Created).ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => e.ToDto(dc)).ToArray();
    }

    public async Task<bool> Update(WaitlistPatientDto dto, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.WaitlistPatients
             .FirstOrDefaultAsync(e => e.Id == dto.Id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        if (!entity.FromDto(dto))
        {
            return true;
        }

        entity.Updated = DateTimeOffset.UtcNow;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeletePatient(Guid id, Guid waitlistId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.WaitlistPatients
             .FirstOrDefaultAsync(e => e.Id == id && e.WaitlistId == waitlistId, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _dbContext.WaitlistPatients.Remove(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> MovePatient(Guid id, Guid waitlistId, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.WaitlistPatients
             .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        entity.WaitlistId = waitlistId;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> CreatePatient(WaitlistPatientDto dto, CancellationToken cancellationToken)
    {
        var entity = new WaitlistPatient();
        entity.FromDto(dto);
        entity.Id = _dbContext.GeneratePk();

        entity.Created = dto.Created ?? DateTimeOffset.UtcNow;

        _dbContext.WaitlistPatients.Add(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
