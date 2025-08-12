using CursorProject.Entities;  // Import domain entities for user information
using Microsoft.AspNetCore.Identity;  // Import Identity framework for user management
using Microsoft.IdentityModel.Tokens;  // Import JWT token validation utilities
using System.IdentityModel.Tokens.Jwt;  // Import JWT token handling classes
using System.Security.Claims;  // Import security claims for token payload
using System.Text;  // Import text encoding utilities

namespace CursorProject.Helpers  // Define namespace for utility helper classes
{
    // JWT helper class that handles JWT token generation, validation, and refresh token operations
    // This class provides all JWT-related functionality for the authentication system
    public class JwtHelper
    {
        // Dependency injection fields for required services
        private readonly IConfiguration _configuration;  // Application configuration for JWT settings
        private readonly UserManager<ApplicationUser> _userManager;  // User management service for role retrieval

        // Constructor for dependency injection of required services
        public JwtHelper(IConfiguration configuration, UserManager<ApplicationUser> userManager)  // Inject configuration and user manager
        {
            _configuration = configuration;  // Store configuration reference
            _userManager = userManager;  // Store user manager reference
        }

        // Generate JWT access token for a user with their claims and roles
        // This method creates a secure token that contains user identity information
        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // Get user roles from the user manager service
            var roles = await _userManager.GetRolesAsync(user);  // Retrieve all roles assigned to the user
            
            // Create list of claims that will be included in the JWT token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),  // Add user ID claim for identification
                new Claim(ClaimTypes.Email, user.Email!),  // Add email claim (null-forgiving operator used)
                new Claim(ClaimTypes.Name, user.FullName)  // Add full name claim for display purposes
            };

            // Add role claims to the token for authorization purposes
            foreach (var role in roles)  // Iterate through all user roles
            {
                claims.Add(new Claim(ClaimTypes.Role, role));  // Add each role as a claim in the token
            }

            // Create symmetric security key from the secret key in configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));  // Convert secret key string to bytes
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);  // Create signing credentials with HMAC-SHA256 algorithm

            // Create JWT token with all necessary parameters
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],  // Set token issuer from configuration
                audience: _configuration["Jwt:Audience"],  // Set token audience from configuration
                claims: claims,  // Include all user claims in the token
                expires: DateTime.UtcNow.AddHours(1),  // Set token expiration to 1 hour from now
                signingCredentials: creds  // Use the signing credentials for token signature
            );

            // Convert JWT token to string format and return
            return new JwtSecurityTokenHandler().WriteToken(token);  // Serialize token to string
        }

        // Generate a refresh token for token renewal without re-authentication
        // This method creates a unique identifier that can be used to get new access tokens
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();  // Generate new GUID and convert to string for refresh token
        }

        // Extract user information from an expired JWT token for refresh token validation
        // This method validates the token signature and extracts claims without checking expiration
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            // Configure token validation parameters for expired token processing
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,  // Don't validate audience for expired tokens
                ValidateIssuer = false,  // Don't validate issuer for expired tokens
                ValidateIssuerSigningKey = true,  // Always validate the signing key for security
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),  // Use the same secret key
                ValidateLifetime = false  // Don't validate lifetime since we expect expired tokens
            };

            // Create JWT token handler for token processing
            var tokenHandler = new JwtSecurityTokenHandler();  // Handler for JWT token operations
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);  // Validate token and extract principal

            // Verify that the token is a valid JWT token with correct algorithm
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");  // Throw exception for invalid tokens

            return principal;  // Return the extracted claims principal
        }
    }
}
