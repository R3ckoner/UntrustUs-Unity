using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<SeedItem> inventory = new List<SeedItem>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventory.Count > 0)
        {
            SelectSeed(inventory[0]);
        }
    }

    public void AddSeed(SeedItem seed)
    {
        inventory.Add(seed);
        Debug.Log($"{seed.seedName} added to inventory.");
    }

    public void SelectSeed(SeedItem seed)
    {
        FindObjectOfType<GridGenerator>().SelectSeed(seed);
    }
}
