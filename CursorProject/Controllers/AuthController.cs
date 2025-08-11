// Import necessary namespaces for the authentication controller
using CursorProject.DTOs;                      // Data transfer objects for requests/responses
using CursorProject.Models;                    // Application models (ApplicationUser, ApplicationRole)
using CursorProject.Services;                  // Custom services (JwtService)
using Microsoft.AspNetCore.Authorization;      // Authorization attributes
using Microsoft.AspNetCore.Identity;           // ASP.NET Identity for user management
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
        // Private fields for dependency injection
        /// <summary>
        /// User manager for user account operations
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;
        
        /// <summary>
        /// Sign-in manager for authentication operations
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        /// <summary>
        /// Role manager for role-based operations
        /// </summary>
        private readonly RoleManager<ApplicationRole> _roleManager;
        
        /// <summary>
        /// JWT service for token generation and validation
        /// </summary>
        private readonly JwtService _jwtService;

        /// <summary>
        /// Constructor that accepts dependencies via dependency injection
        /// </summary>
        /// <param name="userManager">User manager for user operations</param>
        /// <param name="signInManager">Sign-in manager for authentication</param>
        /// <param name="roleManager">Role manager for role operations</param>
        /// <param name="jwtService">JWT service for token handling</param>
        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            JwtService jwtService)
        {
            _userManager = userManager;        // Store user manager reference
            _signInManager = signInManager;    // Store sign-in manager reference
            _roleManager = roleManager;        // Store role manager reference
            _jwtService = jwtService;          // Store JWT service reference
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
            // Check if a user with the provided email already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                // Return bad request if email is already taken
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Email already exists" 
                });
            }

            // Create a new user object with the provided information
            var user = new ApplicationUser
            {
                UserName = request.Email,      // Use email as username
                Email = request.Email,         // Set email address
                FullName = request.FullName    // Set full name
            };

            // Attempt to create the user with the provided password
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // If creation fails, return validation errors
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = errors 
                });
            }

            // Assign the default "Customer" role to the new user
            await _userManager.AddToRoleAsync(user, "Customer");

            // Generate JWT access token and refresh token for the new user
            var token = await _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Get the user's roles for the response
            var roles = await _userManager.GetRolesAsync(user);

            // Return successful registration response with tokens and user info
            return Ok(new AuthResponse
            {
                Success = true,
                Token = token,                 // JWT access token
                RefreshToken = refreshToken,   // Refresh token
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),  // Token expiration time
                User = new UserDto
                {
                    Id = user.Id,              // User ID
                    FullName = user.FullName,  // User's full name
                    Email = user.Email!        // User's email
                },
                Roles = roles.ToList(),        // User's roles
                Message = "Registration successful"  // Success message
            });
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
            // Find user by email address
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Return bad request if user doesn't exist (don't reveal which credential is wrong)
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid email or password" 
                });
            }

            // Verify the provided password against the user's stored password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                // Return bad request if password is incorrect (don't reveal which credential is wrong)
                return BadRequest(new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid email or password" 
                });
            }

            // Generate JWT access token and refresh token for the authenticated user
            var token = await _jwtService.GenerateJwtToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Get the user's roles for the response
            var roles = await _userManager.GetRolesAsync(user);

            // Return successful login response with tokens and user info
            return Ok(new AuthResponse
            {
                Success = true,
                Token = token,                 // JWT access token
                RefreshToken = refreshToken,   // Refresh token
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),  // Token expiration time
                User = new UserDto
                {
                    Id = user.Id,              // User ID
                    FullName = user.FullName,  // User's full name
                    Email = user.Email!        // User's email
                },
                Roles = roles.ToList(),        // User's roles
                Message = "Login successful"   // Success message
            });
        }

        /// <summary>
        /// Logs out the current user
        /// POST: api/auth/logout
        /// Requires authentication
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]  // HTTP POST endpoint
        [Authorize]  // Requires valid JWT token
        public ActionResult Logout()
        {
            // In a real application, you might want to invalidate the refresh token
            // For now, we'll just return success since JWT tokens are stateless
            return Ok(new { Message = "Logout successful" });
        }

        /// <summary>
        /// Gets the current user's profile information
        /// GET: api/auth/profile
        /// Requires authentication
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet("profile")]  // HTTP GET endpoint
        [Authorize]  // Requires valid JWT token
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            // Extract user ID from the JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();  // Return 404 if user not found
            }

            // Return user profile information
            return Ok(new UserDto
            {
                Id = user.Id,              // User ID
                FullName = user.FullName,  // User's full name
                Email = user.Email!        // User's email
            });
        }

        /// <summary>
        /// Updates the current user's profile information
        /// PUT: api/auth/profile
        /// Requires authentication
        /// </summary>
        /// <param name="request">Profile update request containing new information</param>
        /// <returns>Updated user profile information</returns>
        [HttpPut("profile")]  // HTTP PUT endpoint
        [Authorize]  // Requires valid JWT token
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateProfileRequest request)
        {
            // Extract user ID from the JWT token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();  // Return 401 if no user ID found
            }

            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();  // Return 404 if user not found
            }

            // Check if email is being changed and if the new email is already taken
            if (user.Email != request.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "Email already exists" });  // Return 400 if email taken
                }
            }

            // Update user information
            user.FullName = request.FullName;  // Update full name
            user.Email = request.Email;        // Update email
            user.UserName = request.Email;     // Update username to match email

            // Save the changes to the database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                // If update fails, return validation errors
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { Message = errors });
            }

            // Return updated user profile information
            return Ok(new UserDto
            {
                Id = user.Id,              // User ID
                FullName = user.FullName,  // Updated full name
                Email = user.Email!        // Updated email
            });
        }

        /// <summary>
        /// Refreshes an expired JWT token using a refresh token
        /// POST: api/auth/refresh
        /// </summary>
        /// <param name="refreshToken">The refresh token to use for getting a new access token</param>
        /// <returns>New authentication response with fresh tokens</returns>
        [HttpPost("refresh")]  // HTTP POST endpoint
        public ActionResult<AuthResponse> RefreshToken([FromBody] string refreshToken)
        {
            // In a real application, you would validate the refresh token against a database
            // For now, we'll return an error indicating refresh tokens are not implemented
            return BadRequest(new AuthResponse 
            { 
                Success = false, 
                Message = "Refresh token functionality not implemented" 
            });
        }
    }
}
