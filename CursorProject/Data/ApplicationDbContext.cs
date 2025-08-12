// Import necessary namespaces for the database context
using CursorProject.Entities;                    // Application models (User, Product, Order, etc.)
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;  // Identity with EF Core
using Microsoft.EntityFrameworkCore;         // Entity Framework Core

// Namespace for data access layer
namespace CursorProject.Data
{
    /// <summary>
    /// Main database context for the e-commerce application
    /// Extends IdentityDbContext to include ASP.NET Identity functionality
    /// Manages all database operations and entity relationships
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        /// <summary>
        /// Constructor that accepts database configuration options
        /// Passes the options to the base IdentityDbContext constructor
        /// </summary>
        /// <param name="options">Database configuration options (connection string, provider, etc.)</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)  // Call base constructor with the provided options
        {
        }

        // Database table sets (DbSets) - these represent the tables in the database
        /// <summary>
        /// Categories table - stores product categories
        /// </summary>
        public DbSet<Category> Categories { get; set; }
        
        /// <summary>
        /// Products table - stores all available products
        /// </summary>
        public DbSet<Product> Products { get; set; }
        
        /// <summary>
        /// Orders table - stores customer orders
        /// </summary>
        public DbSet<Order> Orders { get; set; }
        
        /// <summary>
        /// OrderItems table - stores individual items within orders
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }
        
        /// <summary>
        /// Carts table - stores user shopping carts
        /// </summary>
        public DbSet<Cart> Carts { get; set; }
        
        /// <summary>
        /// CartItems table - stores individual items within shopping carts
        /// </summary>
        public DbSet<CartItem> CartItems { get; set; }

        /// <summary>
        /// Configures the database model, relationships, constraints, and seed data
        /// Called by Entity Framework when creating the database schema
        /// </summary>
        /// <param name="builder">Model builder for configuring the database model</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call base method to configure Identity tables
            base.OnModelCreating(builder);

            // Configure entity relationships using Fluent API
            // Product to Category relationship (Many-to-One)
            builder.Entity<Product>()
                .HasOne(p => p.Category)           // Product has one Category
                .WithMany(c => c.Products)         // Category has many Products
                .HasForeignKey(p => p.CategoryId)  // Foreign key property
                .OnDelete(DeleteBehavior.Restrict); // Prevent category deletion if products exist

            // Order to User relationship (Many-to-One)
            builder.Entity<Order>()
                .HasOne(o => o.User)               // Order has one User
                .WithMany(u => u.Orders)           // User has many Orders
                .HasForeignKey(o => o.UserId)      // Foreign key property
                .OnDelete(DeleteBehavior.Cascade); // Delete orders when user is deleted

            // OrderItem to Order relationship (Many-to-One)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)            // OrderItem has one Order
                .WithMany(o => o.OrderItems)       // Order has many OrderItems
                .HasForeignKey(oi => oi.OrderId)   // Foreign key property
                .OnDelete(DeleteBehavior.Cascade); // Delete order items when order is deleted

            // OrderItem to Product relationship (Many-to-One)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)          // OrderItem has one Product
                .WithMany(p => p.OrderItems)       // Product has many OrderItems
                .HasForeignKey(oi => oi.ProductId) // Foreign key property
                .OnDelete(DeleteBehavior.Restrict); // Prevent product deletion if referenced in orders

            // Cart to User relationship (One-to-One)
            builder.Entity<Cart>()
                .HasOne(c => c.User)               // Cart has one User
                .WithOne(u => u.Cart)              // User has one Cart
                .HasForeignKey<Cart>(c => c.UserId) // Foreign key property
                .OnDelete(DeleteBehavior.Cascade); // Delete cart when user is deleted

            // CartItem to Cart relationship (Many-to-One)
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)             // CartItem has one Cart
                .WithMany(c => c.CartItems)        // Cart has many CartItems
                .HasForeignKey(ci => ci.CartId)    // Foreign key property
                .OnDelete(DeleteBehavior.Cascade); // Delete cart items when cart is deleted

            // CartItem to Product relationship (Many-to-One)
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)          // CartItem has one Product
                .WithMany(p => p.CartItems)        // Product has many CartItems
                .HasForeignKey(ci => ci.ProductId) // Foreign key property
                .OnDelete(DeleteBehavior.Restrict); // Prevent product deletion if in carts

            // Configure unique constraints to ensure data integrity
            // Category names must be unique
            builder.Entity<Category>()
                .HasIndex(c => c.Name)             // Create index on Name column
                .IsUnique();                       // Make the index unique

            // Product names must be unique
            builder.Entity<Product>()
                .HasIndex(p => p.Name)             // Create index on Name column
                .IsUnique();                       // Make the index unique

            // Seed initial data into the database
            SeedData(builder);
        }

        /// <summary>
        /// Seeds the database with initial data (roles, categories, products)
        /// This data is created when the database is first created
        /// </summary>
        /// <param name="builder">Model builder for configuring seed data</param>
        private void SeedData(ModelBuilder builder)
        {
            // Seed default roles for the application
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = "1", Name = "Customer", NormalizedName = "CUSTOMER" },  // Customer role
                new ApplicationRole { Id = "2", Name = "Admin", NormalizedName = "ADMIN" }         // Admin role
            );

            // Seed default product categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },      // Electronics category
                new Category { Id = 2, Name = "Clothing" },         // Clothing category
                new Category { Id = 3, Name = "Books" },            // Books category
                new Category { Id = 4, Name = "Home & Garden" }     // Home & Garden category
            );

            // Seed sample products for testing and demonstration
            builder.Entity<Product>().HasData(
                // Smartphone product
                new Product 
                { 
                    Id = 1, 
                    Name = "Smartphone", 
                    Description = "Latest smartphone with advanced features", 
                    Price = 599.99m, 
                    ImageUrl = "https://via.placeholder.com/300x300?text=Smartphone", 
                    StockQuantity = 50, 
                    CategoryId = 1  // Electronics category
                },
                // Laptop product
                new Product 
                { 
                    Id = 2, 
                    Name = "Laptop", 
                    Description = "High-performance laptop for work and gaming", 
                    Price = 1299.99m, 
                    ImageUrl = "https://via.placeholder.com/300x300?text=Laptop", 
                    StockQuantity = 25, 
                    CategoryId = 1  // Electronics category
                },
                // T-Shirt product
                new Product 
                { 
                    Id = 3, 
                    Name = "T-Shirt", 
                    Description = "Comfortable cotton t-shirt", 
                    Price = 19.99m, 
                    ImageUrl = "https://via.placeholder.com/300x300?text=T-Shirt", 
                    StockQuantity = 100, 
                    CategoryId = 2  // Clothing category
                },
                // Programming book product
                new Product 
                { 
                    Id = 4, 
                    Name = "Programming Book", 
                    Description = "Learn C# and .NET development", 
                    Price = 49.99m, 
                    ImageUrl = "https://via.placeholder.com/300x300?text=Book", 
                    StockQuantity = 75, 
                    CategoryId = 3  // Books category
                }
            );
        }
    }
}
