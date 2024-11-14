using System.ComponentModel;

namespace LostAndLocate.Users.Models;

/// <summary>
/// Errors that can occur during execution.
/// </summary>
public enum UserError
{
    [Description("Cannot found specified User")]
    InvalidId,

    [Description("Invalid Name provided")]
    InvalidName,

    [Description("Invalid Email provided")]
    InvalidEmail,

    [Description("Invalid Password provided")]
    InvalidPassword,

    [Description("Name already taken")]
    DuplicateName,

    [Description("Email already taken")]
    DuplicateEmail
}