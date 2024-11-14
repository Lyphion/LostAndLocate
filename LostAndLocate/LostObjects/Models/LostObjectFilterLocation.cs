using System.ComponentModel.DataAnnotations;

namespace LostAndLocate.LostObjects.Models;

/// <summary>
/// A filter to limit the range of search by area.
/// </summary>
public sealed class LostObjectFilterLocation
{
    /// <summary>
    /// Coordinate that specifies the north–south position of a point on the surface of the Earth.
    /// Latitude is given as an angle that ranges from –90° at the south pole to 90° at the north pole, with 0° at the Equator.
    /// </summary>
    [Range(-90, 90)]
    public double? Latitude { get; init; }

    /// <summary>
    /// Coordinate that specifies the east–west position of a point on the surface of the Earth.
    /// Longitude is given as an angle that ranges from –180° to 180°.
    /// </summary>
    [Range(-180, 180)]
    public double? Longitude { get; init; }

    /// <summary>
    /// Distance in Meters from the central Coordinate. Default is 1000 Meter.
    /// </summary>
    [Range(0, double.MaxValue)]
    public double Radius { get; init; } = 1000;
}