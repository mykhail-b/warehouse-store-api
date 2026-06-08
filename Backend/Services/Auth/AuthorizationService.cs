using Backend.Services.Infrastructure;
using ClassLibrary;
using ClassLibrary.Dto;
using ClassLibrary.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services.Auth;

public interface IAuthorizationService
{
    Task<string> RegisterAsync(RegisterDto dto);

    Task<string> LoginAsync(LoginDto dto);
}

public class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<UserAccount> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IMailService _mailService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationService(
        UserManager<UserAccount> userManager,
        IConfiguration configuration,
        IMailService mailService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _configuration = configuration;
        _mailService = mailService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        var user = new UserAccount
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Type = dto.Type
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        // Generating an email confirmation token
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // We create a link like this: https://yourdomain.com/api/auth/confirm-email?userId=...&token=...
        var request = _httpContextAccessor.HttpContext!.Request;
        var confirmationLink = $"{request.Scheme}://{request.Host}/api/auth/confirm-email" +
                               $"?userId={user.Id}&token={encodedToken}";

        await _mailService.SendRegisterEmailConfirmationAsync(new RegisterConfirmationMail
        {
            To = user.Email!,
            RecipientName = $"{dto.FirstName} {dto.LastName}",
            ConfirmationLink = confirmationLink
        });

        return await GenerateToken(user);
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new Exception("Invalid email or password");

        var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid) throw new Exception("Invalid email or password");

        return await GenerateToken(user);
    }

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
