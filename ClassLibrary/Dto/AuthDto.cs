using ClassLibrary.Entity;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Dto;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    [Compare(nameof(Password), ErrorMessage = "The passwords don't match")]
    public string ConfirmPassword { get; set; } = string.Empty;
    public UserType Type { get; set; } = UserType.Customer;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
