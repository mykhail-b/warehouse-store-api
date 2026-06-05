using Microsoft.AspNetCore.Identity;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents a user account with authentication and profile information.
/// Extends ASP.NET Core Identity IdentityUser for authentication support.
/// </summary>
public class UserAccount : IdentityUser
{
    public UserType Type { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Specifies the type of user in the system.
/// </summary>
public enum UserType
{
    Customer,
    Employee
}
