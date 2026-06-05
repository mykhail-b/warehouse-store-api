using Backend.Services.Auth;
using ClassLibrary.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Auth
{
    /// <summary>
    /// Handles user authentication operations including registration and login.
    /// Provides endpoints for user account creation and JWT token generation.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthorizationService _authService;

        public AuthController(IAuthorizationService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user account with the provided credentials.
        /// </summary>
        /// <param name="dto">The registration data containing email, password, and user details.</param>
        /// <returns>A JWT token for the newly created user account.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var token = await _authService.RegisterAsync(dto);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token if credentials are valid.
        /// </summary>
        /// <param name="dto">The login data containing email and password.</param>
        /// <returns>A JWT token if authentication succeeds; otherwise, an unauthorized response.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);
                return Ok(new { Token = token });
            }
            catch (Exception)
            {
                // We return a generic message to prevent user enumeration
                return Unauthorized("Invalid email or password.");
            }
        }
    }
}