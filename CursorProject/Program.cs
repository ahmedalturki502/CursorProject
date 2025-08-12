using CursorProject.Data;           // Database context for Entity Framework operations
using CursorProject.Entities;         // Domain entities (User, Product, Order, etc.)
using CursorProject.Services;       // Business logic service implementations
using CursorProject.Interfaces;     // Service contract interfaces
using CursorProject.Repositories;   // Data access repository implementations
using CursorProject.Helpers;        // Utility helper classes
using Microsoft.AspNetCore.Authentication.JwtBearer;  // JWT authentication middleware
using Microsoft.AspNetCore.Identity;                  // ASP.NET Core Identity framework
using Microsoft.EntityFrameworkCore;                // Entity Framework Core ORM
using Microsoft.IdentityModel.Tokens;                 // JWT token validation utilities
using Microsoft.OpenApi.Models;                       // Swagger/OpenAPI documentation models
using System.Text;                                    // Text encoding utilities

// Create the web application builder with configuration from appsettings.json
var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers in the dependency injection container for handling HTTP requests
builder.Services.AddControllers();

// Configure Entity Framework Core to use SQL Server with connection string from configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity for user authentication, authorization, and management
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Configure password complexity requirements for user accounts
    options.Password.RequireDigit = true;              // Password must contain at least one number (0-9)
    options.Password.RequireLowercase = true;          // Password must contain at least one lowercase letter (a-z)
    options.Password.RequireUppercase = true;          // Password must contain at least one uppercase letter (A-Z)
    options.Password.RequireNonAlphanumeric = true;    // Password must contain at least one special character (!@#$%^&*)
    options.Password.RequiredLength = 6;               // Minimum password length requirement
    options.User.RequireUniqueEmail = true;            // Each user must have a unique email address
})
.AddEntityFrameworkStores<ApplicationDbContext>()      // Use Entity Framework Core for storing user data
.AddDefaultTokenProviders();                           // Add default token providers for password reset, email confirmation, etc.

// Configure JWT (JSON Web Token) authentication settings
var jwtSettings = builder.Configuration.GetSection("Jwt");  // Extract JWT configuration section from appsettings.json
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);     // Convert the secret key string to byte array for signing

// Configure authentication to use JWT Bearer tokens as the default authentication scheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Use JWT for user authentication
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // Use JWT for authentication challenges
})
.AddJwtBearer(options =>
{
    // Configure JWT token validation parameters for security
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,                    // Verify that the token was issued by the expected issuer
        ValidateAudience = true,                  // Verify that the token is intended for the expected audience
        ValidateLifetime = true,                  // Verify that the token has not expired
        ValidateIssuerSigningKey = true,          // Verify the token signature using the signing key
        ValidIssuer = jwtSettings["Issuer"],      // The expected issuer name from configuration
        ValidAudience = jwtSettings["Audience"],  // The expected audience name from configuration
        IssuerSigningKey = new SymmetricSecurityKey(key),  // The secret key used to validate token signatures
        ClockSkew = TimeSpan.Zero                 // No tolerance for clock time differences between servers
    };
});

// Register custom business logic services in the dependency injection container
builder.Services.AddScoped<JwtHelper>();  // JWT utility service for token generation and validation
builder.Services.AddScoped<CursorProject.Services.IAuthService, AuthService>();  // User authentication and authorization service
builder.Services.AddScoped<CursorProject.Services.IProductService, ProductService>();  // Product catalog management service
builder.Services.AddScoped<CursorProject.Services.ICartService, CartService>();  // Shopping cart management service
builder.Services.AddScoped<CursorProject.Services.IOrderService, OrderService>();  // Order processing and management service
builder.Services.AddScoped<CursorProject.Services.ICategoryService, CategoryService>();  // Product category management service

// Register data access layer services (Repository pattern and Unit of Work)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();  // Unit of Work pattern for transaction management
builder.Services.AddScoped<IProductRepository, ProductRepository>();  // Product data access repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();  // Category data access repository
builder.Services.AddScoped<IOrderRepository, OrderRepository>();  // Order data access repository

// Configure Swagger/OpenAPI documentation for API testing and documentation
builder.Services.AddEndpointsApiExplorer();  // Enable automatic discovery of API endpoints
builder.Services.AddSwaggerGen(c =>
{
    // Configure the main Swagger document information
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "E-Commerce API",           // The title of the API documentation
        Version = "v1",                     // The version of the API
        Description = "A full-stack e-commerce application API"  // Description of what the API does
    });

    // Configure JWT Bearer authentication in Swagger UI for testing protected endpoints
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",              // The name of the HTTP header
        In = ParameterLocation.Header,       // The location of the parameter (HTTP header)
        Type = SecuritySchemeType.ApiKey,    // The type of security scheme
        Scheme = "Bearer"                    // The authentication scheme name
    });

    // Add security requirement to all API endpoints that require authentication
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,  // Reference to a security scheme
                    Id = "Bearer"                         // The ID of the security scheme to reference
                }
            },
            Array.Empty<string>()  // No specific scopes are required for this API
        }
    });
});

// Configure CORS (Cross-Origin Resource Sharing) to allow frontend applications to access the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        // Allow requests from Angular development server URLs
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()      // Allow any HTTP headers in requests
              .AllowAnyMethod()      // Allow any HTTP methods (GET, POST, PUT, DELETE, etc.)
              .AllowCredentials();   // Allow credentials like cookies and authorization headers
    });
});

// Build the web application instance with all configured services
var app = builder.Build();

// Configure the HTTP request pipeline (middleware) for the application
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI only in development environment for API testing
    app.UseSwagger();  // Enable Swagger JSON endpoint
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1");  // Configure Swagger UI endpoint
    });
}

// Redirect all HTTP requests to HTTPS for security (encrypts data in transit)
app.UseHttpsRedirection();

// Enable CORS with the configured policy to allow cross-origin requests
app.UseCors("AllowAngularApp");

// Enable authentication middleware to process authentication on each request
app.UseAuthentication();  // Must be called before authorization middleware

// Enable authorization middleware to enforce access control policies
app.UseAuthorization();   // Must be called after authentication middleware

// Map controller routes to handle HTTP requests to API endpoints
app.MapControllers();

// Initialize and seed the database with initial data
using (var scope = app.Services.CreateScope())
{
    // Get required services from the dependency injection container
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();      // Database context for EF Core
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();  // User management service
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();  // Role management service

    // Ensure the database exists and is properly initialized
    try
    {
        // Create the database if it doesn't exist (this will fail if database already exists, which is fine)
        context.Database.EnsureCreated();
    }
    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1801) // Database already exists error
    {
        // Database already exists, which is expected behavior - continue with seeding
        Console.WriteLine("Database already exists, continuing...");
    }

    // Seed the database with initial roles and admin user
    await SeedData.SeedRolesAsync(roleManager);  // Create default roles (Admin, User)
    await SeedData.SeedAdminUserAsync(userManager);  // Create default admin user account
}

// Start the web application and begin listening for HTTP requests
app.Run();
