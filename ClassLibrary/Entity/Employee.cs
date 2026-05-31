namespace ClassLibrary.Entity;

public class Employee
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; } = default!;
    public UserAccount IdentityUser { get; set; } = default!;
    public int StoreId { get; set; }
    public EmployeeRole Role { get; set; }
}

public enum EmployeeRole { Cashier, Manager, Admin }