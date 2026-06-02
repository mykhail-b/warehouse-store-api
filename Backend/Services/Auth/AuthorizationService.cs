using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services.Auth;

/// <summary>
/// Defines operations for user registration and authentication.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Registers a new user account with the provided credentials.
    /// </summary>
    /// <param name="dto">The registration data containing email, password, and user details.</param>
    /// <returns>A JWT token for the newly created account.</returns>
    /// <exception cref="Exception">Thrown when user creation fails due to validation errors.</exception>
    Task<string> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Authenticates a user and returns a JWT token if credentials are valid.
    /// </summary>
    /// <param name="dto">The login credentials containing email and password.</param>
    /// <returns>A JWT token valid for 8 hours if authentication succeeds.</returns>
    /// <exception cref="Exception">Thrown when email is not found or password is incorrect.</exception>
    Task<string> LoginAsync(LoginDto dto);
}

/// <summary>
/// Provides services for user registration, authentication, and JWT token issuance.
/// </summary>
/// <remarks>
/// This service utilizes ASP.NET Core Identity for user management and generates 
/// secure JSON Web Tokens for API authorization.
/// </remarks>
public class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<UserAccount> _userManager;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationService"/> class.
    /// </summary>
    /// <param name="userManager">The ASP.NET Core Identity UserManager for managing user accounts.</param>
    /// <param name="configuration">The application configuration containing JWT settings.</param>
    public AuthorizationService(UserManager<UserAccount> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Registers a new user account with the provided credentials.
    /// </summary>
    /// <param name="dto">The registration data containing email, password, and user details.</param>
    /// <returns>A JWT token for the newly created account.</returns>
    /// <exception cref="Exception">Thrown when user creation fails.</exception>
    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var user = new UserAccount
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Type = dto.Type
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return await GenerateToken(user);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token if credentials are valid.
    /// </summary>
    /// <param name="dto">The login credentials containing email and password.</param>
    /// <returns>A JWT token valid for 8 hours if authentication succeeds.</returns>
    /// <exception cref="Exception">Thrown when email is not found or password is incorrect.</exception>
    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new Exception("Invalid email or password");

        var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid) throw new Exception("Invalid email or password");

        return await GenerateToken(user);
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user account to generate the token for.</param>
    /// <returns>A signed JWT token containing user identity and email claims.</returns>
    /// <remarks>The token is valid for 8 hours from the time of generation.</remarks>
    private Task<string> GenerateToken(UserAccount user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            },
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
