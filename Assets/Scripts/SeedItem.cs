using UnityEngine;

[CreateAssetMenu(fileName = "NewSeed", menuName = "Inventory/SeedItem")]
public class SeedItem : ScriptableObject
{
    public string seedName;
    public GameObject cropPrefab; // Prefab to plant
}
