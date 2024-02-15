namespace CityBikeServiceTests;

using Xunit;
using Moq;
using cityBikeApp.Services;
using cityBikeApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Tests for the StationService class
/// </summary>
public class StationServiceTests : IDisposable
{
    private CitybikeContext _context;
    private IStationService _stationService;

    /// <summary>
    /// Constructor for the StationServiceTests class.
    /// Sets up a new in-memory database and a new StationService instance.
    /// </summary>
    public StationServiceTests()
    {
        var options = new DbContextOptionsBuilder<CitybikeContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CitybikeContext(options);
        _stationService = new StationService(_context);
    }

    /// <summary>
    /// Disposes of the in-memory database after each test.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
    }


    [Fact]
    public async Task TestGetAverageDistanceAndDuration()
    {
        // Arrange
        _context.Journeys.Add(new Journey { DepartureStationId = 1, ReturnStationId = 2, Distance = 10, Duration = 100 });
        _context.Journeys.Add(new Journey { DepartureStationId = 1, ReturnStationId = 2, Distance = 20, Duration = 200 });
        _context.Journeys.Add(new Journey { DepartureStationId = 1, ReturnStationId = 2, Distance = 30, Duration = 300 });
        _context.Journeys.Add(new Journey { DepartureStationId = 3, ReturnStationId = 4, Distance = 30, Duration = 300 });
        _context.Journeys.Add(new Journey { DepartureStationId = 3, ReturnStationId = 5, Distance = 30, Duration = 300 });
        _context.SaveChanges();

        var expectedAverageDistance = 20;
        var expectedAverageDuration = 200;

        // Act
        var averageData = await _stationService.GetAverageDistanceAndDuration(1);

        // Assert
        Assert.Equal(expectedAverageDuration, averageData.Duration);
        Assert.Equal(expectedAverageDistance, averageData.Distance);
    }

    [Fact]
    public async Task TestGetJourneysCount()
    {
        // Arrange
        _context.Stations.Add(new Station { Id = 1 });
        _context.Stations.Add(new Station { Id = 2 });
        _context.Stations.Add(new Station { Id = 3 });
        _context.Stations.Add(new Station { Id = 4 });

        _context.Journeys.Add(new Journey { DepartureStationId = 1, ReturnStationId = 2 });
        _context.Journeys.Add(new Journey { DepartureStationId = 2, ReturnStationId = 1 });
        _context.Journeys.Add(new Journey { DepartureStationId = 1, ReturnStationId = 2 });
        _context.Journeys.Add(new Journey { DepartureStationId = 1, ReturnStationId = 3 });

        _context.SaveChanges();

        var Station1ExpectedCounts = (startedJourneysCount: 3, endedJourneysCount: 1);
        var Station2ExpectedCounts = (startedJourneysCount: 1, endedJourneysCount: 2);
        var Station3ExpectedCounts = (startedJourneysCount: 0, endedJourneysCount: 1);
        var Station4ExpectedCounts = (startedJourneysCount: 0, endedJourneysCount: 0);

        // Act
        var Station1JourneyCount = await _stationService.GetJourneysCounts(1);
        var Station2JourneyCount = await _stationService.GetJourneysCounts(2);
        var Station3JourneyCount = await _stationService.GetJourneysCounts(3);
        var Station4JourneyCount = await _stationService.GetJourneysCounts(4);

        // Assert
        Assert.Equal(Station1ExpectedCounts, Station1JourneyCount);
        Assert.Equal(Station2ExpectedCounts, Station2JourneyCount);
        Assert.Equal(Station3ExpectedCounts, Station3JourneyCount);
        Assert.Equal(Station4ExpectedCounts, Station4JourneyCount);
    }
}