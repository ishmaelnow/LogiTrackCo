using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;

public class LogiTrackContext : IdentityDbContext<ApplicationUser>
{
    // ✅ Regular tables for your domain models
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    // ✅ Required constructor for proper DI and EF Core design-time tools
    public LogiTrackContext(DbContextOptions<LogiTrackContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ✅ Ensures Identity-related tables and relationships are configured properly
        base.OnModelCreating(modelBuilder);

        // ✅ Configure relationship between InventoryItem and Order
        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.SetNull);  // If Order is deleted, nullify the reference in InventoryItem
    }
}
