using LostAndLocate.Files.Models;
using LostAndLocate.Files.Repositories;

namespace LostAndLocate.Files.Services;

/// <summary>
/// Service implementation for database managed file/content.
/// </summary>
public sealed class FileDatabaseService : IFileService
{
    private readonly IFileRepository _repository;

    private readonly ILogger<FileDatabaseService> _logger;

    public FileDatabaseService(
        IFileRepository repository,
        ILogger<FileDatabaseService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Receives the file data with given <paramref name="group"/> and <paramref name="name"/> and write it to <paramref name="outputStream"/>.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="outputStream">The stream to write the data to</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>True if the data was written</returns>
    public async Task<bool> GetContentAsync(
        string group, string name, Stream outputStream,
        CancellationToken token = default)
    {
        var option = await _repository.GetAsync(group, name, token);

        if (!option.TryGetValue(out var file))
            return false;

        await outputStream.WriteAsync(file.Data, token);
        return true;
    }

    /// <summary>
    /// Saves the file data with given <paramref name="group"/> and <paramref name="name"/> from the <paramref name="inputStream"/>.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="inputStream">The stream to read the data from</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>True if the data was saved</returns>
    public async Task<bool> SaveContentAsync(
        string group, string name, Stream inputStream,
        CancellationToken token = default)
    {
        // Copy data to array
        await using var memoryStream = new MemoryStream();
        await inputStream.CopyToAsync(memoryStream, token);
        var data = memoryStream.ToArray();

        // Create file
        var file = new SavedFile
        {
            Group = group,
            Name = name,
            Data = data
        };

        _logger.LogInformation("Saved file '{Group}/{Path}'", group, name);

        // Delete old file
        await _repository.DeleteAsync(group, name, token);

        // Update database
        await _repository.AddAsync(file, token);
        return true;
    }

    /// <summary>
    /// Deletes the file data for given <paramref name="group"/> and <paramref name="name"/>.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>True if the data was deleted</returns>
    public async Task<bool> DeleteContentAsync(
        string group, string name,
        CancellationToken token = default)
    {
        // Update database
        var option = await _repository.DeleteAsync(group, name, token);

        if (option.HasValue)
            _logger.LogInformation("Deleted file '{Group}/{Path}'", group, name);

        return option.HasValue;
    }
}