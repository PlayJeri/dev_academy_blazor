using System.Reflection.Metadata.Ecma335;
using cityBikeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace cityBikeApp.Services;

public interface IJourneysService
{
    Task<List<Journey>> GetJourneys(
        bool orderAscending = true,
        OrderBy orderBy = OrderBy.DepartureStationName,
        int offset = 0, int limit = 20
    );
}

public class JourneysService : IJourneysService
{
    private CitybikeContext _context;

    public JourneysService(CitybikeContext context)
    {
        _context = context;
    }

    public async Task<List<Journey>> GetJourneys(
        bool orderAscending = true,
        OrderBy orderBy = OrderBy.DepartureStationName,
        int offset = 0,
        int limit = 20
        )
    {
        IQueryable<Journey> query = _context.Journeys
            .Include(j => j.DepartureStation)
            .Include(j => j.ReturnStation);

        switch (orderBy)
        {
            case OrderBy.ReturnStationName:
                query = orderAscending == true
                    ? query.OrderBy(j => j.ReturnStation.StationName)
                    : query.OrderByDescending(j => j.ReturnStation.StationName);
                break;
            case OrderBy.DepartureStationName:
                query = orderAscending == true
                    ? query.OrderBy(j => j.DepartureStation.StationName)
                    : query.OrderByDescending(j => j.DepartureStation.StationName);
                break;
            case OrderBy.Duration:
                query = orderAscending == true
                    ? query.OrderBy(j => j.Duration)
                    : query.OrderByDescending(j => j.Duration);
                break;
            case OrderBy.Distance:
                query = orderAscending == true
                    ? query.OrderBy(j => j.Distance ?? 0)
                    : query.OrderByDescending(j => j.Distance ?? 0);
                break;
        }

        var journeys = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        return journeys;
    }
}