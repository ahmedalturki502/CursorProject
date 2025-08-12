using CursorProject.DTOs;  // Import authentication data transfer objects
using CursorProject.Services;  // Import business logic services
using Microsoft.AspNetCore.Authorization;  // Import authorization attributes and policies
using Microsoft.AspNetCore.Mvc;  // Import MVC controller base classes and attributes
using System.Security.Claims;  // Import security claims for user identity extraction

namespace CursorProject.Controllers  // Define namespace for API controllers
{
    // Authentication controller that handles user authentication and authorization endpoints
    // This controller provides REST API endpoints for user registration, login, logout, and profile management
    [ApiController]  // Indicates this is an API controller with automatic model validation
    [Route("api/[controller]")]  // Route template: api/auth (controller name becomes route segment)
    public class AuthController : ControllerBase  // Inherit from ControllerBase for API controller functionality
    {
        // Dependency injection field for authentication service
        private readonly IAuthService _authService;  // Authentication service for handling auth operations

        // Constructor for dependency injection of authentication service
        // This constructor is called by the DI container when creating the controller
        public AuthController(IAuthService authService)  // Inject authentication service
        {
            _authService = authService;  // Store authentication service reference
        }

        // Register a new user account in the system
        // POST: api/auth/register
        // This endpoint allows new users to create accounts with email, password, and full name
        [HttpPost("register")]  // HTTP POST endpoint for user registration
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)  // Accept registration request and return auth response
        {
            // Call authentication service to register the new user
            var response = await _authService.RegisterAsync(request);  // Process registration request
            
            // Return bad request if registration failed
            if (!response.Success)  // Check if registration was successful
                return BadRequest(response);  // Return 400 Bad Request with error details
                
            // Return success response with user data and tokens
            return Ok(response);  // Return 200 OK with registration success data
        }

        // Authenticate an existing user and provide access tokens
        // POST: api/auth/login
        // This endpoint allows users to log in with email and password
        [HttpPost("login")]  // HTTP POST endpoint for user login
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)  // Accept login request and return auth response
        {
            // Call authentication service to validate credentials and generate tokens
            var response = await _authService.LoginAsync(request);  // Process login request
            
            // Return bad request if login failed
            if (!response.Success)  // Check if login was successful
                return BadRequest(response);  // Return 400 Bad Request with error details
                
            // Return success response with user data and tokens
            return Ok(response);  // Return 200 OK with login success data
        }

        // Refresh an expired JWT token using a refresh token
        // POST: api/auth/refresh
        // This endpoint allows users to get new access tokens without re-entering credentials
        [HttpPost("refresh")]  // HTTP POST endpoint for token refresh
        public async Task<ActionResult<AuthResponse>> RefreshToken(RefreshTokenRequest request)  // Accept refresh request and return new tokens
        {
            // Call authentication service to validate refresh token and generate new tokens
            var response = await _authService.RefreshTokenAsync(request);  // Process token refresh request
            
            // Return bad request if refresh failed
            if (!response.Success)  // Check if token refresh was successful
                return BadRequest(response);  // Return 400 Bad Request with error details
                
            // Return success response with new tokens
            return Ok(response);  // Return 200 OK with new token data
        }

        // Log out a user by invalidating their refresh token
        // POST: api/auth/logout
        // This endpoint requires authentication and invalidates the user's refresh token
        [HttpPost("logout")]  // HTTP POST endpoint for user logout
        [Authorize]  // Require authentication - user must be logged in to logout
        public async Task<ActionResult<AuthResponse>> Logout()  // No parameters needed, uses authenticated user
        {
            // Extract user ID from the authenticated user's claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Get user ID from JWT token claims
            if (string.IsNullOrEmpty(userId))  // Check if user ID was found in claims
            {
                return BadRequest(new AuthResponse  // Return error response if user ID is missing
                { 
                    Success = false,  // Set success flag to false
                    Message = "User not found"  // Set error message
                });
            }

            // Call authentication service to logout the user
            var response = await _authService.LogoutAsync(userId);  // Process logout request
            
            // Return success response
            return Ok(response);  // Return 200 OK with logout success message
        }

        // Get the current user's profile information
        // GET: api/auth/profile
        // This endpoint requires authentication and returns the logged-in user's profile
        [HttpGet("profile")]  // HTTP GET endpoint for user profile
        [Authorize]  // Require authentication - user must be logged in to get profile
        public async Task<ActionResult<AuthResponse>> GetProfile()  // No parameters needed, uses authenticated user
        {
            // Extract user ID from the authenticated user's claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Get user ID from JWT token claims
            if (string.IsNullOrEmpty(userId))  // Check if user ID was found in claims
            {
                return BadRequest(new AuthResponse  // Return error response if user ID is missing
                { 
                    Success = false,  // Set success flag to false
                    Message = "User not found"  // Set error message
                });
            }

            // Call authentication service to get user profile
            var response = await _authService.GetProfileAsync(userId);  // Process profile request
            
            // Return success response with user profile data
            return Ok(response);  // Return 200 OK with user profile information
        }

        // Update the current user's profile information
        // PUT: api/auth/profile
        // This endpoint requires authentication and allows users to update their profile
        [HttpPut("profile")]  // HTTP PUT endpoint for profile updates
        [Authorize]  // Require authentication - user must be logged in to update profile
        public async Task<ActionResult<AuthResponse>> UpdateProfile(UpdateProfileRequest request)  // Accept profile update request
        {
            // Extract user ID from the authenticated user's claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Get user ID from JWT token claims
            if (string.IsNullOrEmpty(userId))  // Check if user ID was found in claims
            {
                return BadRequest(new AuthResponse  // Return error response if user ID is missing
                { 
                    Success = false,  // Set success flag to false
                    Message = "User not found"  // Set error message
                });
            }

            // Call authentication service to update user profile
            var response = await _authService.UpdateProfileAsync(userId, request);  // Process profile update request
            
            // Return bad request if update failed
            if (!response.Success)  // Check if profile update was successful
                return BadRequest(response);  // Return 400 Bad Request with error details
                
            // Return success response with updated profile data
            return Ok(response);  // Return 200 OK with updated profile information
        }

        // Change the current user's password
        // POST: api/auth/change-password
        // This endpoint requires authentication and allows users to change their password
        [HttpPost("change-password")]  // HTTP POST endpoint for password changes
        [Authorize]  // Require authentication - user must be logged in to change password
        public async Task<ActionResult<AuthResponse>> ChangePassword(ChangePasswordRequest request)  // Accept password change request
        {
            // Extract user ID from the authenticated user's claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Get user ID from JWT token claims
            if (string.IsNullOrEmpty(userId))  // Check if user ID was found in claims
            {
                return BadRequest(new AuthResponse  // Return error response if user ID is missing
                { 
                    Success = false,  // Set success flag to false
                    Message = "User not found"  // Set error message
                });
            }

            // Call authentication service to change user password
            var response = await _authService.ChangePasswordAsync(userId, request);  // Process password change request
            
            // Return bad request if password change failed
            if (!response.Success)  // Check if password change was successful
                return BadRequest(response);  // Return 400 Bad Request with error details
                
            // Return success response
            return Ok(response);  // Return 200 OK with password change success message
        }
    }
}
