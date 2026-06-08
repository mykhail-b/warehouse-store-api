using Microsoft.AspNetCore.Identity;

namespace ClassLibrary.Entity;

public class UserAccount : IdentityUser
{
    public UserType Type { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}

public enum UserType
{
    Customer,
    Employee
}
