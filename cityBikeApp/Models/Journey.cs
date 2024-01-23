using System;
using System.Collections.Generic;

namespace cityBikeApp.Models;

public partial class Journey
{
    public int Id { get; set; }

    public DateTime? DepartureDateTime { get; set; }

    public DateTime? ReturnDateTime { get; set; }

    public int DepartureStationId { get; set; }

    public int ReturnStationId { get; set; }

    public int? Distance { get; set; }

    public int? Duration { get; set; }

    public virtual Station DepartureStation { get; set; } = null!;

    public virtual Station ReturnStation { get; set; } = null!;
}
