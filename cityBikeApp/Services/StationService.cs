using cityBikeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace cityBikeApp;

public class StationService
{
    private readonly CitybikeContext _context;
    private List<Station> _cachedStations = new();

    public StationService(CitybikeContext context)
    {
        _context = context;
        _cachedStations = _context.Stations.ToList();
    }

    public void OrderStationsByName(SortOrder order)
    {
        if (order == SortOrder.Ascending)
        {
            _cachedStations = _cachedStations.OrderBy(s => s.StationName).ToList();
        }
        else
        {
            _cachedStations = _cachedStations.OrderByDescending(s => s.StationName).ToList();
        }
    }

    public List<Station> FilterStations(string filter)
    {
        return _cachedStations
            .Where(
                s => s.StationName != null &&
                s.StationName.Contains(filter, StringComparison.OrdinalIgnoreCase)
            )
            .ToList();
    }

    public List<Station> GetStations()
    {
        return _cachedStations;
    }

    private void DoesNothing()
    {
        return;
    }

    public async Task<StationDetails> GetStation(int stationId)
    {
        var station = _cachedStations.FirstOrDefault(s => s.Id == stationId);
        var journeysCounts = await GetJourneysCounts(stationId);
        var peakTimes = await GetPeakTimesAsync(stationId);

        return new StationDetails
        {
            Station = station,
            StartedJourneysCount = journeysCounts.startedJourneysCount,
            EndedJourneysCount = journeysCounts.endedJourneysCount,
            PeakTimes = peakTimes
        };
    }

    private async Task<(int startedJourneysCount, int endedJourneysCount)> GetJourneysCounts(int stationId)
    {
        var startedJourneysCount = await _context.Stations
            .Where(s => s.Id == stationId)
            .SelectMany(s => s.JourneyDepartureStations)
            .CountAsync();

        var endedJourneysCount = await _context.Stations
            .Where(s => s.Id == stationId)
            .SelectMany(s => s.JourneyReturnStations)
            .CountAsync();

        return (startedJourneysCount, endedJourneysCount);
    }

    private async Task<List<PeakTime>> GetPeakTimesAsync(int stationId)
    {
        var peakTimes = await _context.Journeys
                .Where(j => j.DepartureStationId == stationId && j.DepartureDateTime.HasValue)
                .GroupBy(j => j.DepartureDateTime!.Value.Hour)
                .Select(g => new PeakTime { Hour = g.Key, Count = g.Count() })
                .OrderBy(pt => pt.Hour)
                .ToListAsync();

        return peakTimes;
    }
}
