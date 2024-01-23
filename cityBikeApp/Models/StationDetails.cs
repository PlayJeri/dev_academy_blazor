using cityBikeApp.Models;

namespace cityBikeApp;

public partial class StationDetails
{
    public Station? Station { get; set; }
    public int StartedJourneysCount { get; set; }
    public int EndedJourneysCount { get; set; }
    public List<PeakTime>? PeakTimes { get; set; }
}
