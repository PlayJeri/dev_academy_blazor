using System.Runtime.CompilerServices;
using cityBikeApp.Models;
using Microsoft.EntityFrameworkCore;

namespace cityBikeApp;

public interface IStationService
{
    public List<Station> GetStations();
    void OrderStationsByName(SortOrder order);
    int GetStationsCount(string searchTerm);
    List<Station> FilterStations(string searchTerm, int pageNumber = 1, int pageSize = 20);
    Task<StationDetails> GetStationDetails(int stationId);
    Task<(int Duration, int Distance)> GetAverageDistanceAndDuration(int stationId);
    Task<(List<TopThreeStation> ReturnStations, List<TopThreeStation> DepartureStations)> getTopThreeStations(int stationId);
    Task<(int startedJourneysCount, int endedJourneysCount)> GetJourneysCounts(int stationId);
    Task<List<PeakTime>> GetPeakTimesAsync(int stationId);
}

public class StationService : IStationService
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

    public int GetStationsCount(string searchTerm)
    {
        var stations = _cachedStations.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            stations = stations.Where(
                s => s.StationName != null &&
                s.StationName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );
        }

        return stations.Count();
    }

    public List<Station> FilterStations(string searchTerm, int pageNumber = 1, int pageSize = 20)
    {
        var stations = _cachedStations.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            stations = stations.Where(
                s => s.StationName != null &&
                s.StationName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );
        }

        int maxPageNumber = stations.Count() / pageSize + 1;

        if (pageNumber > maxPageNumber)
        {
            pageNumber = maxPageNumber;
        }

        return stations
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public List<Station> GetStations()
    {
        return _cachedStations;
    }

    public async Task<StationDetails> GetStationDetails(int stationId)
    {
        var station = _cachedStations.FirstOrDefault(s => s.Id == stationId);
        var journeysCounts = await GetJourneysCounts(stationId);
        var peakTimes = await GetPeakTimesAsync(stationId);
        var topThreeStations = await getTopThreeStations(stationId);
        var (averageDuration, averageDistance) = await GetAverageDistanceAndDuration(stationId);

        return new StationDetails
        {
            Station = station,
            StartedJourneysCount = journeysCounts.startedJourneysCount,
            EndedJourneysCount = journeysCounts.endedJourneysCount,
            AvgJourneyDuration = averageDuration,
            AvgJourneyDistance = averageDistance,
            TopThreeReturnStations = topThreeStations.ReturnStations,
            TopThreeDepartureStations = topThreeStations.DepartureStations,
            PeakTimes = peakTimes
        };
    }

    public async Task<(int Duration, int Distance)> GetAverageDistanceAndDuration(int stationId)
    {
        var journeys = await _context.Journeys
            .Where(j => j.DepartureStationId == stationId || j.ReturnStationId == stationId)
            .ToListAsync();

        var totalDistance = journeys.Sum(j => j.Distance);
        var averageDistance = totalDistance / journeys.Count ?? 0;

        var totalDuration = journeys.Sum(j => j.Duration);
        var averageDuration = totalDuration / journeys.Count ?? 0;

        return (averageDuration, averageDistance);
    }

    public async Task<(List<TopThreeStation> ReturnStations, List<TopThreeStation> DepartureStations)> getTopThreeStations(int stationId)
    {
        var top3ReturnStations = await _context.Journeys
            .Where(j => j.ReturnStationId == stationId)
            .GroupBy(j => new { j.DepartureStationId, j.DepartureStation.StationName })
            .Select(g => new { g.Key.StationName, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(3)
            .Select(g => new TopThreeStation { StationName = g.StationName!, Count = g.Count })
            .ToListAsync();

        var top3DepartureStations = await _context.Journeys
            .Where(j => j.DepartureStationId == stationId)
            .GroupBy(j => new { j.ReturnStationId, j.ReturnStation.StationName })
            .Select(g => new { g.Key.StationName, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(3)
            .Select(g => new TopThreeStation { StationName = g.StationName!, Count = g.Count })
            .ToListAsync();

        return (top3ReturnStations, top3DepartureStations);
    }

    public async Task<(int startedJourneysCount, int endedJourneysCount)> GetJourneysCounts(int stationId)
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

    public async Task<List<PeakTime>> GetPeakTimesAsync(int stationId)
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
