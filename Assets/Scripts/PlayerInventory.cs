using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public SeedData seedData;  // The type of seed in this slot
    public int quantity;       // How many seeds are available
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Seeds Inventory")]
    public List<InventorySlot> seedInventory = new List<InventorySlot>();

    [Header("Money")]
    public int money = 0;

    // Add seeds to inventory
    public void AddSeed(SeedData seed, int amount)
    {
        var slot = seedInventory.Find(s => s.seedData == seed);
        if (slot != null)
        {
            slot.quantity += amount;
        }
        else
        {
            seedInventory.Add(new InventorySlot { seedData = seed, quantity = amount });
        }
    }

    // Use one seed when planting
    public bool UseSeed(SeedData seed)
    {
        var slot = seedInventory.Find(s => s.seedData == seed);

        if (slot == null)
        {
            Debug.LogWarning($"No inventory slot found for seed: {seed.seedName}");
            return false;
        }

        if (slot.quantity > 0)
        {
            slot.quantity--;
            Debug.Log($"{seed.seedName} planted! Remaining seeds: {slot.quantity}");
            return true;
        }

        Debug.LogWarning($"Cannot plant {seed.seedName} — quantity is 0!");
        return false;
    }

}
