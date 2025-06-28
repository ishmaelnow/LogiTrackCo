using LogiTrack.Models;   // Gives access to InventoryItem, Order, Customer

namespace LogiTrack         // Must match your project namespace exactly
{
    public static class DbInitializer   // Static class that handles one-time test data setup
    {
        public static void Seed(LogiTrackContext context)
        {
            // ✅ Seed InventoryItems if table is empty
            if (!context.InventoryItems.Any())
            {
                context.InventoryItems.AddRange(
                    new InventoryItem
                    {
                        Name = "Pallet Jack",
                        Quantity = 12,
                        Location = "Warehouse A"
                    },
                    new InventoryItem
                    {
                        Name = "Forklift",
                        Quantity = 3,
                        Location = "Warehouse B"
                    }
                );

                context.SaveChanges();
                Console.WriteLine("✅ Seeded InventoryItems");
            }

            // ✅ Seed Customer if none exist
            if (!context.Customers.Any())
            {
                context.Customers.Add(new Customer { Name = "Samir" });
                context.SaveChanges();
                Console.WriteLine("✅ Seeded Customer");
            }

            // ✅ Seed Order linked to the first customer if no orders exist
            if (!context.Orders.Any())
            {
                var customer = context.Customers.FirstOrDefault();
                if (customer != null)
                {
                    context.Orders.Add(new Order
                    {
                        CustomerId = customer.CustomerId,
                        DatePlaced = DateTime.Now
                    });

                    context.SaveChanges();
                    Console.WriteLine("✅ Seeded Orders");
                }
            }
        }
    }
}
