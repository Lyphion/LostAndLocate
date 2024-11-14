using System.ComponentModel;

namespace LostAndLocate.Reviews.Models;

/// <summary>
/// Errors that can occur during execution.
/// </summary>
public enum ReviewError
{
    [Description("Sender and target must be differnt")]
    InvalidTarget,

    [Description("Cannot found specified User")]
    InvalidUser,

    [Description("Rating is out of range")]
    InvalidRating
}