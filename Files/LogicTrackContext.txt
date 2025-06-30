using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;

public class LogiTrackContext : DbContext
{
    // ✅ DbSets tell EF Core to create tables for these entity classes
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }   // ✅ New DbSet for Customer table
    public DbSet<User> Users { get; set; }


    // ✅ Database connection string points to local SQLite file
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=logitrack.db");

    // ✅ Configure relationships and delete behavior
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One Order has many InventoryItems
        // Each InventoryItem optionally belongs to one Order
        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        // ✅ One Customer has many Orders
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
