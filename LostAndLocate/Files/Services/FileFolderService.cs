namespace LostAndLocate.Files.Services;

/// <summary>
/// Service implementation for filesystem managed file/content.
/// </summary>
public sealed class FileFolderService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileFolderService> _logger;

    public FileFolderService(
        IWebHostEnvironment environment,
        ILogger<FileFolderService> logger)
    {
        _environment = environment;
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
        var path = GetCompletePath(group, name);

        // Check if file exists
        if (!File.Exists(path))
            return false;

        // Copy file to stream
        await using var file = File.OpenRead(path);
        await file.CopyToAsync(outputStream, token);

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
        var path = GetCompletePath(group, name);

        // Create parent folders if they don't exist
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        // Save file through copying from stream
        await using var file = File.Create(path);
        await inputStream.CopyToAsync(file, token);

        _logger.LogInformation("Saved file '{Group}/{Path}'", group, name);

        return true;
    }

    /// <summary>
    /// Deletes the file data for given <paramref name="group"/> and <paramref name="name"/>.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>True if the data was deleted</returns>
    public Task<bool> DeleteContentAsync(
        string group, string name,
        CancellationToken token = default)
    {
        var path = GetCompletePath(group, name);

        // Check if file exists
        if (!File.Exists(path))
            return Task.FromResult(false);

        // Delete file
        File.Delete(path);
        _logger.LogInformation("Deleted file '{Group}/{Path}'", group, name);

        return Task.FromResult(true);
    }

    /// <summary>
    /// Converts the <paramref name="group"/> and <paramref name="name"/> into a filepath.
    /// </summary>
    /// <param name="group">The group of the file</param>
    /// <param name="name">The name of the file</param>
    /// <returns>The converted filepath</returns>
    private string GetCompletePath(string group, string name)
        => Path.Combine(_environment.ContentRootPath, "resources", group, name);
}