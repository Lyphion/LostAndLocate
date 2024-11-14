using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CSharpFunctionalExtensions;
using LostAndLocate.Users.Models;
using Microsoft.IdentityModel.Tokens;

namespace LostAndLocate.Users.Services;

/// <summary>
/// Service implementation for authentication.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly ISecurityService _securityService;

    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUserService userService,
        ISecurityService securityService,
        IConfiguration configuration,
        ILogger<AuthenticationService> logger)
    {
        _userService = userService;
        _securityService = securityService;
        _configuration = configuration.GetSection("Jwt");
        _logger = logger;
    }

    /// <summary>
    /// Try to authenticate a <see cref="User"/> with <paramref name="name"/> and <paramref name="password"/> using JWT.
    /// </summary>
    /// <param name="name">The name of the <see cref="User"/></param>
    /// <param name="password">The password of the <see cref="User"/></param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>The <see cref="Authentication"/> of None if <paramref name="name"/> or <paramref name="password"/> is invalid</returns>
    public async Task<Maybe<Authentication>> AuthenticateAsync(
        string name, string password,
        CancellationToken token = default)
    {
        // Get user with name
        var option = await _userService.GetUserByNameAsync(name, token);

        // Check if user exists
        if (!option.TryGetValue(out var user))
            return Maybe<Authentication>.None;

        // Check password
        if (!_securityService.ValidatePassword(password, user.Credentials))
            return Maybe<Authentication>.None;

        var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Key"));

        // Create token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, name)
            }),

            Expires = DateTime.Now.AddDays(7),
            Issuer = _configuration.GetValue<string>("Issuer"),
            Audience = _configuration.GetValue<string>("Audience"),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature
            )
        };

        // Sign token and convert to string
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(securityToken);

        // Create authentication
        var auth = new Authentication
        {
            Type = "Bearer",
            Token = jwt
        };

        _logger.LogInformation("New token created for {User}", name);

        return auth;
    }

    /// <summary>
    /// Checks if the provided JWT <see cref="Authentication"/> token is valid.
    /// </summary>
    /// <param name="authentication">The authentication token</param>
    /// <param name="token">Cancellation token for aborting the request</param>
    /// <returns>Id if <see cref="Authentication"/> is valid else None</returns>
    public async Task<Maybe<Guid>> ValidateAsync(
        Authentication authentication,
        CancellationToken token = default)
    {
        if (!authentication.Type.Equals("Bearer"))
            return Maybe<Guid>.None;

        var key = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Key"));

        // Create token parameter for validation
        var tokenValidation = new TokenValidationParameters
        {
            ValidIssuer = _configuration.GetValue<string>("Issuer"),
            ValidAudience = _configuration.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            // Check if token is valid
            tokenHandler.ValidateToken(authentication.Token, tokenValidation, out var validatedToken);

            if (validatedToken is not JwtSecurityToken securityToken)
                return Maybe<Guid>.None;

            var id = Guid.Parse(
                securityToken.Claims
                    .First(c => c.Type == "nameid")
                    .Value);

            var option = await _userService.GetUserAsync(id, token);
            return option.TryGetValue(out var user) ? user.Id : Maybe.None;
        }
        catch
        {
            return Maybe<Guid>.None;
        }
    }
}