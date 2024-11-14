using System.Security.Claims;

namespace LostAndLocate.Utils;

/// <summary>
/// Extension class for receiving the Id of the user.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Receives the Id of the user from the claims.
    /// </summary>
    /// <param name="principal">The authenticated user</param>
    /// <returns>The Id of the user</returns>
    public static Guid GetId(this ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier));
}