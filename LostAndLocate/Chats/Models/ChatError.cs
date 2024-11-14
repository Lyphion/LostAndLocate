using System.ComponentModel;

namespace LostAndLocate.Chats.Models;

/// <summary>
/// Errors that can occur during execution.
/// </summary>
public enum ChatError
{
    [Description("Sender and target must be differnt")]
    InvalidTarget,

    [Description("Cannot found specified User")]
    InvalidUser,

    [Description("Invalid Message provides")]
    InvalidMessage
}