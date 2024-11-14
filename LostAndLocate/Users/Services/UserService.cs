using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using LostAndLocate.Users.Models;
using LostAndLocate.Users.Repositories;

namespace LostAndLocate.Users.Services;

/// <summary>
/// Service implementation for users.
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly ISecurityService _securityService;

    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repository,
        ISecurityService securityService,
        ILogger<UserService> logger)
    {
        _repository = repository;
        _securityService = securityService;
        _logger = logger;
    }

    /// <summary>
    /// Receives a list of all <see cref="User"/>s.
    /// </summary>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>A list of all <see cref="User"/>s</returns>
    public async Task<IEnumerable<User>> GetUsersAsync(
        CancellationToken token = default)
    {
        return await _repository.GetAllAsync(token);
    }

    /// <summary>
    /// Receives the matching <see cref="User"/> with the provided Id.
    /// </summary>
    /// <param name="userId">Id of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if Id was not found</returns>
    public async Task<Maybe<User>> GetUserAsync(
        Guid userId, CancellationToken token = default)
    {
        return await _repository.GetAsync(userId, token);
    }

    /// <summary>
    /// Receives a <see cref="User"/> by its <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The Name of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if name was not found</returns>
    public async Task<Maybe<User>> GetUserByNameAsync(
        string name, CancellationToken token = default)
    {
        return await _repository.GetByNameAsync(name, token);
    }

    /// <summary>
    /// Receives a <see cref="User"/> by its <paramref name="email"/>.
    /// </summary>
    /// <param name="email">The E-Mail of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if email was not found</returns>
    public async Task<Maybe<User>> GetUserByEmailAsync(
        string email, CancellationToken token = default)
    {
        return await _repository.GetByEmailAsync(email, token);
    }

    /// <summary>
    /// Creates a new <see cref="User"/> with the given properties.
    /// </summary>
    /// <param name="name">The name of the user</param>
    /// <param name="email">The E-Mail of the user</param>
    /// <param name="password">The password of the user</param>
    /// <param name="description">A description of the object</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The created <see cref="User"/> or <see cref="UserError"/> if some property is invalid</returns>
    public async Task<Result<User, UserError>> CreateUserAsync(
        string name, string email, string password, string description,
        CancellationToken token = default)
    {
        // Check if name is valid
        if (name.Length is < 4 or > 16)
            return UserError.InvalidName;

        // Check if email is valid
        if (!Regex.IsMatch(email, User.EmailRegex))
            return UserError.InvalidEmail;

        // Check if password is valid
        if (!Regex.IsMatch(password, User.PasswordRegex))
            return UserError.InvalidPassword;

        // Check if name exists
        var nameOption = await GetUserByNameAsync(name, token);
        if (nameOption.HasValue)
            return UserError.DuplicateName;

        // Check if email exists
        var emailOption = await GetUserByEmailAsync(email, token);
        if (emailOption.HasValue)
            return UserError.DuplicateEmail;

        // Create credentials
        var credentials = _securityService.HashPassword(password);

        // Create user
        var user = new User
        {
            Name = name,
            Email = email,
            Credentials = credentials,
            Description = description,
        };

        _logger.LogInformation("Created user {Id}", user.Id);

        // Update database
        return await _repository.AddAsync(user, token);
    }

    /// <summary>
    /// Updated a <see cref="User"/> with the given properties.
    /// </summary>
    /// <param name="userId">The name of the <see cref="User"/></param>
    /// <param name="name">The name of the user</param>
    /// <param name="email">The E-Mail of the user</param>
    /// <param name="password">The password of the user</param>
    /// <param name="description">A description of the object</param>
    /// <param name="admin">Whether the user is an admin</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The updated <see cref="User"/> or <see cref="UserError"/> if some property is invalid</returns>
    public async Task<Result<User, UserError>> UpdateUserAsync(
        Guid userId, string? name = null, string? email = null, string? password = null,
        string? description = null, bool? admin = null, CancellationToken token = default)
    {
        var option = await GetUserAsync(userId, token);

        // Check if user exists
        if (!option.TryGetValue(out var user))
            return UserError.InvalidId;

        // Save name if provided and changed
        if (name is not null && !name.Equals(user.Name, StringComparison.InvariantCulture))
        {
            // Check if name is valid
            if (name.Length is < 4 or > 16)
                return UserError.InvalidName;

            // Check if name exists
            if (!name.Equals(user.Name, StringComparison.InvariantCultureIgnoreCase)
                && (await GetUserByNameAsync(name, token)).HasValue)
                return UserError.DuplicateName;

            user.Name = name;
        }

        // Save email if provided and changed
        if (email is not null && !email.Equals(user.Email, StringComparison.InvariantCulture))
        {
            // Check if email is valid
            if (!Regex.IsMatch(email, User.EmailRegex))
                return UserError.InvalidEmail;

            // Check if email exists
            if (!email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase)
                && (await GetUserByEmailAsync(email, token)).HasValue)
                return UserError.DuplicateEmail;

            user.Email = email;
        }

        // Save password if provided
        if (password is not null)
        {
            // Check if password is valid
            if (!Regex.IsMatch(password, User.PasswordRegex))
                return UserError.InvalidPassword;

            user.Credentials = _securityService.HashPassword(password);
        }

        // Save description if provided
        if (description is not null)
            user.Description = description;

        // Save admin if provided
        if (admin is not null)
            user.Admin = (bool) admin;

        _logger.LogInformation("Updated user {Id}", user.Id);

        // Update database
        return await _repository.UpdateAsync(user, token);
    }

    /// <summary>
    /// Deletes and returns the matching <see cref="User"/> with the provided Id.
    /// </summary>
    /// <param name="userId">Id of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The matching <see cref="User"/> or None if Id was not found</returns>
    public async Task<Maybe<User>> DeleteUserAsync(
        Guid userId, CancellationToken token = default)
    {
        // Update database
        var option = await _repository.DeleteAsync(userId, token);

        if (option.HasValue)
            _logger.LogInformation("Deleted user {Id}", userId);

        return option;
    }
}