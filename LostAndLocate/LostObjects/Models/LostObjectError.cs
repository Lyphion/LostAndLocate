using System.ComponentModel;

namespace LostAndLocate.LostObjects.Models;

/// <summary>
/// Errors that can occur during execution.
/// </summary>
public enum LostObjectError
{
    [Description("Cannot found specified Object")]
    InvalidId,

    [Description("Invalid Name provided")]
    InvalidName,

    [Description("Invalid Coordinates provided")]
    InvalidCoordinates,

    [Description("Cannot found specified User")]
    InvalidUser,
    
    [Description("Missing Coordinate or Radius")]
    InvalidLocationParameter
}