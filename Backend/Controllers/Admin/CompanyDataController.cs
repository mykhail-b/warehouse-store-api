using Backend.Services;
using ClassLibrary.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Admin;

/// <summary>
/// Manages company information and data access.
/// Provides endpoints for retrieving and managing company details with role-based access control.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CompanyDataController : ControllerBase
{
    private readonly ICompanyDataService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyDataController"/> class.
    /// </summary>
    /// <param name="service">The company data service for handling company information.</param>
    public CompanyDataController(ICompanyDataService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves publicly available company data.
    /// This endpoint is accessible without authentication.
    /// </summary>
    /// <returns>Company data visible to the public including name, address, email, and phone.</returns>
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<ActionResult<ComapnyDataPublicDto>> GetPublicCompanyData()
    {
        return Ok(await _service.GetPublicCompanyData());
    }

    /// <summary>
    /// Retrieves detailed company data including sensitive information.
    /// This endpoint requires authorization.
    /// </summary>
    /// <returns>Complete company data including registry number and all details.</returns>
    [Authorize]
    [HttpGet("admin")]
    public async Task<ActionResult<ComapnyDataDetailDto>> GetCompanyData()
    {
        return Ok(await _service.GetCompanyData());
    }

    /// <summary>
    /// Creates or updates company data.
    /// This endpoint requires authorization and will update existing records or create new ones.
    /// </summary>
    /// <param name="dto">The company data details to save.</param>
    /// <returns>The saved company data.</returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SetCompanyData([FromBody] ComapnyDataDetailDto dto)
    {
        return Ok(await _service.SetCompanyData(dto));
    }
}