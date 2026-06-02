using ClassLibrary.Entity;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Dto;

/// <summary>
/// Data transfer object for user registration.
/// </summary>
public class RegisterDto
{
    /// <summary>Gets or sets the email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's first name.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's last name.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the password confirmation.</summary>
    [Compare(nameof(Password), ErrorMessage = "The passwords don't match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>Gets or sets the user type (Customer or Employee).</summary>
    public UserType Type { get; set; } = UserType.Customer;
}

/// <summary>
/// Data transfer object for user login.
/// </summary>
public class LoginDto
{
    /// <summary>Gets or sets the email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the password.</summary>
    public string Password { get; set; } = string.Empty;
}
