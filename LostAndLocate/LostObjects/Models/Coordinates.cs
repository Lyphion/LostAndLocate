using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.LostObjects.Models;

/// <summary>
/// Object which describes coordinates by latitude and longitude.
/// </summary>
[Owned]
public sealed class Coordinates
{
    /// <summary>
    /// Coordinate that specifies the north–south position of a point on the surface of the Earth.
    /// Latitude is given as an angle that ranges from –90° at the south pole to 90° at the north pole, with 0° at the Equator.
    /// </summary>
    [Required]
    [Range(-90, 90)]
    public double Latitude { get; init; }

    /// <summary>
    /// Coordinate that specifies the east–west position of a point on the surface of the Earth.
    /// Longitude is given as an angle that ranges from –180° to 180°.
    /// </summary>
    [Required]
    [Range(-180, 180)]
    public double Longitude { get; init; }

    /// <summary>
    /// Checks if this object is valid.
    /// </summary>
    /// <returns>True if latitude and longitude are in range</returns>
    public bool Valid() => Latitude is >= -90 and <= 90 && Longitude is >= -180 and <= 180;

    /// <summary>
    /// Calculate the distance to another <see cref="Coordinates"/> on earth.
    /// </summary>
    /// <param name="other">The coordinate to get the distance to</param>
    /// <returns>The distance in meters</returns>
    public double Distance(Coordinates other)
    {
        const double oneDegree = Math.PI / 180.0;
        // Radius of earth
        const double radius = 6_378_137;
        
        var d1 = Latitude * oneDegree;
        var num1 = Longitude * oneDegree;
        var d2 = other.Latitude * oneDegree;
        var num2 = other.Longitude * oneDegree - num1;
        
        // https://www.movable-type.co.uk/scripts/latlong.html
        var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) 
                 + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

        return radius * 2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3));
    }
}