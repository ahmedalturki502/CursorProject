using CursorProject.DTOs.Auth;
using CursorProject.Entities;
using CursorProject.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CursorProject.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<AuthResponse> LogoutAsync(string userId);
        Task<AuthResponse> GetProfileAsync(string userId);
        Task<AuthResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<AuthResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtHelper _jwtHelper;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            JwtHelper jwtHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Email already exists" 
                };
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = errors 
                };
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            var token = await _jwtHelper.GenerateJwtToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName
                }
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid email or password" 
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid email or password" 
                };
            }

            var token = await _jwtHelper.GenerateJwtToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName
                }
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid token" 
                };
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid token" 
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid refresh token" 
                };
            }

            var newToken = await _jwtHelper.GenerateJwtToken(user);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
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
