// Import necessary namespaces for the authentication controller
using CursorProject.DTOs.Auth;                 // Data transfer objects for requests/responses
using CursorProject.Services;                  // Custom services (AuthService)
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Mvc;                // MVC controller base classes
using System.Security.Claims;                  // User claims for authentication

// Namespace for API controllers
namespace CursorProject.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and authorization
    /// Provides endpoints for registration, login, logout, and profile management
    /// </summary>
    [ApiController]  // Indicates this is an API controller
    [Route("api/[controller]")]  // Route template: api/auth
    public class AuthController : ControllerBase
    {
        // Private field for dependency injection
        /// <summary>
        /// Authentication service for handling auth operations
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor that accepts dependencies via dependency injection
        /// </summary>
        /// <param name="authService">Authentication service for auth operations</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;        // Store auth service reference
        }

        /// <summary>
        /// Registers a new user account
        /// POST: api/auth/register
        /// </summary>
        /// <param name="request">Registration request containing user details</param>
        /// <returns>Authentication response with tokens and user information</returns>
        [HttpPost("register")]  // HTTP POST endpoint
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Authenticates a user and provides access tokens
        /// POST: api/auth/login
        /// </summary>
        /// <param name="request">Login request containing credentials</param>
        /// <returns>Authentication response with tokens and user information</returns>
        [HttpPost("login")]  // HTTP POST endpoint
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Refreshes an expired JWT token using a refresh token
        /// POST: api/auth/refresh
        /// </summary>
        /// <param name="request">Refresh token request</param>
        /// <returns>New authentication response with fresh tokens</returns>
        [HttpPost("refresh")]  // HTTP POST endpoint
        public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)
        {
            var response = await _authService.RefreshTokenAsync(request);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Logs out a user by invalidating their refresh token
        /// POST: api/auth/logout
        /// </summary>
        /// <returns>Success response indicating logout completion</returns>
        [HttpPost("logout")]  // HTTP POST endpoint
        [Authorize]  // Require authentication
        public async Task<ActionResult<AuthResponse>> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "User not found" 
                });
            }

            var response = await _authService.LogoutAsync(userId);
            return Ok(response);
        }

        /// <summary>
        /// Gets the current user's profile information
        /// GET: api/auth/profile
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet("profile")]  // HTTP GET endpoint
        [Authorize]  // Require authentication
        public async Task<ActionResult<AuthResponse>> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "User not found" 
                });
            }

            var response = await _authService.GetProfileAsync(userId);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Updates the current user's profile information
        /// PUT: api/auth/profile
        /// </summary>
        /// <param name="request">Profile update request</param>
        /// <returns>Updated user profile information</returns>
        [HttpPut("profile")]  // HTTP PUT endpoint
        [Authorize]  // Require authentication
        public async Task<ActionResult<AuthResponse>> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "User not found" 
                });
            }

            var response = await _authService.UpdateProfileAsync(userId, request);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }

        /// <summary>
        /// Changes the current user's password
        /// POST: api/auth/change-password
        /// </summary>
        /// <param name="request">Password change request</param>
        /// <returns>Success response indicating password change completion</returns>
        [HttpPost("change-password")]  // HTTP POST endpoint
        [Authorize]  // Require authentication
        public async Task<ActionResult<AuthResponse>> ChangePassword(ChangePasswordRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "User not found" 
                });
            }

            var response = await _authService.ChangePasswordAsync(userId, request);
            
            if (!response.Success)
                return BadRequest(response);
                
            return Ok(response);
        }
    }
}
