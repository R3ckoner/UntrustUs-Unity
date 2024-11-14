using UnityEngine;

public class SeedPickup : MonoBehaviour
{
    public SeedItem seed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventory = other.GetComponent<InventoryManager>();
            if (inventory != null)
            {
                inventory.AddSeed(seed);
                Destroy(gameObject); // Destroy the seed pickup after collection
            }
        }
    }
}
