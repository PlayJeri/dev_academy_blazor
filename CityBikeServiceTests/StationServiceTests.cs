namespace CityBikeServiceTests;

using Xunit;
using Moq;
using cityBikeApp.Services;
using cityBikeApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class StationServiceTests : IDisposable
{
    private CitybikeContext _context;
    private IStationService _stationService;

    public StationServiceTests()
    {
        var options = new DbContextOptionsBuilder<CitybikeContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CitybikeContext(options);
        _stationService = new StationService(_context);
    }

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
}