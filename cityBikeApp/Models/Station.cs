using System;
using System.Collections.Generic;

namespace cityBikeApp.Models;

public partial class Station
{
    public int Id { get; set; }

    public string? StationName { get; set; }

    public string? StationAddress { get; set; }

    public string? CoordinateX { get; set; }

    public string? CoordinateY { get; set; }

    public virtual ICollection<Journey> JourneyDepartureStations { get; } = new List<Journey>();

    public virtual ICollection<Journey> JourneyReturnStations { get; } = new List<Journey>();
}
