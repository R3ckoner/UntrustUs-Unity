using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<SeedItem> inventory = new List<SeedItem>();

    void Update()
    {
        // Select the first seed in the inventory with the '1' key
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventory.Count > 0)
        {
            SelectSeed(inventory[0]);
        }
    }

    public void AddSeed(SeedItem seed)
    {
        inventory.Add(seed);
        Debug.Log($"Picked up {seed.seedName}");
    }

    public void SelectSeed(SeedItem seed)
    {
        FindObjectOfType<GridGenerator>().SelectSeed(seed);
        Debug.Log($"{seed.seedName} selected for planting.");
    }
}
