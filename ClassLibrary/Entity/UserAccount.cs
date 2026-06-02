using Microsoft.AspNetCore.Identity;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents a user account with authentication and profile information.
/// Extends ASP.NET Core Identity IdentityUser for authentication support.
/// </summary>
public class UserAccount : IdentityUser
{
    /// <summary>
    /// Gets or sets the type of user account (Customer or Employee).
    /// </summary>
    public UserType Type { get; set; }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Specifies the type of user in the system.
/// </summary>
public enum UserType
{
    /// <summary>Regular customer user</summary>
    Customer,
    /// <summary>Employee user with system access</summary>
    Employee
}
