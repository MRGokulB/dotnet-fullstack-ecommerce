using DotnetEcommerce.Application.Common.Interfaces;
using DotnetEcommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetEcommerce.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(c => c.Slug).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Slug).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.ImageUrl).IsRequired().HasMaxLength(500);

            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.SessionId).IsRequired().HasMaxLength(128);
            entity.HasIndex(c => c.SessionId);

            entity.HasMany(c => c.Items)
                  .WithOne(i => i.Cart)
                  .HasForeignKey(i => i.CartId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.UnitPrice).HasPrecision(18, 2);

            entity.HasOne(i => i.Product)
                  .WithMany()
                  .HasForeignKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.CustomerEmail).IsRequired().HasMaxLength(256);
            entity.Property(o => o.CustomerName).IsRequired().HasMaxLength(150);
            entity.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(300);
            entity.Property(o => o.Subtotal).HasPrecision(18, 2);
            entity.Property(o => o.Tax).HasPrecision(18, 2);
            entity.Property(o => o.ShippingFee).HasPrecision(18, 2);
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(50);
            entity.HasIndex(o => o.StripeSessionId).IsUnique();
            entity.HasIndex(o => o.StripePaymentIntentId).IsUnique();

            entity.HasMany(o => o.Items)
                  .WithOne(i => i.Order)
                  .HasForeignKey(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(i => i.UnitPrice).HasPrecision(18, 2);
            entity.Property(i => i.TotalPrice).HasPrecision(18, 2);
        });

        var catHardwareId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var catAudioId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var catDeskId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Category>().HasData(
            Category.Create(catHardwareId, "Developer Hardware", "hardware", "Mechanical keyboards, macropads, and developer peripherals", "Cpu"),
            Category.Create(catAudioId, "Studio Audio", "audio", "High-fidelity noise cancelling monitors and microphones", "Headphones"),
            Category.Create(catDeskId, "Workspace & Desk", "workspace", "Ergonomic desk accessories and cable management tools", "Layers")
        );

        modelBuilder.Entity<Product>().HasData(
            Product.Create(
                Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                "Ergonomic Split Ortholinear Keyboard",
                "split-keyboard",
                "Custom CNC aluminum anodized casing with hot-swappable tactile switches and QMK/VIA firmware.",
                249.00m,
                "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=800&q=80",
                catHardwareId,
                25,
                4.9,
                88,
                true
            ),
            Product.Create(
                Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                "Hi-Fi Planar Magnetic Headphones",
                "planar-headphones",
                "Open-back audiophile planar drivers delivering pristine frequency response for deep focus work.",
                399.00m,
                "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=800&q=80",
                catAudioId,
                15,
                4.8,
                64,
                true
            ),
            Product.Create(
                Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                "Precision Anodized Macropad 12K",
                "macropad-12k",
                "12 programmable mechanical keys with rotary aluminum dial for volume, timeline scrubbing, and git shortcuts.",
                119.00m,
                "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?auto=format&fit=crop&w=800&q=80",
                catHardwareId,
                40,
                4.7,
                35,
                false
            ),
            Product.Create(
                Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                "Walnut Monitor Riser & Desk Shelf",
                "walnut-monitor-shelf",
                "Solid American walnut with integrated felt tray and anodized aluminum legs.",
                149.00m,
                "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=800&q=80",
                catDeskId,
                30,
                4.9,
                112,
                true
            ),
            Product.Create(
                Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                "Broadcast Dynamic USB-C / XLR Microphone",
                "broadcast-microphone",
                "Cardioid capsule with built-in DSP, analog limiter, and headphone zero-latency monitoring.",
                189.00m,
                "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?auto=format&fit=crop&w=800&q=80",
                catAudioId,
                20,
                4.8,
                78,
                false
            ),
            Product.Create(
                Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                "Magnetic MagSafe Charging Station",
                "magsafe-charging-station",
                "3-in-1 fast wireless charging dock constructed from machined aircraft-grade alloy.",
                89.00m,
                "https://images.unsplash.com/photo-1586953208448-b95a79798f07?auto=format&fit=crop&w=800&q=80",
                catDeskId,
                50,
                4.6,
                52,
                false
            )
        );
    }
}
