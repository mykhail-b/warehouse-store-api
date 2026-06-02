namespace ClassLibrary.Entity;

/// <summary>
/// Represents an employee in the system.
/// Links a user account to employee-specific information including role and store assignment.
/// </summary>
public class Employee
{
    /// <summary>
    /// Gets or sets the employee's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the associated user account identifier.
    /// </summary>
    public string IdentityUserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the associated user account.
    /// </summary>
    public UserAccount IdentityUser { get; set; } = default!;

    /// <summary>
    /// Gets or sets the store ID where the employee works.
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the employee's role within the system.
    /// </summary>
    public EmployeeRole Role { get; set; }
}

/// <summary>
/// Specifies the role of an employee in the system.
/// </summary>
public enum EmployeeRole
{
    /// <summary>Cashier employee</summary>
    Cashier,
    /// <summary>Manager employee with supervisory rights</summary>
    Manager,
    /// <summary>Administrator with full system access</summary>
    Admin
}