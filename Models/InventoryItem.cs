using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LogiTrack.Models;

public class InventoryItem
{
    [Key]
    public int ItemId { get; set; }

    public required string Name { get; set; }
    public int Quantity { get; set; }
    public required string Location { get; set; }

    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public void DisplayInfo()
    {
        Console.WriteLine($"Item: {Name} | Quantity: {Quantity} | Location: {Location}");
    }
}
