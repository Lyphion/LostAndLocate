namespace LostAndLocate.Files.Services;

/// <summary>
/// Service definitions for file/content.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Receives the file data with given <paramref name="group"/> and <paramref name="name"/> and write it to <paramref name="outputStream"/>.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="outputStream">The stream to write the data to</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>True if the data was written</returns>
    Task<bool> GetContentAsync(
        string group, string name, Stream outputStream,
        CancellationToken token = default);

    /// <summary>
    /// Saves the file data with given <paramref name="group"/> and <paramref name="name"/> from the <paramref name="inputStream"/>.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="inputStream">The stream to read the data from</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>True if the data was saved</returns>
    Task<bool> SaveContentAsync(
        string group, string name, Stream inputStream,
        CancellationToken token = default);

    /// <summary>
    /// Deletes the file data for given <paramref name="group"/> and <paramref name="name"/>.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>True if the data was deleted</returns>
    Task<bool> DeleteContentAsync(
        string group, string name,
        CancellationToken token = default);
}