using CursorProject.DTOs;  // Import authentication data transfer objects
using CursorProject.Entities;  // Import domain entities
using CursorProject.Helpers;  // Import utility helper classes
using Microsoft.AspNetCore.Identity;  // Import ASP.NET Core Identity framework
using Microsoft.AspNetCore.Mvc;  // Import MVC framework components
using System.Security.Claims;  // Import security claims for JWT tokens

namespace CursorProject.Services  // Define namespace for business logic services
{
    // Interface defining the contract for authentication service operations
    // This interface allows for dependency injection and unit testing
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);  // Register new user account
        Task<AuthResponse> LoginAsync(LoginRequest request);  // Authenticate existing user
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);  // Renew JWT token using refresh token
        Task<AuthResponse> LogoutAsync(string userId);  // Logout user and invalidate tokens
        Task<AuthResponse> GetProfileAsync(string userId);  // Retrieve user profile information
        Task<AuthResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request);  // Update user profile
        Task<AuthResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request);  // Change user password
    }

    // Implementation of authentication service that handles user authentication and authorization
    // This class contains all business logic for user management operations
    public class AuthService : IAuthService  // Implement the IAuthService interface
    {
        // Dependency injection fields for required services
        private readonly UserManager<ApplicationUser> _userManager;  // Service for user management operations
        private readonly SignInManager<ApplicationUser> _signInManager;  // Service for sign-in operations
        private readonly RoleManager<ApplicationRole> _roleManager;  // Service for role management operations
        private readonly JwtHelper _jwtHelper;  // Utility service for JWT token operations

        // Constructor for dependency injection of required services
        // These services are automatically provided by the DI container
        public AuthService(
            UserManager<ApplicationUser> userManager,  // Inject user management service
            SignInManager<ApplicationUser> signInManager,  // Inject sign-in management service
            RoleManager<ApplicationRole> roleManager,  // Inject role management service
            JwtHelper jwtHelper)  // Inject JWT helper utility
        {
            _userManager = userManager;  // Store user manager reference
            _signInManager = signInManager;  // Store sign-in manager reference
            _roleManager = roleManager;  // Store role manager reference
            _jwtHelper = jwtHelper;  // Store JWT helper reference
        }

        // Register a new user account in the system
        // This method handles user registration with validation and role assignment
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if a user with the provided email already exists in the database
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)  // If user already exists, return error response
            {
                return new AuthResponse  // Create error response object
                { 
                    Success = false,  // Set success flag to false
                    Message = "Email already exists"  // Set error message
                };
            }

            // Create new user entity with provided registration data
            var user = new ApplicationUser
            {
                UserName = request.Email,  // Set username to email address
                Email = request.Email,  // Set email address
                FullName = request.FullName  // Set user's full name
            };

            // Attempt to create the user account with the provided password
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)  // If user creation failed, return error response
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));  // Combine all error messages
                return new AuthResponse  // Create error response object
                { 
                    Success = false,  // Set success flag to false
                    Message = errors  // Set error message with validation details
                };
            }

            // Assign default "User" role to the newly created user account
            await _userManager.AddToRoleAsync(user, "User");

            // Generate JWT access token for immediate authentication
            var token = await _jwtHelper.GenerateJwtToken(user);
            // Generate refresh token for future token renewal
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            // Store refresh token and its expiration time in the user entity
            user.RefreshToken = refreshToken;  // Set the refresh token
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);  // Set expiration to 7 days from now
            await _userManager.UpdateAsync(user);  // Save changes to database

            // Return successful registration response with tokens and user data
            return new AuthResponse
            {
                Success = true,  // Set success flag to true
                Message = "Registration successful",  // Set success message
                Token = token,  // Include JWT access token
                RefreshToken = refreshToken,  // Include refresh token
                User = new UserDto  // Create user data transfer object
                {
                    Id = user.Id,  // Set user ID
                    Email = user.Email!,  // Set email (null-forgiving operator used)
                    FullName = user.FullName  // Set full name
                }
            };
        }

        // Authenticate an existing user and provide access tokens
        // This method handles user login with credential validation
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // Find user by email address in the database
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)  // If user not found, return error response
            {
                return new AuthResponse  // Create error response object
                { 
                    Success = false,  // Set success flag to false
                    Message = "Invalid email or password"  // Set generic error message for security
                };
            }

            // Verify the provided password against the user's stored password hash
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)  // If password verification failed, return error response
            {
                return new AuthResponse  // Create error response object
                { 
                    Success = false,  // Set success flag to false
                    Message = "Invalid email or password"  // Set generic error message for security
                };
            }

            // Generate new JWT access token for the authenticated user
            var token = await _jwtHelper.GenerateJwtToken(user);
            // Generate new refresh token for future token renewal
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            // Update user's refresh token and expiration time in the database
            user.RefreshToken = refreshToken;  // Set the new refresh token
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);  // Set expiration to 7 days from now
            await _userManager.UpdateAsync(user);  // Save changes to database

            // Return successful login response with tokens and user data
            return new AuthResponse
            {
                Success = true,  // Set success flag to true
                Message = "Login successful",  // Set success message
                Token = token,  // Include JWT access token
                RefreshToken = refreshToken,  // Include refresh token
                User = new UserDto  // Create user data transfer object
                {
                    Id = user.Id,  // Set user ID
                    Email = user.Email!,  // Set email (null-forgiving operator used)
                    FullName = user.FullName  // Set full name
                }
            };
        }

        // Renew JWT access token using a valid refresh token
        // This method allows users to get new access tokens without re-entering credentials
        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // Extract user information from the expired JWT token
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)  // If token extraction failed, return error response
            {
                return new AuthResponse  // Create error response object
                { 
                    Success = false,  // Set success flag to false
                    Message = "Invalid token"  // Set error message
                };
            }

            // Extract user ID from the token claims
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))  // If user ID is missing, return error response
            {
                return new AuthResponse  // Create error response object
                { 
                    Success = false,  // Set success flag to false
                    Message = "Invalid token"  // Set error message
                };
            }

            // Find the user in the database using the extracted user ID
            var user = await _userManager.FindByIdAsync(userId);
            // Validate refresh token: check if user exists, refresh token matches, and hasn't expired
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponse  // Create error response object
                { 
                    Success = false,  // Set success flag to false
                    Message = "Invalid refresh token"  // Set error message
                };
            }

            // Generate new JWT access token for the user
            var newToken = await _jwtHelper.GenerateJwtToken(user);
            // Generate new refresh token to replace the old one
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            // Update user's refresh token and expiration time in the database
            user.RefreshToken = newRefreshToken;  // Set the new refresh token
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);  // Set expiration to 7 days from now
            await _userManager.UpdateAsync(user);  // Save changes to database

            // Return successful token refresh response with new tokens
            return new AuthResponse
            {
                Success = true,  // Set success flag to true
                Message = "Token refreshed successfully",  // Set success message
                Token = newToken,
                RefreshToken = newRefreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName
                }
            };
        }

        public async Task<AuthResponse> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userManager.UpdateAsync(user);
            }

            return new AuthResponse
            {
                Success = true,
                Message = "Logout successful"
            };
        }

        public async Task<AuthResponse> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "User not found" 
                };
            }

            return new AuthResponse
            {
                Success = true,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName
                }
            };
        }

        public async Task<AuthResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "User not found" 
                };
            }

            user.FullName = request.FullName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = errors 
                };
            }

            return new AuthResponse
            {
                Success = true,
                Message = "Profile updated successfully",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName
                }
            };
        }

        public async Task<AuthResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "User not found" 
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = errors 
                };
            }

            return new AuthResponse
            {
                Success = true,
                Message = "Password changed successfully"
            };
        }
    }
}
