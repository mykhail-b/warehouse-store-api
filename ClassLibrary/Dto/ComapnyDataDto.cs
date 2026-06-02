using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Dto;

/// <summary>
/// Represents detailed company information intended for the administrative dashboard.
/// Includes sensitive or internal-only data.
/// </summary>
public class ComapnyDataDetailDto
{
    public required string Name { get; set; }

    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(50)]
    public required string RegistryNumber { get; set; }

    //[MaxLength(50)]
    //public string VatNumber { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}

/// <summary>
/// Represents public company information intended for external/client-facing API endpoints.
/// </summary>
public class ComapnyDataPublicDto
{
    public required string Name { get; set; }

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}
