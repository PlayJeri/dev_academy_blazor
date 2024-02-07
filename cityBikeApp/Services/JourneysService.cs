using System.Reflection.Metadata.Ecma335;
using cityBikeApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace cityBikeApp;

public class JourneysService
{
    private CitybikeContext _context;

    public JourneysService(CitybikeContext context)
    {
        _context = context;
    }

    public async Task<List<Journey>> GetJourneys()
    {
        return await _context.Journeys
            .Include(j => j.DepartureStation)
            .Include(j => j.ReturnStation)
            .Take(20)
            .ToListAsync();
    }
}
