namespace LostAndLocate.Data;

/// <summary>
/// Basis Entity definition.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Unique Id of the Entity.
    /// </summary>
    Guid Id { get; init; }
}