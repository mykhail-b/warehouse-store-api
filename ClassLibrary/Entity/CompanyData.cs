using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassLibrary.Entity;

/// <summary>
/// Represents company information and details.
/// Stores organization data including legal information, location, and contact details.
/// This is typically a singleton entity with a single record.
/// </summary>
[Table("CompanyData", Schema = "Company")]
public class CompanyData
{
    /// <summary>
    /// Gets or sets the company data's unique identifier (typically 1 for singleton pattern).
    /// </summary>
    public short Id { get; set; } = 1;

    /// <summary>
    /// Gets or sets the company's legal name (max 200 characters).
    /// </summary>
    [MaxLength(200)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the company's country (max 100 characters).
    /// </summary>
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company's physical address (max 500 characters).
    /// </summary>
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company's registry or identification number (max 50 characters).
    /// </summary>
    [MaxLength(50)]
    public required string RegistryNumber { get; set; }

    /// <summary>
    /// Gets or sets the company's email address (max 100 characters).
    /// </summary>
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the company's telephone number (max 20 characters).
    /// </summary>
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}
