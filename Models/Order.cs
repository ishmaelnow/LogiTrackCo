using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiTrack.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }      // Primary key

        public int CustomerId { get; set; }   // Foreign key to Customer

        public Customer? Customer { get; set; }  // Navigation property to Customer

        public DateTime DatePlaced { get; set; }  // Date order was placed

        public List<InventoryItem> Items { get; set; } = new List<InventoryItem>();

        // Optional methods for item management
        public void AddItem(InventoryItem item)
        {
            Items.Add(item);
        }

        public void RemoveItem(int itemId)
        {
            Items.RemoveAll(i => i.ItemId == itemId);
        }

        public string GetOrderSummary()
        {
            return $"Order #{OrderId} for {Customer?.Name} | Items: {Items.Count} | Placed: {DatePlaced.ToShortDateString()}";
        }
    }
}
