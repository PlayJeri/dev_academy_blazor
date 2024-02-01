using cityBikeApp.Models;

namespace cityBikeApp;

public class StationDetails
{
    public Station? Station { get; set; }
    public int StartedJourneysCount { get; set; }
    public int EndedJourneysCount { get; set; }
    public int AvgJourneyDuration { get; set; }
    public int AvgJourneyDistance { get; set; }
    public List<TopThreeStation>? TopThreeReturnStations { get; set; }
    public List<TopThreeStation>? TopThreeDepartureStations { get; set; }
    public List<PeakTime>? PeakTimes { get; set; }
}
