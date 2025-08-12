using CursorProject.Entities;  // Import domain entities for database mapping
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;  // Import Identity database context base class
using Microsoft.EntityFrameworkCore;  // Import Entity Framework Core for database operations

namespace CursorProject.Data  // Define namespace for data access layer
{
    // Database context class that manages all database operations and entity relationships
    // This class extends IdentityDbContext to include ASP.NET Core Identity functionality
    // It serves as the main entry point for Entity Framework Core database operations
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>  // Inherit from IdentityDbContext with custom user and role types
    {
        // Constructor for dependency injection of database configuration options
        // This constructor is called by the DI container with connection string and other settings
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)  // Accept database configuration options
            : base(options)  // Pass options to base IdentityDbContext constructor
        {
        }

        // Database table for products in the e-commerce system
        // This DbSet represents the Products table and provides query capabilities
        public DbSet<Product> Products { get; set; } = null!;  // Null-forgiving operator tells compiler this will be initialized

        // Database table for product categories
        // This DbSet represents the Categories table for organizing products
        public DbSet<Category> Categories { get; set; } = null!;  // Null-forgiving operator tells compiler this will be initialized

        // Database table for customer orders
        // This DbSet represents the Orders table for tracking customer purchases
        public DbSet<Order> Orders { get; set; } = null!;  // Null-forgiving operator tells compiler this will be initialized

        // Database table for individual items within orders
        // This DbSet represents the OrderItems table for order line items
        public DbSet<OrderItem> OrderItems { get; set; } = null!;  // Null-forgiving operator tells compiler this will be initialized

        // Database table for shopping carts
        // This DbSet represents the Carts table for temporary shopping cart storage
        public DbSet<Cart> Carts { get; set; } = null!;  // Null-forgiving operator tells compiler this will be initialized

        // Database table for items within shopping carts
        // This DbSet represents the CartItems table for cart line items
        public DbSet<CartItem> CartItems { get; set; } = null!;  // Null-forgiving operator tells compiler this will be initialized

        // Database table for user roles (inherited from IdentityDbContext)
        // This DbSet represents the Roles table for role-based authorization
        // Note: Roles is inherited from IdentityDbContext, so we don't need to declare it again

        // Configure entity relationships and database constraints during model creation
        // This method is called by Entity Framework when building the database model
        protected override void OnModelCreating(ModelBuilder modelBuilder)  // Override base method to customize model
        {
            // Call base OnModelCreating to configure Identity tables and relationships
            base.OnModelCreating(modelBuilder);  // Configure ASP.NET Core Identity tables first

            // Configure Product entity relationships and constraints
            modelBuilder.Entity<Product>(entity =>  // Configure Product entity
            {
                // Set primary key for Product entity
                entity.HasKey(p => p.Id);  // Configure Id as primary key
                
                // Configure required fields for Product entity
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);  // Name is required with max 200 characters
                entity.Property(p => p.Description).HasMaxLength(1000);  // Description has max 1000 characters
                entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");  // Price is required with decimal precision
                entity.Property(p => p.ImageUrl).HasMaxLength(500);  // ImageUrl has max 500 characters
                entity.Property(p => p.StockQuantity).IsRequired();  // StockQuantity is required
                
                // Configure foreign key relationship to Category
                entity.HasOne(p => p.Category)  // Product has one Category
                    .WithMany(c => c.Products)  // Category has many Products
                    .HasForeignKey(p => p.CategoryId)  // Foreign key is CategoryId
                    .OnDelete(DeleteBehavior.Restrict);  // Prevent category deletion if products exist
                
                // Configure one-to-many relationship with OrderItems
                entity.HasMany(p => p.OrderItems)  // Product has many OrderItems
                    .WithOne(oi => oi.Product)  // OrderItem has one Product
                    .HasForeignKey(oi => oi.ProductId)  // Foreign key is ProductId
                    .OnDelete(DeleteBehavior.Restrict);  // Prevent product deletion if order items exist
                
                // Configure one-to-many relationship with CartItems
                entity.HasMany(p => p.CartItems)  // Product has many CartItems
                    .WithOne(ci => ci.Product)  // CartItem has one Product
                    .HasForeignKey(ci => ci.ProductId)  // Foreign key is ProductId
                    .OnDelete(DeleteBehavior.Cascade);  // Delete cart items when product is deleted
            });

            // Configure Category entity relationships and constraints
            modelBuilder.Entity<Category>(entity =>  // Configure Category entity
            {
                // Set primary key for Category entity
                entity.HasKey(c => c.Id);  // Configure Id as primary key
                
                // Configure required fields for Category entity
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);  // Name is required with max 100 characters
                entity.Property(c => c.Description).HasMaxLength(500);  // Description has max 500 characters
                
                // Configure one-to-many relationship with Products
                entity.HasMany(c => c.Products)  // Category has many Products
                    .WithOne(p => p.Category)  // Product has one Category
                    .HasForeignKey(p => p.CategoryId)  // Foreign key is CategoryId
                    .OnDelete(DeleteBehavior.Restrict);  // Prevent category deletion if products exist
            });

            // Configure Order entity relationships and constraints
            modelBuilder.Entity<Order>(entity =>  // Configure Order entity
            {
                // Set primary key for Order entity
                entity.HasKey(o => o.Id);  // Configure Id as primary key
                
                // Configure required fields for Order entity
                entity.Property(o => o.UserId).IsRequired();  // UserId is required
                entity.Property(o => o.Status).IsRequired();  // Status is required
                entity.Property(o => o.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");  // TotalAmount is required with decimal precision
                entity.Property(o => o.OrderDate).IsRequired();  // OrderDate is required
                entity.Property(o => o.UpdatedDate).IsRequired();  // UpdatedDate is required
                entity.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);  // ShippingAddress is required with max 500 characters
                entity.Property(o => o.PhoneNumber).IsRequired().HasMaxLength(20);  // PhoneNumber is required with max 20 characters
                entity.Property(o => o.Notes).HasMaxLength(1000);  // Notes has max 1000 characters
                
                // Configure foreign key relationship to ApplicationUser
                entity.HasOne(o => o.User)  // Order has one User
                    .WithMany(u => u.Orders)  // User has many Orders
                    .HasForeignKey(o => o.UserId)  // Foreign key is UserId
                    .OnDelete(DeleteBehavior.Cascade);  // Delete orders when user is deleted
                
                // Configure one-to-many relationship with OrderItems
                entity.HasMany(o => o.OrderItems)  // Order has many OrderItems
                    .WithOne(oi => oi.Order)  // OrderItem has one Order
                    .HasForeignKey(oi => oi.OrderId)  // Foreign key is OrderId
                    .OnDelete(DeleteBehavior.Cascade);  // Delete order items when order is deleted
            });

            // Configure OrderItem entity relationships and constraints
            modelBuilder.Entity<OrderItem>(entity =>  // Configure OrderItem entity
            {
                // Set primary key for OrderItem entity
                entity.HasKey(oi => oi.Id);  // Configure Id as primary key
                
                // Configure required fields for OrderItem entity
                entity.Property(oi => oi.OrderId).IsRequired();  // OrderId is required
                entity.Property(oi => oi.ProductId).IsRequired();  // ProductId is required
                entity.Property(oi => oi.Quantity).IsRequired();  // Quantity is required
                entity.Property(oi => oi.Price).IsRequired().HasColumnType("decimal(18,2)");  // Price is required with decimal precision
                
                // Configure foreign key relationship to Order
                entity.HasOne(oi => oi.Order)  // OrderItem has one Order
                    .WithMany(o => o.OrderItems)  // Order has many OrderItems
                    .HasForeignKey(oi => oi.OrderId)  // Foreign key is OrderId
                    .OnDelete(DeleteBehavior.Cascade);  // Delete order item when order is deleted
                
                // Configure foreign key relationship to Product
                entity.HasOne(oi => oi.Product)  // OrderItem has one Product
                    .WithMany(p => p.OrderItems)  // Product has many OrderItems
                    .HasForeignKey(oi => oi.ProductId)  // Foreign key is ProductId
                    .OnDelete(DeleteBehavior.Restrict);  // Prevent order item deletion if product is deleted
            });

            // Configure Cart entity relationships and constraints
            modelBuilder.Entity<Cart>(entity =>  // Configure Cart entity
            {
                // Set primary key for Cart entity
                entity.HasKey(c => c.Id);  // Configure Id as primary key
                
                // Configure required fields for Cart entity
                entity.Property(c => c.UserId).IsRequired();  // UserId is required
                entity.Property(c => c.CreatedDate).IsRequired();  // CreatedDate is required
                entity.Property(c => c.UpdatedDate).IsRequired();  // UpdatedDate is required
                
                // Configure foreign key relationship to ApplicationUser
                entity.HasOne(c => c.User)  // Cart has one User
                    .WithOne(u => u.Cart)  // User has one Cart
                    .HasForeignKey<Cart>(c => c.UserId)  // Foreign key is UserId
                    .OnDelete(DeleteBehavior.Cascade);  // Delete cart when user is deleted
                
                // Configure one-to-many relationship with CartItems
                entity.HasMany(c => c.CartItems)  // Cart has many CartItems
                    .WithOne(ci => ci.Cart)  // CartItem has one Cart
                    .HasForeignKey(ci => ci.CartId)  // Foreign key is CartId
                    .OnDelete(DeleteBehavior.Cascade);  // Delete cart items when cart is deleted
            });

            // Configure CartItem entity relationships and constraints
            modelBuilder.Entity<CartItem>(entity =>  // Configure CartItem entity
            {
                // Set primary key for CartItem entity
                entity.HasKey(ci => ci.Id);  // Configure Id as primary key
                
                // Configure required fields for CartItem entity
                entity.Property(ci => ci.CartId).IsRequired();  // CartId is required
                entity.Property(ci => ci.ProductId).IsRequired();  // ProductId is required
                entity.Property(ci => ci.Quantity).IsRequired();  // Quantity is required
                entity.Property(ci => ci.Price).IsRequired().HasColumnType("decimal(18,2)");  // Price is required with decimal precision
                
                // Configure foreign key relationship to Cart
                entity.HasOne(ci => ci.Cart)  // CartItem has one Cart
                    .WithMany(c => c.CartItems)  // Cart has many CartItems
                    .HasForeignKey(ci => ci.CartId)  // Foreign key is CartId
                    .OnDelete(DeleteBehavior.Cascade);  // Delete cart item when cart is deleted
                
                // Configure foreign key relationship to Product
                entity.HasOne(ci => ci.Product)  // CartItem has one Product
                    .WithMany(p => p.CartItems)  // Product has many CartItems
                    .HasForeignKey(ci => ci.ProductId)  // Foreign key is ProductId
                    .OnDelete(DeleteBehavior.Cascade);  // Delete cart item when product is deleted
            });

            // Configure Role entity relationships and constraints
            // Note: Role configuration is handled by ASP.NET Core Identity
            // Custom role properties can be configured here if needed
        }
    }
}
