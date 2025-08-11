// Import necessary namespaces for the application
using CursorProject.Data;           // Database context and data access
using CursorProject.Models;         // Entity models (User, Product, Order, etc.)
using CursorProject.Services;       // Custom services (JWT service)
using Microsoft.AspNetCore.Authentication.JwtBearer;  // JWT authentication
using Microsoft.AspNetCore.Identity;                  // User identity management
using Microsoft.EntityFrameworkCore;                // Entity Framework Core
using Microsoft.IdentityModel.Tokens;                 // JWT token validation
using Microsoft.OpenApi.Models;                       // Swagger/OpenAPI documentation
using System.Text;                                    // String encoding utilities

// Create the web application builder with configuration
var builder = WebApplication.CreateBuilder(args);

// Add MVC controllers to the service container for handling HTTP requests
builder.Services.AddControllers();

// Configure Entity Framework Core with SQL Server database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Identity for user authentication and authorization
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password requirements for user accounts
    options.Password.RequireDigit = true;              // Must contain at least one digit
    options.Password.RequireLowercase = true;          // Must contain at least one lowercase letter
    options.Password.RequireUppercase = true;          // Must contain at least one uppercase letter
    options.Password.RequireNonAlphanumeric = true;    // Must contain at least one special character
    options.Password.RequiredLength = 6;               // Minimum password length
    options.User.RequireUniqueEmail = true;            // Email addresses must be unique
})
.AddEntityFrameworkStores<ApplicationDbContext>()      // Use EF Core for data storage
.AddDefaultTokenProviders();                           // Add default token providers for password reset, etc.

// Configure JWT (JSON Web Token) Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");  // Get JWT settings from configuration
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);     // Convert secret key to bytes

// Set up authentication with JWT Bearer as the default scheme
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Use JWT for authentication
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // Use JWT for challenges
})
.AddJwtBearer(options =>
{
    // Configure JWT token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,                    // Validate the token issuer
        ValidateAudience = true,                  // Validate the token audience
        ValidateLifetime = true,                  // Validate token expiration
        ValidateIssuerSigningKey = true,          // Validate the signing key
        ValidIssuer = jwtSettings["Issuer"],      // Expected issuer from configuration
        ValidAudience = jwtSettings["Audience"],  // Expected audience from configuration
        IssuerSigningKey = new SymmetricSecurityKey(key),  // Key used to validate token signature
        ClockSkew = TimeSpan.Zero                 // No tolerance for clock differences
    };
});

// Register custom services in the dependency injection container
builder.Services.AddScoped<JwtService>();  // JWT service for token generation and validation

// Configure Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();  // Enable API endpoint discovery
builder.Services.AddSwaggerGen(c =>
{
    // Configure the main Swagger document
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "E-Commerce API",           // API title
        Version = "v1",                     // API version
        Description = "A full-stack e-commerce application API"  // API description
    });

    // Configure JWT authentication in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",              // Header name
        In = ParameterLocation.Header,       // Parameter location
        Type = SecuritySchemeType.ApiKey,    // Security scheme type
        Scheme = "Bearer"                    // Authentication scheme
    });

    // Add security requirement to all endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,  // Reference type
                    Id = "Bearer"                         // Reference ID
                }
            },
            Array.Empty<string>()  // No specific scopes required
        }
    });
});

// Configure CORS (Cross-Origin Resource Sharing) for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        // Allow requests from Angular development server
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()      // Allow any HTTP headers
              .AllowAnyMethod()      // Allow any HTTP methods (GET, POST, PUT, DELETE)
              .AllowCredentials();   // Allow credentials (cookies, authorization headers)
    });
});

// Build the web application
var app = builder.Build();

// Configure the HTTP request pipeline (middleware)
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development environment
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1");  // Swagger UI endpoint
    });
}

// Redirect HTTP requests to HTTPS for security
app.UseHttpsRedirection();

// Enable CORS with the configured policy
app.UseCors("AllowAngularApp");

// Enable authentication and authorization middleware
app.UseAuthentication();  // Must come before authorization
app.UseAuthorization();   // Must come after authentication

// Map controller routes
app.MapControllers();

// Database initialization and seeding
using (var scope = app.Services.CreateScope())
{
    // Get required services from the service provider
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();      // Database context
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();  // User manager
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();  // Role manager

    // Ensure the database exists and is up to date
    try
    {
        // Try to ensure the database is created (this will fail if it already exists, but that's okay)
        context.Database.EnsureCreated();
    }
    catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1801) // Database already exists
    {
        // Database already exists, which is fine - just continue
        Console.WriteLine("Database already exists, continuing...");
    }

    // Create default admin user if no users exist in the database
    if (!await userManager.Users.AnyAsync())
    {
        // Create admin user with default credentials
        var adminUser = new ApplicationUser
        {
            UserName = "admin@example.com",    // Username
            Email = "admin@example.com",       // Email address
            FullName = "Admin User",           // Full name
            EmailConfirmed = true              // Mark email as confirmed
        };

        // Create the user with password
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            // Assign admin role to the user
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// Start the web application
app.Run();
